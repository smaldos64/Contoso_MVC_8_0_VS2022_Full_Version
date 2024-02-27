using Contoso_MVC_8_0_VS2022.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class GenericRepository<TEntity> where TEntity : class
  {
    internal SchoolContext context;
    internal DbSet<TEntity> dbSet;

    public GenericRepository(SchoolContext context)
    {
      this.context = context;
      this.dbSet = context.Set<TEntity>();
    }

    public virtual IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "",
        int Id = 0)
    {
      IQueryable<TEntity> query = dbSet;

      if (filter != null)
      {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }

      if (orderBy != null)
      {
        return orderBy(query).ToList();
      }
      else
      {
        return query.ToList();
      }
    }

    public virtual TEntity GetByID(object id)
    {
      return dbSet.Find(id);
    }

    // LTPE
    public virtual TEntity GetByFilter(Expression<Func<TEntity, bool>> filter,
                                       string includeProperties = "")
    {
      IQueryable<TEntity> query = dbSet;

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }

      return query.SingleOrDefault(filter);
    }

    public virtual void Insert(TEntity entity)
    {
      dbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
      TEntity entityToDelete = dbSet.Find(id);
      Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
      if (context.Entry(entityToDelete).State == EntityState.Detached)
      {
        dbSet.Attach(entityToDelete);
      }
      dbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
      dbSet.Attach(entityToUpdate);
      context.Entry(entityToUpdate).State = EntityState.Modified;
    }
  }
}
