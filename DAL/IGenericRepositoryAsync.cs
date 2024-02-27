using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public interface IGenericRepositoryAsync<TEntity> where TEntity : class
  {
    Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    Task<TEntity> GetByID(object id);

    Task<TEntity> GetByFilter(Expression<Func<TEntity, bool>> filter,
                                       string includeProperties = "");

    Task Insert(TEntity entity);
    Task Delete(object id);
    Task Delete(TEntity entityToDelete);
    Task Update(TEntity entityToUpdate);
  }
}
