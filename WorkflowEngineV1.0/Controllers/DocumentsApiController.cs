﻿using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Models;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngineV1._0.Engine;

namespace WorkflowEngineV1._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly WorkflowEngine _workflowEngine;

        public DocumentsApiController(ApplicationDbContext context, WorkflowEngine workflowEngine)
        {
            _context = context;
            _workflowEngine = workflowEngine;
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

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                // Start the workflow
                await _workflowEngine.StartWorkflow(document.WorkflowId, document);
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

        [HttpPost("approveDocument/{documentId}")]
        public async Task<IActionResult> ApproveDocument(Guid documentId)
        {

            Console.WriteLine("");
            var document = await _context.Documents
                .Include(d => d.Workflow)
                .ThenInclude(w => w.Tasks)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                return NotFound("Document not found.");
            }

            var workflow = document.Workflow;

            //Update task states for approval
            foreach (var task in workflow.Tasks)
                {
                    if (task.Name == "Start" || task.Name == "Create Doc")
                    {
                        task.State = TaskState.Completed;
                    }
                    else if (task.Name == "Send E-mail")
                    {
                        task.State = TaskState.Completed;
                    }
                    else if (task.Name == "Finish")
                    {
                        task.State = TaskState.Working;
                    }
                }

            workflow.State = TaskState.Working;

            await _workflowEngine.UpdateWorkflow(workflow);

            foreach (var task in workflow.Tasks)
            {
                await _workflowEngine.UpdateTask(task);
            }


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

            // Update task states for publishing
            foreach (var task in workflow.Tasks)
            {
                task.State = TaskState.Completed;
            }

            workflow.State = TaskState.Completed;

            await _workflowEngine.UpdateWorkflow(workflow);

            foreach (var task in workflow.Tasks)
            {
                await _workflowEngine.UpdateTask(task);
            }

            return Ok(document);
        }

    }
}