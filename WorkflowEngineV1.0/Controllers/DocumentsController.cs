using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Controllers
{

    public class DocumentsController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public DocumentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Documents/Create
        public async Task<IActionResult> Create()
        {
            var documents = await _unitOfWork.Workflows
                .GetAll(d => d.Document).ToListAsync();
            ViewBag.Workflows = documents;
            return View();
        }

        // GET: Documents/Index
        public async Task<IActionResult> Index()
        {
            var documents = await _unitOfWork.Documents.GetAll(d => d.Workflow).ToListAsync();
            var documentWorkflowViewModels = new List<DocumentWorkflowViewModel>();

            foreach (var document in documents)
            {
                var workflow = await _unitOfWork.Workflows
                    .GetAll(w => w.Tasks, workflow => workflow.Connections)
                    .FirstOrDefaultAsync(w => w.Id == document.WorkflowId);

                var documentWorkflowViewModel = new DocumentWorkflowViewModel
                {
                    Document = document,
                    Workflow = workflow
                };

                documentWorkflowViewModels.Add(documentWorkflowViewModel);
            }

            ViewBag.DocumentWorkflows = documentWorkflowViewModels;
            return View();
        }
    }
}
