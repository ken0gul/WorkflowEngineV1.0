using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Services
{
    public class WorkflowService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowService(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteWorkflow(int workflowId)
        {


            var workflow = await _unitOfWork.Workflows
                  .GetAll(w => w.Tasks, w => w.Connections)
                  .FirstOrDefaultAsync(w => w.Id == workflowId);


            // If workflow is null
            if (workflow == null)
            {
                throw new Exception("Workflow not found!");
            }

            foreach(var task in workflow.Tasks)
            {
                if (task.Type == TaskType.Start)
                {
                    task.State = TaskState.Working;
                    await _context.SaveChangesAsync();
                    await ExecuteTask(task);
                }
            }
            var anyUnfinishedTask = workflow.Tasks.Any(task => task.State == TaskState.Working);
            if (anyUnfinishedTask)
            {
                workflow.State = TaskState.Working;
            } else
            {

            workflow.State = TaskState.Completed;
            }

            await _unitOfWork.CompleteAsync();

        }

        public async Task ExecuteTask(TaskItem task)
        {
            // Simulate task execution
            await Task.Delay(1000);

            task.State = TaskState.Working;
            await _context.SaveChangesAsync();

            var connections = _context.Connections
                .Where(c => c.StartTaskId == task.Name)
                .ToList();

            foreach (var connection in connections)
            {
                var nextTask = _context.TaskItems.FirstOrDefault(t => t.Name == connection.EndTaskId);
                if (nextTask != null && nextTask.State == TaskState.Preparing)
                {
                    nextTask.State = TaskState.Working;
                    await _context.SaveChangesAsync();
                    await ExecuteTask(nextTask);
                }
            }
        }

    }
}
