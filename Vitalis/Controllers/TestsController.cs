using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Vitalis.Core.Contracts;
using Vitalis.Core.Infrastructure;
using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;
using Vitalis.Core.Models.Tests;
using CacheExtensions = Microsoft.Extensions.Caching.Memory.CacheExtensions;

namespace Vitalis.Controllers
{
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ITestService testService;
        private readonly ITestResultsService testResultsService;
        private readonly IMemoryCache cache;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Random random = new Random();

        public TestsController(ITestService _testService, IMemoryCache _cache, ITestResultsService testResultsService, IWebHostEnvironment webHostEnvironment)
        {
            testService = _testService;
            cache = _cache;
            this.testResultsService = testResultsService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, int grade, List<OrganicGroup> groups, Sorting sorting,
            int currentPage)
        {
            if (CacheExtensions.TryGetValue(cache, Constants.TestsCacheKey, out QueryModel<TestViewModel>? model) && false)
            {
            }
            else
            {
                if (currentPage == 0)
                {
                    currentPage = 1;
                }

                QueryModel<TestViewModel> query =
                    new QueryModel<TestViewModel>(searchTerm, grade, groups, sorting, currentPage);
                model = await testService.GetAll(query);
                //var cacheEntryOptions = new DistributedCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync(Constants.TestsCacheKey, model, cacheEntryOptions);
            }

            return Ok(model);
        }

        [HttpGet]
        public async Task<QueryModel<TestViewModel>> MyTests([FromQuery] QueryModel<TestViewModel> query)
        {
            //get user id
            QueryModel<TestViewModel> model = await testService.GetMy(User.Id(), query);
            return model;
        }

        [Route("Tests/Edit/{testId}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid testId)
        {
            TestEditViewModel test = await testService.GetTestEditViewModel(testId, User.Id());
            if (test is null)
            {
                return NotFound();
            }
            return Ok(test);
        }

        private async Task SaveImage(QuestionViewModel question)
        {
            if (question.Image != null && question.Image.ContentType.StartsWith("image"))
            {
                if (question.ImagePath.StartsWith("imgs/"))
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, question.ImagePath);
                    System.IO.File.Delete(path);
                }

                string folder = "imgs/";
                folder += Guid.NewGuid() + "_" + question.Image.FileName;
                question.ImagePath = "/" + folder;
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await question.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }
        }

        [HttpPost]
        [Route("Tests/Edit/{testId}/{groupId}")]
        public async Task<IActionResult> Edit(Guid testId, [FromForm] TestEditViewModel model)
        {
            model.OpenQuestions ??= new List<OpenQuestionViewModel>();
            model.ClosedQuestions ??= new List<ClosedQuestionViewModel>();
            model.QuestionsOrder ??= new List<QuestionType>();

            foreach (var question in model.OpenQuestions)
            {
                await SaveImage(question);
            }

            foreach (var question in model.ClosedQuestions)
            {
                await SaveImage(question);
            }

            //test groups are counted as an error
            //TODO find smarter way
            if (ModelState.ErrorCount > 1 || !AllQuestionsHaveAnswerIndexes(model.ClosedQuestions))
            {
                return BadRequest();
            }

            string response = await testService.Edit(testId, model, User.Id());
            if (response == Constants.NotFoundResponse)
            {
                return NotFound();
            }
            //TODO questionable response
            return Content("redirect");
        }

        private bool AllQuestionsHaveAnswerIndexes(List<ClosedQuestionViewModel> closedQuestions)
        {
            if (closedQuestions is null)
            {
                return true;
            }

            return closedQuestions.All(c => c.AnswerIndexes.Any(ai => ai));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestViewModel model)
        {
            if (ModelState.ErrorCount != 1 && !ModelState.IsValid)
            {
                return BadRequest();
            }

            Guid id = await testService.Create(model, User.Id());
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpGet]
        [Route("Tests/Take/{testId}")]
        public async Task<IActionResult> Take(Guid testId)
        {
            var test = await testService.GetTestSubmitViewModel(testId, User.Id());
            if (test is null)
            {
                return NotFound();
            }

            if (await testService.IsTestTakenByUser(testId, User.Id()))
            {
                return Content("redirect");
            }

            return Ok(test);
        }

        [HttpPost]
        [Route("Tests/Take/{testId}")]
        public async Task<IActionResult> Take([FromBody] TestSubmitViewModel model, Guid testId)
        {
            if (!await testService.TestExistsById(testId))
            {
                return NotFound();
            }

            if (await testService.IsTestTakenByUser(testId, User.Id()))
            {
                return Ok("redirect");
            }

            //TODO could be important
            //var openQuestionIds = (string[])TempData["OpenQuestionIds"];
            //openQuestionIds ??= new string[0];
            //for (int i = 0; i < openQuestionIds.Length; i++)
            //{
            //    model.OpenQuestions[i].Id = new Guid(openQuestionIds[i]);
            //}
            //var closedQuestionIds = (string[])TempData["ClosedQuestionIds"];
            //closedQuestionIds ??= new string[0];
            //for (int i = 0; i < closedQuestionIds.Length; i++)
            //{
            //    model.ClosedQuestions[i].Id = new Guid(closedQuestionIds[i]);
            //}

            await testResultsService.SaveStudentTestAnswer(model.OpenQuestions, model.ClosedQuestions, testId, User.Id());

            return Ok("redirect");
        }

        [HttpGet]
        [Route("Tests/Review/{testId}/{studentId}")]
        public async Task<IActionResult> ReviewAnswers(Guid testId, string studentId)
        {
            if (!await testService.TestExistsById(testId))
            {
                return NotFound();
            }

            if (!await testService.IsTestTakenByUser(testId, User.Id()))
            {
                return NotFound();
            }

            var test = await testResultsService.GetUsersTestResults(testId, User.Id());
            return Ok(test);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Statistics(Guid testId)
        {
            if (!await testService.IsTestCreator(testId, User.Id()))
            {
                return NotFound();
            }

            var model = await testResultsService.GetStatistics(testId);

            return Ok(model);
        }

        [HttpGet]
        [Route("Test/Delete/{Id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await testService.IsTestCreator(id, User.Id()))
            {
                return NotFound();
            }

            await testService.DeleteTest(id);

            //TODO
            //redirect to somewhere
            return RedirectToAction("Index", "Tests");
        }

        //[HttpGet]
        //[Route("TestTakers/{testId}")]
        //public async Task<IActionResult> GetAllTestTakers(Guid testId)
        //{
        //    IEnumerable<StudentViewModel> examiners = await studentService.GetExaminers(testId);
        //    return View("ExaminersAll", examiners);
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
