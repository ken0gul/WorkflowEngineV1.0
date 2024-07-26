using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Models;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Services;
using Microsoft.EntityFrameworkCore;

namespace WorkflowEngineV1._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsApiController : ControllerBase
    {
         private readonly ApplicationDbContext _context;
        private readonly WorkflowService _workflowService;
        public DocumentsApiController(ApplicationDbContext context, WorkflowService workflowService)
        {
            _context = context;
            _workflowService = workflowService;
        }

        [HttpPost("createDocument")]
        public async Task<ActionResult<Document>> PostDocument(Document document)
        {
            if (document.WorkflowId > 0)
            {
                var workflow = await _context.Workflows.FindAsync(document.WorkflowId);
                if (workflow == null)
                {
                    return BadRequest("Invalid WorkflowId.");
                }
                document.Id = Guid.NewGuid();
                document.CreatedDate = DateTime.UtcNow;
                document.UpdatedDate = DateTime.UtcNow;
                document.Workflow = workflow;
            }

            

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Start the workflow
            if (document.WorkflowId > 0)
            {
                await StartWorkflow(document);
            }

            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
        }

        // GET: api/DocumentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }
        private async Task StartWorkflow(Document document)
        {
            // Fetch the existing workflow with tasks and connections
            var workflow = await _context.Workflows
                .Include(w => w.Tasks)
                .Include(w => w.Connections) // Include connections
                .FirstOrDefaultAsync(w => w.Id == document.WorkflowId);

            if (workflow != null)
            {
                // Update the workflow state
                workflow.State = TaskState.Preparing;

                // Update the document reference (if needed)
                workflow.DocumentId = document.Id;
                workflow.Document = document;
                
                // Set all the tasks to preparing in the beginning
                foreach (var item in workflow.Tasks)
                {
                    item.State = TaskState.Preparing;
                
                }
                // Update the state of each task
                foreach (var task in workflow.Tasks)
                {
                    if (task.Name == "Start" || task.Name == "Create Doc")
                    {

                    task.State = TaskState.Preparing;
                    }
                }

                // Save changes to the context
                 _context.Workflows.Update(workflow);
                await _context.SaveChangesAsync();

                // Optionally, you could start processing the workflow asynchronously
                // ProcessWorkflow(workflow.Id);
            }
        }

        [HttpPost("approveDocument/{documentId}")]
        public async Task<IActionResult> ApproveDocument(Guid documentId)
        {
            var document = await _context.Documents
                .Include(d => d.Workflow)
                .ThenInclude(w => w.Tasks)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                return NotFound("Document not found.");
            }

            var workflow = document.Workflow;
            workflow.State = TaskState.Working;

            // Update the tasks' states
            foreach (var task in workflow.Tasks)
            {
                if (task.Name == "Start" || task.Name == "Create Doc")
                {
                    task.State = TaskState.Completed;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(document);
        }
        [HttpPost("publishDocument/{documentId}")]
        public async Task<IActionResult> PublishDocument(Guid documentId)
        {
            var document = await _context.Documents
                .Include(d => d.Workflow)
                .ThenInclude(w => w.Tasks)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                return NotFound("Document not found.");
            }

            var workflow = document.Workflow;
            var finishTask = workflow.Tasks.FirstOrDefault(t => t.Name == "Finish");

            if (finishTask != null)
            {
                finishTask.State = TaskState.Completed;
            }

            workflow.State = TaskState.Completed;

            await _context.SaveChangesAsync();
            return Ok(document);
        }


        private void ProcessWorkflow(Guid workflowInstanceId)
        {
            // Implement the logic to process the workflow tasks asynchronously
            // Update the state of each task and the workflow accordingly
        }
    }
}
