using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public interface ITaskHandler
    {
        void SetNext(ITaskHandler nextHandler);
        Task Handle(TaskItem task, WorkflowEngine engine, Workflow workflow);
    }

}
