using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public abstract class TaskHandler
    {
        protected TaskHandler _nextHandler;

        public void SetNext(TaskHandler nextHandler)
        {

            _nextHandler = nextHandler;
        }

        public async Task Handle(TaskItem taskItem, WorkflowEngine workflowEngine)
        {

            if (CanHandle(taskItem))
            {
                await Execute(taskItem, workflowEngine);
                var nextTask = GetNextTask(taskItem, workflowEngine);

                if (nextTask != null)
                {
                    await _nextHandler.Handle(nextTask, workflowEngine);
                } 

            }
            else
            {
                await Execute(taskItem, workflowEngine);
            }

        }

        protected abstract bool CanHandle(TaskItem task);
        protected abstract Task Execute(TaskItem taskItem, WorkflowEngine workflowEngine);
        protected abstract TaskItem GetNextTask(TaskItem task, WorkflowEngine workflowEngine);
    }
}
