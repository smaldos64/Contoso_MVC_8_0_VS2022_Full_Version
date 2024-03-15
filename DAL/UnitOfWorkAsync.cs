using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class UnitOfWorkAsync : IUnitOfWorkAsync, IDisposable
  {
    // LTPE
    private SchoolContext context;
    //private SchoolContext context = new SchoolContext();
    private IGenericRepositoryAsync<Department> departmentRepositoryAsync;
    private IGenericRepositoryAsync<Course> courseRepositoryAsync;
    private IGenericRepositoryAsync<Student> studentRepositoryAsync;

    private ICourseRepositoryAsync courseRepositoryOwnAsync;

    public UnitOfWorkAsync(SchoolContext context)
    {
      this.context = context;
    }

    public IGenericRepositoryAsync<Department> DepartmentRepositoryAsync
    {
      get
      {
        if (this.departmentRepositoryAsync == null)
        {
          this.departmentRepositoryAsync = new GenericRepositoryAsync<Department>(context);
        }
        return departmentRepositoryAsync;
      }
    }

    public IGenericRepositoryAsync<Course> CourseRepository
    {
      get
      {
        if (this.courseRepositoryAsync == null)
        {
          this.courseRepositoryAsync = new GenericRepositoryAsync<Course>(context);
        }
        return courseRepositoryAsync;
      }
    }

    public IGenericRepositoryAsync<Student> StudentRepository
    {
      get
      {
        if (this.studentRepositoryAsync == null)
        {
          this.studentRepositoryAsync = new GenericRepositoryAsync<Student>(context);
        }
        return studentRepositoryAsync;
      }
    }

    public ICourseRepositoryAsync CourseRepositoryOwnAsync 
    {
      get
      {
        if (null == courseRepositoryOwnAsync)
        {
          courseRepositoryOwnAsync = new CourseRepositoryAsync(context);
        }

        return (courseRepositoryOwnAsync);
      }
    }

    public async Task Save()
    {
      await context.SaveChangesAsync();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          context.Dispose();
        }
      }
      this.disposed = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
