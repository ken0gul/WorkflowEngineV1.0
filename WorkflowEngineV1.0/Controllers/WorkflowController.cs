using Microsoft.AspNetCore.Mvc;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Models;
using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Services;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;

namespace WorkflowEngineV1._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;

        private readonly IUnitOfWork _unitOfWork;

        private readonly WorkflowService _workflowService;

        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(WorkflowService workflowService, IUnitOfWork unitOfWork, ILogger<WorkflowController> logger)
        {
            _workflowService = workflowService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        [HttpPost("saveWorkflow")]
        public async Task<IActionResult> SaveWorkflow([FromBody] Workflow workflowDto)
        {
            if (workflowDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Workflow foundWorkflow = await _unitOfWork.Workflows
                .GetAll(w => w.Tasks, w => w.Connections)
                .FirstOrDefaultAsync(w => w.WorkflowName == workflowDto.WorkflowName);

            //Let's reset HasProblem flag to make it re-work
            if (foundWorkflow != null)
            {

                foundWorkflow.HasProblem = false;
            }
            // If tasks are deleted and workflow is empty..
            if (foundWorkflow?.Tasks.Count() == 0)
            {
                foundWorkflow.Connections.Clear();
                foundWorkflow.Tasks.Clear();
                foundWorkflow.State = TaskState.Preparing;
                await _unitOfWork.Workflows.UpdateAsync(foundWorkflow);
                await _unitOfWork.CompleteAsync();

                var foundDoc = await _unitOfWork.Documents.GetFirstOrDefaultAsync(d => d.WorkflowId == foundWorkflow.Id);
                if (foundDoc != null)
                {
                  await _unitOfWork.Documents.DeleteAsync(foundDoc);
                await _unitOfWork.CompleteAsync();

                }
            }
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
                        _ => TaskState.Preparing
                    }
                }).ToList();

                foundWorkflow.Connections = workflowDto.Connections.Select(c => new Connection
                {
                    StartTaskId = c.StartTaskId,
                    EndTaskId = c.EndTaskId,
                    XLoc = c.XLoc,
                    YLoc = c.YLoc
                }).ToList();

                await _unitOfWork.Workflows.UpdateAsync(foundWorkflow);
            }
            else
            {
                // Create a new workflow
                var workflow = new Workflow
                {
                    WorkflowName = workflowDto.WorkflowName,
                    Caption = workflowDto.Caption,
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

                await _unitOfWork.Workflows.UpdateAsync(workflow);
            }

            await _unitOfWork.CompleteAsync();
            return Ok();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkflow(int id)
        {

            _logger.LogInformation($"/// Workflow with id: {id} is being fetched! ///");
            var workflow = await _unitOfWork.Workflows
                            .GetAll(w => w.Tasks,  w => w.Connections)
                            .FirstOrDefaultAsync(w =>  w.Id == id); 

            if (workflow == null)
            {
                return NotFound();
            }

            return Ok(workflow);
        }

        //[HttpPost("start/{workflowId}")]
        //public async Task<IActionResult> StartWorkflow(int workflowId)
        //{
        //    _logger.LogInformation($"/// StartWorkFlow Action has been called with `start/workflowId` with id {workflowId} ///");

        //    try
        //    {
        //        await _workflowService.ExecuteWorkflow(workflowId);
        //        return Ok();
        //    }catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("getStates/{workflowId}")]
        public async Task<IActionResult> GetTaskState(int workflowId)
        {
            var workflow = await _unitOfWork.Workflows
                     .GetAll(w => w.Tasks)
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
            var workflows = await _unitOfWork.Workflows.GetAllAsync();
            if (workflows == null)
            {
                return NotFound();
            }
            return Ok(workflows);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflow(int id)
        {
            var workflow = await _unitOfWork.Workflows
                          .GetAll(w => w.Tasks, w => w.Connections)
                          .FirstOrDefaultAsync(w => w.Id == id); // Can we use GetFirstOrDefaultAsync here??


            if (workflow == null)
            {
                return NotFound();
            }

            else
            {
                await _unitOfWork.Workflows.DeleteAsync(workflow);
                await _unitOfWork.CompleteAsync();
            }
            return Ok("Workflow has been deleted!");
        }
    }

 
}
