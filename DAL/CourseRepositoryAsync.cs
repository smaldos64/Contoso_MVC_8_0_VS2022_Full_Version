using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;
using System.Linq.Expressions;

namespace Contoso_MVC_8_0_VS2022.DAL
{
    public class CourseRepositoryAsync : GenericRepositoryAsync<Course>, ICourseRepositoryAsync
    {
        public CourseRepositoryAsync(SchoolContext context) : base(context)
        {
            if (null == context)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
        public async Task<IEnumerable<Course>> GetAllCourses(Expression<Func<Course, bool>> filter = null,
                                                             Func<IQueryable<Course>, IOrderedQueryable<Course>> orderBy = null,
                                                             string includeProperties = "")
        {
            return await base.Get(filter, orderBy, includeProperties);
        }

        public async Task<IEnumerable<Course>> GetAllInterestingCourses()
        {
            return await base.Get();
        }
    }
}
