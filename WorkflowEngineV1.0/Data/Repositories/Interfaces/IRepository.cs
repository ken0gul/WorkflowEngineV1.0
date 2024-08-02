using System.Linq.Expressions;

namespace WorkflowEngineV1._0.Data.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {

        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
    }
}
