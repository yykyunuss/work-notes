
namespace Fctr.Edison.FileAdapter.DataAccess
{
    public interface IEntityRepository<T> where T : class, new()
    {
        IQueryable<T> GetAllQueryable();
        void Insert(T entity);

    }
}
