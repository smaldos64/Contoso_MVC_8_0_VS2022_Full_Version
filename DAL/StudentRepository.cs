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

    // LTPE Dette er metoden fra guiden !!!
    public IEnumerable<Student> GetStudents()
    {
      return context.Students.ToList();
    }

    // LTPE
    public IQueryable<Student> GetStudentsIQueryable()
    {
      return context.Students;
    }

    public Student GetStudentByID(int id)
    {
      // LTPE
      //return context.Students.Find(id);
      return context.Students
      .Include(s => s.Enrollments)
        .ThenInclude(e => e.Course)
        .AsNoTracking()
        .FirstOrDefault(m => m.ID == id);
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
      // LTPE
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