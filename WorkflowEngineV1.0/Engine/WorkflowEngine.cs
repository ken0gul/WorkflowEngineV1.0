using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Handlers;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Engine
{
    public class WorkflowEngine
    {
        private readonly ApplicationDbContext _context;
        private TaskHandler _firstHandler;

        public WorkflowEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SetFirstHandler(TaskHandler firstHandler)
        {

            _firstHandler = firstHandler;
        }

        public async Task StartWorkflow(int workflowId, Document document)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Tasks)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

        

            // if workflow does not exist then throw it
            if (workflow == null) throw new Exception("Workflow not found");


            workflow.DocumentId = document.Id;
            workflow.Document = document;

            // Save changes to the database
            _context.Workflows.Update(workflow);
            await _context.SaveChangesAsync();

            // get the start task
            var startTask = workflow.Tasks.FirstOrDefault(t => t.Name.Equals("Start"));

            if (startTask != null)
            {
                await _firstHandler.Handle(startTask, this);
            }

        }
        public async Task UpdateTaskState(TaskItem taskItem)
        {
            _context.TaskItems.Update(taskItem);
            await _context.SaveChangesAsync();
        }

        public TaskItem GetNextTask(TaskItem taskItem)
        {
            var connection = _context.Connections.FirstOrDefault(c => c.StartTaskId == taskItem.Name);
            return connection != null ? _context.TaskItems.Find(connection.EndTaskId) : null;
        }

        public async Task SendEmail()
        {
            Console.WriteLine("E-Mail is being sent");
            await Task.Delay(1500);
            Console.WriteLine("Email is sent");
        }

        public void CompleteWorkflow(int workflowId)
        {
            var workflow = _context.Workflows.Find(workflowId);
            if (workflow != null)
            {
                workflow.State = TaskState.Completed;
                _context.Workflows.Update(workflow);
                _context.SaveChanges();
            }
        }
    }
}
