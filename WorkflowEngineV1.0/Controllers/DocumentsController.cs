using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Controllers
{

    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Documents/Create
        public async Task<IActionResult> Create()
        {
            var documents = await _context.Workflows.Include(d => d.Document).ToListAsync();
            ViewBag.Workflows = documents;
            return View();
        }

        // GET: Documents/Index
        public async Task<IActionResult> Index()
        {
            var documents = await _context.Documents.Include(d => d.Workflow).ToListAsync();
            var documentWorkflowViewModels = new List<DocumentWorkflowViewModel>();

            foreach (var document in documents)
            {
                var workflow = await _context.Workflows
                    .Include(w => w.Tasks)
                    .Include(w => w.Connections)
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
