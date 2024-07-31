using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class CreateDocTaskHandler : ITaskHandler
    {
        private ITaskHandler _nextHandler;
        public bool _shouldMoveToNextHandler { get; set; } = false;

        public void SetNext(ITaskHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        

        public async Task Handle(TaskItem task, WorkflowEngine engine, Workflow workflow)
        {
            if (task.Name == "Create Doc" && task.State == TaskState.Working)
            {
                // Complete the Create Doc task
                task.State = TaskState.Completed;

                // Save changes
                await engine.UpdateTask(task);

                foreach (var taskItem in workflow.Tasks)
                {
                    Console.WriteLine(taskItem.Name);
                    if (taskItem.Name == "Send E-mail")
                    {
                        Console.WriteLine("In the IF of CreateDocTaskHandler");
                        // Proceed to next handler
                        if (_nextHandler != null && _shouldMoveToNextHandler)
                        {
                            await _nextHandler.Handle(taskItem, engine, workflow);
                        }

                    }
                }
             
              
            }
            else if (_nextHandler != null)
            {
                await _nextHandler.Handle(task, engine, workflow);
            }
        }
    }

}
