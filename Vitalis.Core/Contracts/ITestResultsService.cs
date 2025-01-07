using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;
using Vitalis.Core.Models.Tests;

namespace Vitalis.Core.Contracts
{
    public interface ITestResultsService
    {
        Task SaveStudentTestAnswer(List<OpenQuestionSubmitViewModel> openQuestions,
            List<ClosedQuestionViewModel> closedQuestions, Guid testId, string userId);

        float CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, float MaxScore);

        string[] GetTestTakersIds(Guid testId);

        Task<TestStatsViewModel> GetStatistics(Guid testId);

        Task<TestReviewViewModel> GetUsersTestResults(Guid testId, string userId);

        Task<IEnumerable<TestResultsViewModel>> GetUsersTestsResults(string userId);

        Task SubmitTestScore(Guid testId, string userId, TestReviewViewModel scoredTest);
    }
}
