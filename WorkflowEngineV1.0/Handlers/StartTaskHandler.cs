using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{

    public class StartTaskHandler : ITaskHandler
    {
        private ITaskHandler _nextHandler;

        public void SetNext(ITaskHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public async Task Handle(TaskItem task, WorkflowEngine engine)
        {

            Console.WriteLine("Handle() is running for StartTaskHandler)");
            if (task.Name == "Start" && task.State == TaskState.Working)
            {
                // Complete the Start task
                task.State = TaskState.Completed;
                await engine.UpdateTask(task);
                // Proceed to the next handler
                Console.WriteLine("The NextHandler in StartTaskHandler: " + _nextHandler.ToString());

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
