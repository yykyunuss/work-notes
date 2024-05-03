
using System.Diagnostics.CodeAnalysis;
using Fctr.Edison.FileAdapter.Repositories;
using Fctr.Edison.FileAdapter.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Fctr.Edison.FileAdapter.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class EntityRepository<T> : IEntityRepository<T> where T : class, new()
    {
        private DocumentPoolAdapterContext _dbContext;
        private DbSet<T> _dbSet;

        public EntityRepository(DocumentPoolAdapterContext dbContext){
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual IQueryable<T> GetAllQueryable(){
            return _dbSet;
        }

        public virtual void Insert(T entity) {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
        }
        
    }
}
