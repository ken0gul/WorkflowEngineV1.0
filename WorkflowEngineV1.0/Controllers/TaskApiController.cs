using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Models;
using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;

namespace WorkflowEngineV1._0.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public TaskApiController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {

            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return NotFound();

            task.X = updatedTask.X;
            task.Y = updatedTask.Y;

            await _unitOfWork.TaskItems.UpdateAsync(task);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

     

        [HttpPost("connections")]
        public async Task<IActionResult> SaveConnection([FromBody] Connection connection)
        {
            _context.Connections.Add(connection);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id, [FromQuery] int workflowId)
        {
            var workflow = await _unitOfWork.Workflows
                
                .GetAll(W => W.Tasks)
                .FirstOrDefaultAsync(w => w.Id == workflowId);
               
            var taskFound = workflow?.Tasks.Find(t => t.Name == id);
            if (taskFound == null) return NotFound();

            _context.TaskItems.Remove(taskFound);
            await _unitOfWork.CompleteAsync();

            return Ok("Task has been removed!");
        }
    }
}
