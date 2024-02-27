using Contoso_MVC_8_0_VS2022.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity> where TEntity : class
  {
    internal SchoolContext context;
    internal DbSet<TEntity> dbSet;

    public GenericRepositoryAsync(SchoolContext context)
    {
      this.context = context;
      this.dbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
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
        return await orderBy(query).ToListAsync();
      }
      else
      {
        return await query.ToListAsync();
      }
    }

    public virtual async Task<TEntity> GetByID(object id)
    {
      return await dbSet.FindAsync(id);
    }

    // LTPE
    public virtual async Task<TEntity> GetByFilter(Expression<Func<TEntity, bool>> filter,
                                       string includeProperties = "")
    {
      IQueryable<TEntity> query = dbSet;

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }

      return await query.SingleOrDefaultAsync(filter);
    }

    public virtual async Task Insert(TEntity entity)
    {
      await dbSet.AddAsync(entity);
    }

    public virtual async Task Delete(object id)
    {
      TEntity entityToDelete = await dbSet.FindAsync(id);
      await Delete(entityToDelete);
    }

    public virtual async Task Delete(TEntity entityToDelete)
    {
      if (context.Entry(entityToDelete).State == EntityState.Detached)
      {
        dbSet.Attach(entityToDelete);
      }
      dbSet.Remove(entityToDelete);
    }

    public virtual async Task Update(TEntity entityToUpdate)
    {
      dbSet.Attach(entityToUpdate);
      context.Entry(entityToUpdate).State = EntityState.Modified;
    }
  }
}

