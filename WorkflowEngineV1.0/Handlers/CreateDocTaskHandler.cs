using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class CreateDocTaskHandler : ITaskHandler
    {
        private ITaskHandler _nextHandler;

        public void SetNext(ITaskHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public async Task Handle(TaskItem task, WorkflowEngine engine)
        {
            Console.WriteLine($"TaskItem is {task.Name} in CreateDocTaskHandler");
            if (task.Name == "Create Doc" && task.State == TaskState.Working)
            {
                // Complete the Create Doc task
                task.State = TaskState.Completed;

                // Save changes
                await engine.UpdateTask(task);

                // Proceed to next handler
                if (_nextHandler != null)
                {
                    await _nextHandler.Handle(task, engine);
                }
            }
            else if (_nextHandler != null)
            {
                await _nextHandler.Handle(task, engine);
            }
        }
    }

}
