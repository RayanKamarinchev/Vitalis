using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    public class TestResultsService : ITestResultsService
    {
        private readonly VitalisDbContext context;
        private readonly IConfiguration config;
        //private readonly TestResultsProcessorService testResultsProcessor;

        public TestResultsService(VitalisDbContext _context, IConfiguration _config)
        {
            context = _context;
            config = _config;
            //if (!(string.IsNullOrEmpty(config["ConnectionString"]) || string.IsNullOrEmpty(config["OpenAIKey"])))
            //{
            //    testResultsProcessor =
            //        new TestResultsProcessorService(config["OpenAIKey"], config["ConnectionString"], _context);
            //}
        }

        private Func<TestResult, TestResultsViewModel> ToResultsViewModel = t => new TestResultsViewModel()
        {
            TakenOn = t.TakenOn,
            Title = t.Test.Title,
            Description = t.Test.Description,
            Score = t.Score,
            TestId = t.TestId
        };

        private List<QuestionType> ProcessQuestionOrder(string questionOrderText)
        {
            return questionOrderText.Split(Constants.SeparationCharacter, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
                                    .ToList();
        }


        public async Task SaveStudentTestAnswer(List<OpenQuestionSubmitViewModel> openQuestions,
            List<ClosedQuestionViewModel> closedQuestions, Guid testId, string userId)
        {
            if (context.TestResults.Any(t => t.TestId == testId && t.TestTakerId == userId))
            {
                return;
            }
            var test = await context.Tests
                                 .Include(t => t.ClosedQuestions)
                                 .Include(t=>t.OpenQuestions)
                                 .FirstOrDefaultAsync(t => t.Id == testId);

            var open = openQuestions?.Select(q => new OpenQuestionAnswer()
            {
                UserAnswer = q.Answer,
                QuestionId = q.Id,
                UserId = userId,
                Score = test.OpenQuestions.FirstOrDefault(x=>x.Id == q.Id).Answer == q.Answer ? q.MaxScore : 0
            });
            var closed = closedQuestions?.Select(q =>
            {
                var closedQuestion = test.ClosedQuestions.FirstOrDefault(x => x.Id == q.Id);
                return new ClosedQuestionAnswer()
                {
                    UserAnswersIndexes = string.Join("&", q.UsersAnswersArray
                                                      .Select((val, indx) => new { val, indx })
                                                      .Where(q => q.val)
                                                      .Select(q => q.indx)),
                    QuestionId = q.Id,
                    UserId = userId,
                    Score = CalculateClosedQuestionScore(q.UsersAnswersArray,
                        closedQuestion.Answers.Select(x=>x.IsCorrect).ToArray(), (decimal)closedQuestion.MaxScore)
                };
            });

            if (open != null)
            {
                await context.OpenQuestionAnswers.AddRangeAsync(open);
            }

            if (closed != null)
            {
                await context.ClosedQuestionAnswers.AddRangeAsync(closed);
            }

            await context.SaveChangesAsync();

            await context.TestResults.AddAsync(new TestResult()
            {
                Score = 0,
                TestId = testId,
                TestTakerId = userId,
                TakenOn = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        private decimal CalculateClosedQuestionScore(bool[] answers, bool[] correctAnswers, decimal maxScore)
        {
            decimal score = 0;
            int attempts = 0;
            for (int i = 0; i < answers.Length; i++)
            {
                if (answers[i])
                {
                    attempts++;
                    if (correctAnswers[i])
                    {
                        score++;
                    }
                }
            }
            int correctAnswersCount = correctAnswers.Length;
            score -= attempts - correctAnswersCount;
            score *= maxScore / correctAnswersCount;
            if (score < 0)
            {
                score = 0;
            }

            return score;
        }


        public string[] GetTestTakersIds(Guid testId)
        {
            return context.TestResults
                          .Where(tr => tr.TestId == testId)
                          .Select(x => x.TestTakerId)
                          .ToArray();
        }

        public async Task<TestStatsViewModel> GetStatistics(Guid testId)
        {
            TestStatsViewModel testStatistics = new TestStatsViewModel();

            var testTakersIds = GetTestTakersIds(testId);
            List<TestReviewViewModel> res = new List<TestReviewViewModel>();
            foreach (var studentId in testTakersIds)
            {
                res.Add(await GetUsersTestResults(testId, studentId));
            }

            var test = await context.Tests
                                 .Include(g => g.TestResults)
                                 .FirstOrDefaultAsync(g => g.Id == testId);

            testStatistics.AverageScore =
                (float)Math.Round(!test.TestResults.Any() ? 0 : test.TestResults.Average(r => r.Score), 2);
            testStatistics.Title = test.Title;
            testStatistics.TestTakers = test.TestResults.Count();
            testStatistics.QuestionOrder = ProcessQuestionOrder(test.QuestionsOrder);

            List<List<List<int>>> allClosedAnswers = new List<List<List<int>>>();
            res.ForEach(r =>
            {
                List<List<int>> answers = new List<List<int>>();
                r.ClosedQuestions.ForEach(q =>
                {
                    answers.Add(new List<int>());
                    for (int i = 0; i < q.Options.Length; ++i)
                    {
                        if (q.UsersAnswersArray[i])
                        {
                            answers.Last().Add(i);
                        }
                    }
                });
                allClosedAnswers.Add(answers);
            });
            if (allClosedAnswers.Count > 0)
            {
                for (int i = 0; i < allClosedAnswers[0].Count; i++)
                {
                    testStatistics.ClosedQuestions.Add(new ClosedQuestionStatsViewModel()
                    {
                        StudentAnswers = allClosedAnswers.Select(a => a[i]).ToList(),
                        Text = res[0].ClosedQuestions[i].Text,
                        Answers = res[0].ClosedQuestions[i].Options,
                        ImagePath = res[0].ClosedQuestions[i].ImagePath
                    });
                }
            }

            List<List<string>> allOpenAnswers = new List<List<string>>();
            res.ForEach(r =>
            {
                List<string> answers = new List<string>();
                r.OpenQuestions.ForEach(q => { answers.Add(q.Answer); });
                allOpenAnswers.Add(answers);
            });
            if (allOpenAnswers.Count > 0)
            {
                for (int i = 0; i < allOpenAnswers[0].Count; i++)
                {
                    testStatistics.OpenQuestions.Add(new OpenQuestionStatsViewModel()
                    {
                        StudentAnswers = allOpenAnswers.Select(a => a[i]).ToList(),
                        Text = res[0].OpenQuestions[i].Text,
                        ImagePath = res[0].OpenQuestions[i].ImagePath
                    });
                }
            }

            return testStatistics;
        }

        public async Task<TestReviewViewModel> GetUsersTestResults(Guid testId, string userId)
        {
            var testResult = await context.TestResults.Include(r => r.Test)
                                        .FirstOrDefaultAsync(r => r.TestTakerId == userId && r.TestId == testId);
            //TODO
            //if (testResultsProcessor is not null)
            //{
            //    testResultsProcessor.setOpenQuesitons(openQuestionAnswers);
            //    await testResultsProcessor.StartAsync(new CancellationToken());
            //}

            try
            {
                var openQuestionsViewModels = await context.OpenQuestionAnswers
                                                           .Where(q => q.UserId == userId &&
                                                                       q.Question.TestId == testId)
                                                           .Include(q => q.Question)
                                                           .Select(q => new OpenQuestionReviewViewModel()
                                                           {
                                                               Text = q.Question.Text,
                                                               Id = q.Id,
                                                               CorrectAnswer = q.Question.Answer,
                                                               MaxScore = q.Question.MaxScore,
                                                               Answer = q.UserAnswer,
                                                               Score = q.Score,
                                                               Explanation = q.Explanation,
                                                               ImagePath = q.Question.ImagePath
                                                           })
                                                           .ToListAsync();
                var closedQuestions = new List<ClosedQuestionReviewViewModel>();
                var db = context.ClosedQuestionAnswers
                                .Where(q => q.UserId == userId && q.Question.TestId == testId)
                                .Include(q => q.Question);
                foreach (var q in db)
                {
                    var closedQuestionModel = new ClosedQuestionReviewViewModel()
                    {
                        Options = q.Question.Answers.Select(x=>x.Text).ToArray(),
                        IsDeleted = false,
                        Text = q.Question.Text,
                        Id = q.Id,
                        UsersAnswersArray = ProcessAnswerIndexes(q.Question.Answers, q.UserAnswersIndexes),
                        CorrectAnswersArray = q.Question.Answers.Select(x=>x.IsCorrect).ToArray(),
                        MaxScore = q.Question.MaxScore,
                        ImagePath = q.Question.ImagePath,
                        Score = (float)q.Score
                    };
                    closedQuestions.Add(closedQuestionModel);
                }

                return new TestReviewViewModel()
                {
                    OpenQuestions = openQuestionsViewModels,
                    ClosedQuestions = closedQuestions,
                    Score = closedQuestions.Sum(q => q.Score)
                                   + openQuestionsViewModels.Sum(q => q.Score),
                    QuestionsOrder = testResult.Test.QuestionsOrder
                                                     .Split(Constants.SeparationCharacter)
                                                     .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
                                                     .ToList()
                };
            }
            catch (InvalidOperationException e)
            {
            }

            return new TestReviewViewModel()
            {
                OpenQuestions = new List<OpenQuestionReviewViewModel>(),
                ClosedQuestions = new List<ClosedQuestionReviewViewModel>()
            };
        }

        private bool[] ProcessAnswerIndexes(IEnumerable<Answer> answers, string answerIndexes)
        {
            var list = Enumerable.Repeat(false, answers.Count()).ToArray();
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

        public async Task<IEnumerable<TestResultsViewModel>> GetUsersTestsResults(string userId)
        {
            return await context.TestResults
                                .Where(t => t.TestTakerId == userId)
                                .Select(t => ToResultsViewModel(t))
                                .ToListAsync();
        }

        public async Task SubmitTestScore(Guid testId, string userId, TestReviewViewModel scoredTest)
        {
            var openQuestionAnswers = await context.OpenQuestionAnswers
                                                   .Include(q => q.Question)
                                                   .Where(q => q.UserId == userId
                                                               && q.Question.TestId == testId)
                                                   .ToListAsync();
            foreach (var openQuestionAnswer in openQuestionAnswers)
            {
                var scoredQuestion = scoredTest.OpenQuestions.FirstOrDefault(q => q.Id == openQuestionAnswer.Id);
                openQuestionAnswer.Explanation = scoredQuestion.Explanation;
                openQuestionAnswer.Score = scoredQuestion.Score;
            }

            await context.SaveChangesAsync();
        }
    }
}