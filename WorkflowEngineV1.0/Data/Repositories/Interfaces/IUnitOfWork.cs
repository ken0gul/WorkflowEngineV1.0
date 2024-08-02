using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Data.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<TaskItem> TaskItems { get; }
        IRepository<Connection> Connections { get; }
        IRepository<Workflow> Workflows { get; }
        IRepository<Document> Documents { get; }
        Task<int> CompleteAsync();
    }
}
