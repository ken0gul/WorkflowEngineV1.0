using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class SendEmailTaskHandler : ITaskHandler
    {
        private ITaskHandler _nextHandler;

        public void SetNext(ITaskHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public async Task Handle(TaskItem task, WorkflowEngine engine)
        {
            Console.WriteLine("Handle() is running for SendEmailTaskHandler)");

            if (task.Name == "Send E-mail" && task.State == TaskState.Working)
            {
                // Simulate sending email
                Console.WriteLine("E-Mail is being sent");
                await Task.Delay(1500); // Simulate delay
                Console.WriteLine("Email is sent");

                // Complete the Send E-mail task
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
                Console.WriteLine("_nextHandler is null in SendEmailTaskHandler.cs");

                await _nextHandler.Handle(task, engine);
            }
        }
    }

}
