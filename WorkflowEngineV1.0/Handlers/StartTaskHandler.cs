﻿using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{

    public class StartTaskHandler : ITaskHandler
    {
        private ITaskHandler _nextHandler;
        private bool _shouldMoveToNextHandler;

        public void SetNext(ITaskHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public async Task Handle(TaskItem task, WorkflowEngine engine, Workflow workflow)
        {

            Console.WriteLine("Handle() is running for StartTaskHandler)");
            if (task.Name == ConnName.Start && task.State == TaskState.Working)
            {
                // Complete the Start task
                task.State = TaskState.Completed;
               
                await engine.UpdateTask(task);
                // Proceed to the next handler
                foreach(var taskItem in workflow.Tasks)
                {
                    if (taskItem.Name == ConnName.CreateDoc)
                    {
                        if (_nextHandler != null)
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
