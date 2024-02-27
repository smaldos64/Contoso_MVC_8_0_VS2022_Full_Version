using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;
using Microsoft.EntityFrameworkCore;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class StudentRepositoryAsync : IStudentRepositoryAsync, IDisposable
  {
    private SchoolContext context;
    internal DbSet<Student> dbSet;

    public StudentRepositoryAsync(SchoolContext context)
    {
      this.context = context;
      this.dbSet = this.context.Set<Student>();
    }

    // LTPE Dette er metoden fra guiden !!!
    public async Task<IEnumerable<Student>> GetStudents()
    {
      return await context.Students.ToListAsync();
    }

    public async Task<IQueryable<Student>> GetStudentsIQueryable()
    {
      IQueryable<Student> Query = dbSet;
      // LTPE
      return context.Students;
    }

    public async Task<Student> GetStudentByID(int id)
    {
      // LTPE
      //return context.Students.Find(id);
      return await context.Students
      .Include(s => s.Enrollments)
        .ThenInclude(e => e.Course)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.ID == id);
    }

    public async Task InsertStudent(Student student)
    {
      await context.Students.AddAsync(student);
    }

    public async Task DeleteStudent(int studentID)
    {
      Student student = await context.Students.FindAsync(studentID);
      context.Students.Remove(student);
    }

    public async Task UpdateStudent(Student student)
    {
      context.Entry(student).State = EntityState.Modified;
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

