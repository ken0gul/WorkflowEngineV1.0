using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;
using WorkflowEngineV1._0.Handlers;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Engine
{
    public class WorkflowEngine
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private ITaskHandler _firstHandler;


        public WorkflowEngine(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SetFirstHandler(ITaskHandler handler)
        {
            _firstHandler = handler;
        }

        public async Task StartWorkflow(int workflowId, Document document, bool? shouldMove, bool? isDone)
        {
            // Find the workflow along with its tasks and connections
            var workflow = await _unitOfWork.Workflows
                .GetAll(w => w.Tasks, wc => wc.Connections)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            // Let's handle our exceptions gracefully :)
            if (workflow == null) throw new Exception("Workflow not found");

            // Set workflow to preparing state
            workflow.State = TaskState.Preparing;
            // and tasks to preparing state
            foreach (var task in workflow.Tasks)
            {
                task.State = TaskState.Working;
            }

            // Update the document reference
            workflow.DocumentId = document.Id;
            workflow.Document = document;

            await _unitOfWork.Workflows.UpdateAsync(workflow);
            await _unitOfWork.CompleteAsync();

            // Initialize handlers
            var startHandler = new StartTaskHandler();
            var createDocHandler = new CreateDocTaskHandler();
            var sendEmailHandler = new SendEmailTaskHandler();
            var finishHandler = new FinishTaskHandler();

            workflow.Connections.ForEach(conn =>
            {

                switch (conn.StartTaskId)
                {
                    case "Start":
                        startHandler.SetNext(createDocHandler);
                        break;

                    case "Create Doc":

                        createDocHandler.SetNext(sendEmailHandler);
                        break;

                    case "Send E-mail":
                        sendEmailHandler.SetNext(finishHandler);
                        break;

                    // Optionally, handle cases where conn.StartTaskId doesn't match any of the above
                    default:
                        // Handle unknown or default case if necessary
                        workflow.HasProblem = true;
                        workflow.ProblemTaskId = conn.StartTaskId;
                       
                        break;
                        //throw new ArgumentException($"Unexpected StartTaskId: {conn.StartTaskId}");
                }
            });
            //startHandler.SetNext(createDocHandler);
            //createDocHandler.SetNext(sendEmailHandler);
            //sendEmailHandler.SetNext(finishHandler);
            await _unitOfWork.CompleteAsync();
            // State
            createDocHandler._shouldMoveToNextHandler = shouldMove.Value;
            sendEmailHandler._shouldMoveToNextHandler = shouldMove.Value;
            finishHandler.isDone = isDone.Value;

            SetFirstHandler(startHandler);

            

            // Start processing the workflow
            await ProcessWorkflow(workflow);
        }

        private async Task ProcessWorkflow(Workflow workflow)
        {
            var startTask = workflow.Tasks.FirstOrDefault(t => t.Name.Equals("Start"));

            if (startTask != null)
            {
                await _firstHandler.Handle(startTask, this, workflow);
            }
        }
        public async Task UpdateTask(TaskItem task)
        {
            await _unitOfWork.TaskItems.UpdateAsync(task);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateWorkflow(Workflow workflow)
        {
            await _unitOfWork.Workflows.UpdateAsync(workflow);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<Workflow> GetWorkflowByTask(int taskId)
        {
            var task = await _unitOfWork.TaskItems
                .GetAll(t => t.Workflow)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            return task?.Workflow;
        }
    }


}
