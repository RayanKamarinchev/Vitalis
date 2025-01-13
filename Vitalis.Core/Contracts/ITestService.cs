using Vitalis.Core.Models.Tests;
using Vitalis.Data.Entities;

namespace Vitalis.Core.Contracts
{
    public interface ITestService
    {
        Task<QueryModel<TestViewModel>> GetAll(QueryModel<TestViewModel> query);

        Task<QueryModel<TestViewModel>> GetMy(string userId, QueryModel<TestViewModel> query);

        Task<TestViewModel?> GetById(Guid id, string userId);

        Task<TestSubmitViewModel> GetTestSubmitViewModel(Guid testId, string userId);

        Task<string> Edit(Guid id, TestEditViewModel model, string userId);

        Task SaveChanges();

        Task<bool> IsTestTakenByUser(Guid testId, string userId);

        Task<QueryModel<TestViewModel>> TestsTakenByStudent(string userId,
            QueryModel<TestViewModel> query);

        Task<Guid> Create(TestCreateViewModel model, string userId);

        Task<bool> TestExistsById(Guid id);

        Task DeleteTest(Guid id);

        Task<bool> IsTestCreator(Guid testId, string userId);

        Task<TestEditViewModel> GetTestEditViewModel(Guid testId, string userId);
    }
}
