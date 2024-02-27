using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public interface IStudentRepositoryAsync : IDisposable
  {
    Task<IEnumerable<Student>> GetStudents();
    Task<IQueryable<Student>> GetStudentsIQueryable();
    Task<Student> GetStudentByID(int studentId);
    Task InsertStudent(Student student);
    Task DeleteStudent(int studentID);
    Task UpdateStudent(Student student);
    Task Save();
  }
}
