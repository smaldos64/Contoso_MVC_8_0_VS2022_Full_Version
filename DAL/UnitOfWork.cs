using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class UnitOfWork : IDisposable
  {
    // LTPE
    private SchoolContext context;
    //private SchoolContext context = new SchoolContext();
    private GenericRepository<Department> departmentRepository;
    private GenericRepository<Course> courseRepository;

    public UnitOfWork(SchoolContext context)
    {
      this.context = context;
    }

    public GenericRepository<Department> DepartmentRepository
    {
      get
      {

        if (this.departmentRepository == null)
        {
          this.departmentRepository = new GenericRepository<Department>(context);
        }
        return departmentRepository;
      }
    }

    public GenericRepository<Course> CourseRepository
    {
      get
      {

        if (this.courseRepository == null)
        {
          this.courseRepository = new GenericRepository<Course>(context);
        }
        return courseRepository;
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
