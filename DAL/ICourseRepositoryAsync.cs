using Contoso_MVC_8_0_VS2022.Models;
using System.Linq.Expressions;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public interface ICourseRepositoryAsync : IGenericRepositoryAsync<Course>
  {
    Task<IEnumerable<Course>> GetAllCourses(Expression<Func<Course, bool>> filter = null,
                                            Func<IQueryable<Course>, IOrderedQueryable<Course>> orderBy = null,
                                            string includeProperties = "");
    Task<IEnumerable<Course>> GetAllInterestingCourses();
  }
}
