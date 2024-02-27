using System;
using System.Collections.Generic;
using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.DAL
{
  public interface IStudentRepository : IDisposable
  {
    IEnumerable<Student> GetStudents();
    IQueryable<Student> GetStudentsIQueryable();
    Student GetStudentByID(int studentId);
    void InsertStudent(Student student);
    void DeleteStudent(int studentID);
    void UpdateStudent(Student student);
    void Save();
  }
}