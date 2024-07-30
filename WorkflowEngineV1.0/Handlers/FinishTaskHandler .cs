using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class FinishTaskHandler : TaskHandler
    {
        protected override bool CanHandle(TaskItem task) => task.Name == "Finish";

        protected override async Task Execute(TaskItem task, WorkflowEngine workflowEngine)
        {
            task.State = TaskState.Working;
            await workflowEngine.UpdateTaskState(task);
            task.State = TaskState.Completed;
            await workflowEngine.UpdateTaskState(task);
            workflowEngine.CompleteWorkflow(task.WorkflowId.Value);
        }

        protected override TaskItem GetNextTask(TaskItem task, WorkflowEngine workflowEngine)
        {
            return null; // No next task for the finish task
        }
    }
}
