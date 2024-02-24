using Contoso_MVC_8_0_VS2022.DAL;
using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;
using Microsoft.EntityFrameworkCore;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public class StudentRepository : IStudentRepository, IDisposable
  {
    private SchoolContext context;

    public StudentRepository(SchoolContext context)
    {
      this.context = context;
    }

    //public IEnumerable<Student> GetStudents()
    //{
    //  return context.Students.ToList();
    //}

    public IQueryable<Student> GetStudents()
    {
      //return context.Students.ToList();
      // LTPE
      return context.Students;
    }

    public Student GetStudentByID(int id)
    {
      //return context.Students.Find(id);
      return context.Students
      .Include(s => s.Enrollments)
        .ThenInclude(e => e.Course)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.ID == id);
    }

    public void InsertStudent(Student student)
    {
      context.Students.Add(student);
    }

    public void DeleteStudent(int studentID)
    {
      Student student = context.Students.Find(studentID);
      context.Students.Remove(student);
    }

    public void UpdateStudent(Student student)
    {
      context.Entry(student).State = EntityState.Modified;
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