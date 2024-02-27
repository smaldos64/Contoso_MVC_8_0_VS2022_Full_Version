using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class UnitOfWorkAsync : IDisposable
  {
    // LTPE
    private SchoolContext context;
    //private SchoolContext context = new SchoolContext();
    private GenericRepositoryAsync<Department> departmentRepositoryAsync;
    private GenericRepositoryAsync<Course> courseRepositoryAsync;

    public UnitOfWorkAsync(SchoolContext context)
    {
      this.context = context;
    }

    public GenericRepositoryAsync<Department> DepartmentRepositoryAsync
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

    public GenericRepositoryAsync<Course> CourseRepository
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

    public void Save()
    {
      context.SaveChanges();
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
