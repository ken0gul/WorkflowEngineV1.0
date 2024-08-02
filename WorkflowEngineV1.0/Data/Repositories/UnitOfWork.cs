using WorkflowEngineV1._0.Data.Repositories.Interfaces;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRepository<TaskItem> TaskItems { get; private set; }
        public IRepository<Connection> Connections { get; private set; }
        public IRepository<Workflow> Workflows { get; private set; }
        public IRepository<Document> Documents { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            TaskItems = new Repository<TaskItem>(_context);
            Connections = new Repository<Connection>(_context);
            Workflows = new Repository<Workflow>(_context);
            Documents = new Repository<Document>(_context);
        }



        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
