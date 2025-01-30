using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitalis.Core.Contracts;
using Vitalis.Core.Infrastructure;
using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;
using Vitalis.Core.Models.Tests;

namespace Vitalis.Controllers
{
    [ApiController]
    [Authorize]
    [Route("tests")]
    public class TestsController : ControllerBase
    {
        private readonly ITestService testService;
        private readonly ITestResultsService testResultsService;
        //private readonly IMemoryCache cache;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Random random = new Random();

        public TestsController(ITestService _testService, ITestResultsService testResultsService, IWebHostEnvironment webHostEnvironment)
        {
            testService = _testService;
            //cache = _cache;
            this.testResultsService = testResultsService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int grade = 0, List<int>? groups = null, Sorting sorting = Sorting.TestTakers,
            int currentPage = 1)
        {
            groups = new List<int>();
            if (currentPage == 0)
            {
                currentPage = 1;
            }

            QueryModel<TestViewModel> query =
                new QueryModel<TestViewModel>(searchTerm, grade, groups, sorting, currentPage);
            var model = await testService.GetAll(query, User.Id());
            //var cacheEntryOptions = new DistributedCacheEntryOptions()
            //    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            //cache.SetAsync(Constants.TestsCacheKey, model, cacheEntryOptions);

            return Ok(model);
        }

        [HttpGet("mine")]
        public async Task<QueryModel<TestViewModel>> MyTests([FromQuery] QueryModel<TestViewModel> query)
        {
            //get user id
            QueryModel<TestViewModel> model = await testService.GetMy(User.Id(), query);
            return model;
        }

        [Route("edit/{testId}")]
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
        [Route("edit/{testId}")]
        public async Task<IActionResult> Edit(Guid testId, TestEditViewModel model)
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
            return Content("success");
        }

        private bool AllQuestionsHaveAnswerIndexes(List<ClosedQuestionViewModel> closedQuestions)
        {
            if (closedQuestions is null)
            {
                return true;
            }

            return closedQuestions.All(c => c.AnswerIndexes.Any(ai => ai));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TestCreateViewModel model)
        {
            if (ModelState.ErrorCount != 1 && !ModelState.IsValid)
            {
                return BadRequest();
            }
            Guid id = await testService.Create(model, User.Id());
            return Ok(id);
        }

        [HttpGet]
        [Route("take/{testId}")]
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
        [Route("take/{testId}")]
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
        [Route("review/{testId}/{studentId}")]
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

        [HttpGet("stats")]
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
        [Route("delete/{Id}")]
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
