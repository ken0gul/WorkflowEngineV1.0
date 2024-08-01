using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkflowEngineV1._0.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {

            var tasks = await _context.TaskItems.ToListAsync();
            Console.Write(tasks.Count.ToString());
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();

            task.X = updatedTask.X;
            task.Y = updatedTask.Y;

            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

     

        [HttpPost("connections")]
        public async Task<IActionResult> SaveConnection([FromBody] Connection connection)
        {
            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id, [FromQuery] int workflowId)
        {
            var workflow = await _context.Workflows
                
                .Include(W => W.Tasks)
                .FirstOrDefaultAsync(w => w.Id == workflowId);
               
            var taskFound = workflow?.Tasks.Find(t => t.Name == id);
            if (taskFound == null) return NotFound();

            _context.TaskItems.Remove(taskFound);
            await _context.SaveChangesAsync();

            return Ok("Task has been removed!");
        }
    }
}
