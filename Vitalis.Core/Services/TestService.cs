﻿using Microsoft.EntityFrameworkCore;
using Vitalis.Core.Contracts;
using Vitalis.Core.Infrastructure;
using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;
using Vitalis.Core.Models.Tests;
using Vitalis.Data;
using Vitalis.Data.Entities;

namespace Vitalis.Core.Services
{
    public class TestService : ITestService
    {
        private readonly VitalisDbContext context;

        public TestService(VitalisDbContext _context)
        {
            context = _context;
        }

        private static Func<IEnumerable<TestResult>, float> CalculateAverageScore = t => (float)Math.Round(
            !t.Any()
            ? 0
            : t.Average(tr => tr.Score), 2);

        private Func<Test, string, TestViewModel> ToViewModel = (t, userId) => new TestViewModel()
        {
            AverageScore = CalculateAverageScore(t.TestResults),
            CreatedOn = t.CreatedOn.ToShortDateString(),
            Description = t.Description,
            Grade = t.Grade,
            Id = t.Id,
            Title = t.Title,
            IsCreator = t.CreatorId == userId,
            IsTestTaken = t.TestResults.Any(tr => tr.TestTakerId == userId),
            QuestionsCount = t.ClosedQuestions.Count + t.OpenQuestions.Count,
            Groups = t.Groups.Select(x => x.OrganicGroup.Name).ToList(),
            TestTakers = t.TestResults.Count(),
        };

        private Func<Test, RawTestViewModel> ToRawViewModel = t => new RawTestViewModel()
        {
            OpenQuestions = t.OpenQuestions.ToList(),
            ClosedQuestions = t.ClosedQuestions.ToList(),
            Id = t.Id,
            QuestionsOrder = t.QuestionsOrder,
            TestTitle = t.Title
        };

        private List<QuestionType> ProcessQuestionOrder(string questionOrderText)
        {
            return questionOrderText.Split(Constants.SeparationCharacter, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
            .ToList();
        }

        private async Task<QueryModel<TestViewModel>> Filter(IQueryable<Test> testQuery, QueryModel<TestViewModel> query,
            string userId = "")
        {
            testQuery = testQuery.Where(t => !query.Filters.Groups
                                                   .Except(t.Groups.Select(x => x.OrganicGroupId))
                                                   .Any());

            if (query.Filters.Grade >= 1 && query.Filters.Grade <= 12)
            {
                testQuery = testQuery.Where(t => t.Grade == query.Filters.Grade);
            }

            if (string.IsNullOrEmpty(query.Filters.SearchTerm) == false)
            {
                query.Filters.SearchTerm = $"%{query.Filters.SearchTerm.ToLower()}%";

                testQuery = testQuery
                    .Where(t => EF.Functions.Like(t.Title.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(t.Creator.Name.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(t.Description.ToLower(), query.Filters.SearchTerm));
            }

            //if (query.Filters.Sorting == Sorting.Likes)
            //{
            //    testQuery = testQuery.OrderBy(t => t.TestLikes.Count());
            //}
            if (query.Filters.Sorting == Sorting.TestTakers)
            {
                testQuery = testQuery.OrderByDescending(t => t.TestResults.Count());
            }
            else if (query.Filters.Sorting == Sorting.Questions)
            {
                testQuery = testQuery.OrderByDescending(t => t.OpenQuestions.Count + t.ClosedQuestions.Count);
            }
            else if (query.Filters.Sorting == Sorting.Score)
            {
                testQuery = testQuery.OrderByDescending(t => t.TestResults.Average(r => r.Score));
            }

            var dbTests = testQuery.Skip(query.ItemsPerPage * (query.CurrentPage - 1))
                                .Take(query.ItemsPerPage)
                                .Include(t => t.ClosedQuestions)
                                .Include(t => t.OpenQuestions)
                                .Include(g => g.TestResults)
                                .Include(t => t.Groups)
                                .ThenInclude(g => g.OrganicGroup)
                                .Select(x => ToViewModel(x, userId));
            var tests = await dbTests.ToListAsync();
            query.Items = tests;
            query.TotalPages = (int)Math.Ceiling(tests.Count * 1.0 / query.ItemsPerPage);
            return query;
        }

        public async Task<QueryModel<TestViewModel>> GetAll(QueryModel<TestViewModel> query, string userId)
        {
            var testQuery = context.Tests
                                   .Include(t => t.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted && t.IsPublic);
            return await Filter(testQuery, query, userId);
        }

        public async Task<QueryModel<TestViewModel>> GetMy(string userId,
            QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(g => g.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted && t.CreatorId == userId);
            return await Filter(testQuery, query, userId);
        }

        public async Task<TestViewModel?> GetById(Guid id, string userId)
        {
            var test = await context.Tests
                                 .Where(t => !t.IsDeleted)
                                 .Include(t => t.OpenQuestions)
                                 .Include(t => t.ClosedQuestions)
                                 .Include(g => g.TestResults)
                                 .Include(t => t.TestLikes)
                                 .FirstOrDefaultAsync(t => t.Id == id);
            if (test is null || (!test.IsPublic && test.CreatorId == userId))
            {
                return null;
            }

            return ToViewModel(test, userId);
        }

        public async Task<TestSubmitViewModel?> GetTestSubmitViewModel(Guid testId, string userId)
        {
            var test = await context.Tests
                                    .Include(t => t.OpenQuestions)
                                    .Include(t => t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t => t.Id == testId);
            if (test is null || (!test.IsPublic && test.CreatorId != userId))
            {
                return null;
            }

            return new TestSubmitViewModel()
            {
                OpenQuestions = test.OpenQuestions
                                    .Where(q => !q.IsDeleted)
                                    .Select(q => new OpenQuestionSubmitViewModel()
                                    {
                                        Text = q.Text,
                                        Id = q.Id,
                                        MaxScore = q.MaxScore,
                                        ImagePath = q.ImagePath,
                                        Answer = ""
                                    })
                                    .ToList(),
                ClosedQuestions = test.ClosedQuestions
                                      .Where(q => !q.IsDeleted)
                                      .Select(q => new ClosedQuestionViewModel()
                                      {
                                          Options = q.Answers.Split(Constants.SeparationCharacter),
                                          IsDeleted = false,
                                          Text = q.Text,
                                          Id = q.Id,
                                          MaxScore = q.MaxScore,
                                          ImagePath = q.ImagePath,
                                          AnswerIndexes =
                                              new bool[q.Answers.Count(x => x == Constants.SeparationCharacter) + 1]
                                      })
                                      .ToList(),
                QuestionsOrder = ProcessQuestionOrder(test.QuestionsOrder),
                Title = test.Title,
                Id = test.Id,
            };
        }

        public async Task<string> Edit(Guid id, TestEditViewModel model, string userId)
        {
            var test = await context.Tests
                                    .Include(t => t.OpenQuestions)
                                    .Include(t => t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t => t.Id == id);
            if (test.CreatorId != userId)
            {
                return Constants.NotFoundResponse;
            }

            test.OpenQuestions = test.OpenQuestions.Select(q => EditOpenQuestion(model.OpenQuestions, q))
                                     .Where(q => !string.IsNullOrEmpty(q.Text))
                                     .Union(model.OpenQuestions
                                                 .Select(q => new OpenQuestion()
                                                 {
                                                     Text = q.Text,
                                                     Answer = q.Answer,
                                                     MaxScore = q.MaxScore,
                                                     ImagePath = q.ImagePath
                                                 }))
                                     .ToList();

            test.ClosedQuestions = test.ClosedQuestions.Select(q => EditClosedQuestion(model.ClosedQuestions, q))
                                       .Where(q => !string.IsNullOrEmpty(q.Text))
                                       .Union(model.ClosedQuestions
                                                   .Select(q => new ClosedQuestion()
                                                   {
                                                       Text = q.Text,
                                                       AnswerIndexes = string.Join(Constants.SeparationCharacter,
                                                           q.AnswerIndexes
                                                            .Select((val, indx) =>
                                                                new { val, indx })
                                                            .Where(q => q.val)
                                                            .Select(q => q.indx)),
                                                       Answers = string.Join(
                                                           Constants.SeparationCharacter,
                                                           q.Options.Where(a =>
                                                               !string.IsNullOrEmpty(a))),
                                                       MaxScore = q.MaxScore,
                                                       ImagePath = q.ImagePath
                                                   }))
                                       .ToList();
            test.QuestionsOrder = string.Join(Constants.SeparationCharacter, model.QuestionsOrder.Select(q => q.ToString()[0]));

            test.Title = model.Title;
            test.Description = model.Description;
            test.Grade = model.Grade;
            test.IsPublic = model.IsPublic;
            await context.SaveChangesAsync();

            return Constants.SuccessResponse;
        }

        private Func<List<OpenQuestionViewModel>, OpenQuestion, OpenQuestion> EditOpenQuestion =
            (allQuestions, question) =>
            {
                var testQuestion =
                    allQuestions.FirstOrDefault(q => q.Answer == question.Answer || q.Text == question.Text);
                if (testQuestion is null)
                {
                    return new OpenQuestion();
                }

                allQuestions.Remove(testQuestion);
                question.Text = testQuestion.Text;
                question.Answer = testQuestion.Answer;
                question.MaxScore = testQuestion.MaxScore;
                question.ImagePath = testQuestion.ImagePath;
                return question;
            };

        private Func<List<ClosedQuestionViewModel>, ClosedQuestion, ClosedQuestion> EditClosedQuestion =
            (allQuestions, question) =>
            {
                var modelQuestion = allQuestions
                    .FirstOrDefault(q => CheckForSameAnswers(q, question.Answers) || q.Text == question.Text);
                if (modelQuestion is null)
                {
                    return new ClosedQuestion();
                }

                allQuestions.Remove(modelQuestion);

                question.Text = modelQuestion.Text;
                question.Answers = string.Join(
                    Constants.SeparationCharacter, modelQuestion.Options.Where(a => !string.IsNullOrEmpty(a)));

                question.AnswerIndexes = string.Join(Constants.SeparationCharacter, modelQuestion.AnswerIndexes
                                                                       .Select((val, indx) =>
                                                                           new { val, indx })
                                                                       .Where(q => q.val)
                                                                       .Select(q => q.indx));
                question.MaxScore = modelQuestion.MaxScore;
                question.ImagePath = modelQuestion.ImagePath;
                return question;
            };

        private static bool CheckForSameAnswers(ClosedQuestionViewModel questionViewModel, string savedQuestionAnswers)
        {
            return string.Join("&", questionViewModel.Options.Where(a => !string.IsNullOrEmpty(a))) ==
                   savedQuestionAnswers;
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsTestTakenByUser(Guid testId, string userId)
        {
            return await context.Tests.Where(t => t.Id == testId)
                          .SelectMany(t => t.TestResults)
                          .AnyAsync(tr => tr.TestTakerId == userId);
        }


        public async Task<QueryModel<TestViewModel>> TestsTakenByStudent(string userId,
            QueryModel<TestViewModel> query)
        {
            var testsQuery = context.TestResults
                                    .Include(g => g.Test)
                                    .ThenInclude(t => t.TestLikes)
                                    .Where(t => t.TestTakerId == userId)
                                    .Select(tr => tr.Test);
            return await Filter(testsQuery, query, userId);
        }

        public async Task<Guid> Create(TestCreateViewModel model, string userId)
        {
            //var groupIds = BoolArrayToIndexes(model.Groups);
            var groups = await context.OrganicGroups.Where(x => model.Groups[x.Id]).ToListAsync();
            Test test = new Test()
            {
                Title = model.Title,
                Description = model.Description,
                Grade = model.Grade,
                CreatedOn = DateTime.UtcNow,
                CreatorId = userId,
                IsPublic = model.IsPublic,
                QuestionsOrder = "",
                Groups = groups.Select(x => new TestOrganicGroup()
                {
                    OrganicGroup = x
                }).ToList()
            };
            var e = await context.Tests.AddAsync(test);
            await context.SaveChangesAsync();
            return e.Entity.Id;
        }

        //private List<int> BoolArrayToIndexes(bool[] array)
        //{
        //    List<int> resultIndexes = new List<int>();
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        if (array[i])
        //        {
        //            resultIndexes.Add(i);
        //        }
        //    }
        //    return resultIndexes;
        //}

        public async Task<bool> TestExistsById(Guid id)
        {
            return await context.Tests
                                .Where(c => !c.IsDeleted)
                                .AnyAsync(t => t.Id == id);
        }

        public async Task DeleteTest(Guid id)
        {
            var test = await context.Tests.FindAsync(id);
            test.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsTestCreator(Guid testId, string userId)
        {
            var user = await context.Users
                                       .Include(x => x.Tests)
                                       .FirstOrDefaultAsync(t => t.Id == userId);
            if (user is null)
            {
                return false;
            }

            return user.Tests.Any(t => t.Id == testId);
        }

        public async Task<TestEditViewModel> GetTestEditViewModel(Guid testId, string userId)
        {
            var test = await context.Tests
                                    .Include(t => t.OpenQuestions)
                                    .Include(t => t.ClosedQuestions)
                                    .Include(test => test.Groups)
                                    .FirstOrDefaultAsync(t => t.Id == testId);
            if (test is null || (!test.IsPublic && test.CreatorId == userId))
            {
                return null;
            }

            var allOrganicGroups = Enum.GetValues(typeof(Models.Tests.OrganicGroup)).Cast<int>();
            var testsOrganicGroups = test.Groups.Select(x => x.OrganicGroupId);
            var checkboxList = allOrganicGroups.Select(x => testsOrganicGroups.Contains(x)).ToList();

            var t = new TestEditViewModel()
            {
                Groups = checkboxList,
                OpenQuestions = test.OpenQuestions
                                             .Where(q => !q.IsDeleted)
                                             .Select(q => new OpenQuestionViewModel()
                                             {
                                                 Answer = q.Answer,
                                                 IsDeleted = false,
                                                 Text = q.Text,
                                                 MaxScore = q.MaxScore,
                                                 ImagePath = q.ImagePath
                                             })
                                             .ToList(),
                ClosedQuestions = test.ClosedQuestions
                                               .Where(q => !q.IsDeleted)
                                               .Select(q => new ClosedQuestionViewModel()
                                               {
                                                   Options = q.Answers.Split(Constants.SeparationCharacter),
                                                   AnswerIndexes =
                                                                    ProcessAnswerIndexes(q.Answers.Split(Constants.SeparationCharacter),
                                                                        q.AnswerIndexes),
                                                   IsDeleted = false,
                                                   Text = q.Text,
                                                   MaxScore = q.MaxScore,
                                                   ImagePath = q.ImagePath
                                               })
                                               .ToList(),
                QuestionsOrder = ProcessQuestionOrder(test.QuestionsOrder),
                Description = test.Description,
                Grade = test.Grade,
                Title = test.Title,
                Id = test.Id,
                IsPublic = test.IsPublic
            };
            return t;
        }

        private bool[] ProcessAnswerIndexes(string[] answers, string answerIndexes)
        {
            var list = Enumerable.Repeat(false, answers.Length).ToArray();
            if (answerIndexes == "")
            {
                return list;
            }

            var listOfIndx = answerIndexes.Split(Constants.SeparationCharacter).Select(int.Parse);
            for (int i = 0; i < list.Length; i++)
            {
                if (listOfIndx.Contains(i))
                {
                    list[i] = true;
                }
            }

            return list;
        }

        //public async Task<QueryModel<TestViewModel>> GetAllAdmin(QueryModel<TestViewModel> query)
        //{
        //    //TODO Admin test results
        //    var testQuery = context.Tests
        //                           //.Include(t => t.TestResults)
        //                           .Include(t => t.TestLikes)
        //                           .Where(t => !t.IsDeleted);
        //    return await Filter(testQuery, query, null, null);
        //}
    }
}