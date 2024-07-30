using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class StartTaskHandler : TaskHandler
    {
        protected override bool CanHandle(TaskItem task) => task.Name == "start";
        

        protected override async Task Execute(TaskItem taskItem, WorkflowEngine workflowEngine)
        {

            Console.WriteLine("StartTaskHandler is getting executed!!!!!!!!!!!!!!!!!!!!!!");
            taskItem.State = TaskState.Working;
            await workflowEngine.UpdateTaskState(taskItem);
            taskItem.State = TaskState.Completed;
            await workflowEngine.UpdateTaskState(taskItem);
        }

        protected override TaskItem GetNextTask(TaskItem task, WorkflowEngine workflowEngine)
        {
            return workflowEngine.GetNextTask(task);
        }
    }
}
