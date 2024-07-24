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

        //[HttpGet("connections")]
        //public async Task<IActionResult> GetConnections()
        //{
        //    var connections = await _context.Connections.Include(c => c.StartTask).Include(c => c.EndTask).ToListAsync();
        //    return Ok(connections);
        //}

        [HttpPost("connections")]
        public async Task<IActionResult> SaveConnection([FromBody] Connection connection)
        {
            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
