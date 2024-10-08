﻿using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Handlers
{
    public class FinishTaskHandler : ITaskHandler
    {
        private ITaskHandler _nextHandler;
        public bool isDone { get; set; } = false; 

        public void SetNext(ITaskHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public async Task Handle(TaskItem task, WorkflowEngine engine, Workflow wf)
        {
            if (task.Name == ConnName.Finish && task.State == TaskState.Working && isDone)
            {
                Console.WriteLine("In the finishTaskHandler....");
                // Complete the Finish task
                task.State = TaskState.Completed;

                // Save changes
                await engine.UpdateTask(task);

                // Update workflow state
                var workflow = await engine.GetWorkflowByTask(task.Id.Value);
                workflow.State = TaskState.Completed;

                // Complete all tasks
                foreach (var t in workflow.Tasks)
                {
                    t.State = TaskState.Completed;
                    await engine.UpdateTask(t);
                }

                // Save changes
                await engine.UpdateWorkflow(workflow);
            }
            else if (_nextHandler != null)
            {
                await _nextHandler.Handle(task, engine, wf);
            }
        }
    }

}
