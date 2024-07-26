using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Models;
using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Services;

namespace WorkflowEngineV1._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly WorkflowService _workflowService;

        public WorkflowController(ApplicationDbContext context, WorkflowService workflowService)
        {
            _context = context;
            _workflowService = workflowService;
        }
        [HttpPost("saveWorkflow")]
        public async Task<IActionResult> SaveWorkflow([FromBody] Workflow workflowDto)
        {
            if (workflowDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Console.WriteLine(workflowDto);
            Workflow foundWorkflow = await _context.Workflows
                .Include(w => w.Tasks)
                .Include(w => w.Connections)
                .FirstOrDefaultAsync(w => w.WorkflowName == workflowDto.WorkflowName);

            if (foundWorkflow != null)
            {
                // Update existing workflow
                foundWorkflow.Connections.Clear();
                foundWorkflow.Tasks.Clear();

                foundWorkflow.Tasks = workflowDto.Tasks.Select(t => new TaskItem
                {
                    Id = t.Id, // Make sure to set Id properly if you're reusing TaskItem objects
                    Name = t.Name,
                    X = t.X,
                    Y = t.Y,
                    iconHTML = t.iconHTML,
                    StateDTO = t.StateDTO,
                    State = t.StateDTO switch
                    {
                        "Preparing" => TaskState.Preparing,
                        "Working" => TaskState.Working,
                        "Completed" => TaskState.Completed,
                        _ => throw new ArgumentException($"Invalid state: {t.StateDTO}")
                    }
                }).ToList();

                foundWorkflow.Connections = workflowDto.Connections.Select(c => new Connection
                {
                    StartTaskId = c.StartTaskId,
                    EndTaskId = c.EndTaskId,
                    XLoc = c.XLoc,
                    YLoc = c.YLoc
                }).ToList();

                _context.Workflows.Update(foundWorkflow);
            }
            else
            {
                // Create a new workflow
                var workflow = new Workflow
                {
                    WorkflowName = workflowDto.WorkflowName,
                    Tasks = workflowDto.Tasks.Select(t => new TaskItem
                    {
                        Id = t.Id,
                        Name = t.Name,
                        X = t.X,
                        Y = t.Y,
                        iconHTML = t.iconHTML
                    }).ToList(),
                    Connections = workflowDto.Connections.Select(c => new Connection
                    {
                        StartTaskId = c.StartTaskId,
                        EndTaskId = c.EndTaskId,
                        XLoc = c.XLoc,
                        YLoc = c.YLoc,

                    }).ToList()
                };

                _context.Workflows.Add(workflow);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflow(int id)
        {
            Console.WriteLine(id);
            var workflow = await _context.Workflows
                .Include(w => w.Tasks)
                .Include(w => w.Connections)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow == null)
            {
                return NotFound();
            }

            return Ok(workflow);
        }

        [HttpPost("start/{workflowId}")]
        public async Task<IActionResult> StartWorkflow(int workflowId)
        {
            try
            {
                await _workflowService.ExecuteWorkflow(workflowId);
                return Ok();
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getStates/{workflowId}")]
        public async Task<IActionResult> GetTaskState(int workflowId)
        {
            var workflow = await _context.Workflows
                     .Include(w => w.Tasks)
                     .FirstOrDefaultAsync(w => w.Id == workflowId);
            if (workflow == null)
                return NotFound();

            var taskStates = workflow.Tasks.Select(t => new
            {
                t.Id,
                State = t.State.ToString(),
                t.Name
            }).ToList();

            return Ok(taskStates);
        }


        [HttpGet("getWorkflowStates")]
        public async Task<IActionResult> GetWorkflowStates()
        {
            var workflows = await _context.Workflows
             .ToListAsync();
            if (workflows == null)
            {
                return NotFound();
            }
            return Ok(workflows);
        }
    }

 
}
