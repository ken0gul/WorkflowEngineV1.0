using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkflowEngineV1._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkflowController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("saveWorkflow")]
        public async Task<IActionResult> SaveWorkflow([FromBody] Workflow workflowDto)
        {
            if (workflowDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                    iconHTML = t.iconHTML
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
    }

 
}
