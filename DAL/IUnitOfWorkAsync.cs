using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public interface IUnitOfWorkAsync
  {
    IGenericRepositoryAsync<Department> DepartmentRepositoryAsync { get;}

    IGenericRepositoryAsync<Course> CourseRepository { get; }

    IGenericRepositoryAsync<Student> StudentRepository { get;}

    public ICourseRepositoryAsync CourseRepositoryOwnAsync { get;}

    void Save();
  }
}
