using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;
using Contoso_MVC_8_0_VS2022.DAL;
using System.Data;

namespace Contoso_MVC_8_0_VS2022.Controllers
{
  public class StudentsAsyncController : Controller
  {
    private readonly SchoolContext _context;
    private IStudentRepositoryAsync studentRepositoryAsync;

    public StudentsAsyncController(IStudentRepositoryAsync studentRepositoryAsync, SchoolContext context)
    {
      this.studentRepositoryAsync = studentRepositoryAsync;
      this._context = context;
    }

    public async Task<IActionResult> Index(
      string sortOrder,
      string currentFilter,
      string searchString,
      int? pageNumber)
    {
      ViewData["CurrentSort"] = sortOrder;
      ViewData["NameSortParm"] =
          String.IsNullOrEmpty(sortOrder) ? "LastName_desc" : "";
      ViewData["DateSortParm"] =
          sortOrder == "EnrollmentDate" ? "EnrollmentDate_desc" : "EnrollmentDate";

      if (searchString != null)
      {
        pageNumber = 1;
      }
      else
      {
        searchString = currentFilter;
      }

      ViewData["CurrentFilter"] = searchString;

      var students = from s in await studentRepositoryAsync.GetStudents()
                        select s;
      
      if (!String.IsNullOrEmpty(searchString))
      {
        students = students.Where(s => s.LastName.Contains(searchString)
                               || s.FirstMidName.Contains(searchString));
      }

      if (string.IsNullOrEmpty(sortOrder))
      {
        sortOrder = "LastName";
      }

      bool descending = false;
      if (sortOrder.EndsWith("_desc"))
      {
        sortOrder = sortOrder.Substring(0, sortOrder.Length - 5);
        descending = true;
      }

      // Hvis der arbejdes på en IEnumeable liste af Students kommer der en tom liste ud ved order
      // metoderne herunder. Derfor vælges det at arbejde med en IQueryable liste fra Student Repository.
      //if (descending)
      //{
      //  students = students.OrderByDescending(e => EF.Property<object>(e, sortOrder));
      //}
      //else
      //{
      //  students = students.OrderBy(e => EF.Property<object>(e, sortOrder));
      //}

      int pageSize = 3;
      IQueryable<Student> studentsIQueryable = students.AsQueryable<Student>();
      return View(await PaginatedList<Student>.CreateAsync(studentsIQueryable, pageNumber ?? 1, pageSize));
    }
       
    public async Task<IActionResult> Details(int ?id)
    {
      if (id == null)
      {
        return NotFound();
      }

      Student student = await studentRepositoryAsync.GetStudentByID((int)id);
      if (student == null)
      {
        return NotFound();
      }

      return View(student);
    }

    // GET: Students/Create
    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
    {
      try
      {
        if (ModelState.IsValid)
        {
          await studentRepositoryAsync.InsertStudent(student);
          await studentRepositoryAsync.Save();
          return RedirectToAction(nameof(Index));
        }
      }
      catch (DbUpdateException /* ex */)
      {
        //Log the error (uncomment ex variable name and write a log.
        ModelState.AddModelError("", "Unable to save changes. " +
            "Try again, and if the problem persists " +
            "see your system administrator.");
      }
      return View(student);
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int ?id)
    {
      if (id == null)
      {
        return NotFound();
      }

      //var student = await _context.Students.FindAsync(id);
      Student student = await studentRepositoryAsync.GetStudentByID((int)id);
      if (student == null)
      {
        return NotFound();
      }
      return View(student);
    }
        
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
                    [Bind("LastName, FirstMidName, EnrollmentDate")]
                     Student student)
    {
      try
      {
        if (ModelState.IsValid)
        {
          await studentRepositoryAsync.UpdateStudent(student);
          await studentRepositoryAsync.Save();
          return RedirectToAction("Index");
        }
      }
      catch (DataException /* dex */)
      {
        //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
        ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
      }
      return View(student);
    }
    
    //
    // GET: /Student/Delete/5

    public async Task <IActionResult> Delete(bool? saveChangesError = false, int id = 0)
    {
      if (saveChangesError.GetValueOrDefault())
      {
        ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
      }
      Student student = await studentRepositoryAsync.GetStudentByID(id);
      return View(student);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      //var student = await _context.Students.FindAsync(id);
      //if (student == null)
      //{
      //  return RedirectToAction(nameof(Index));
      //}

      try
      {
        //_context.Students.Remove(student);
        //await _context.SaveChangesAsync();
        //return RedirectToAction(nameof(Index));
        Student student = await studentRepositoryAsync.GetStudentByID(id);
        await studentRepositoryAsync.DeleteStudent(id);
        await studentRepositoryAsync.Save();
      }
      catch (DbUpdateException /* ex */)
      {
        //Log the error (uncomment ex variable name and write a log.)
        return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
      }
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      studentRepositoryAsync.Dispose();
      base.Dispose(disposing);
    }
  }
}

