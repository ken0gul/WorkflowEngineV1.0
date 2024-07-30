using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class SendEmailTaskHandler : TaskHandler
    {
        protected override bool CanHandle(TaskItem task) => task.Name == "Send E-mail";
      

        protected override async Task Execute(TaskItem task, WorkflowEngine workflowEngine)
        {
            task.State = TaskState.Working;
            await workflowEngine.UpdateTaskState(task);
            await workflowEngine.SendEmail();
            task.State = TaskState.Completed;
            await workflowEngine.UpdateTaskState(task);
        }

        protected override TaskItem GetNextTask(TaskItem task, WorkflowEngine workflowEngine)
        {
            return workflowEngine.GetNextTask(task);
        }

     
    }
}
