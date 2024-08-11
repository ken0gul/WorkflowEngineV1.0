using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Controllers
{

    [Route("/engine")]
    public class TaskController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public TaskController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var workflows = await _unitOfWork.Workflows.GetAllAsync();

            return View(workflows);
        }
    }
}
