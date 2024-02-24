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
  public class StudentsController : Controller
  {
    private readonly SchoolContext _context;

    //public StudentsController(SchoolContext context)
    //{
    //  _context = context;
    //}

    private IStudentRepository studentRepository;

    //public StudentsController()
    //{
    //  this.studentRepository = new StudentRepository(new SchoolContext());
    //}

    public StudentsController(IStudentRepository studentRepository, SchoolContext context)
    {
      this.studentRepository = studentRepository;
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

      //var students = from s in _context.Students
      //               select s;
      var students = from s in studentRepository.GetStudents()
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
      if (descending)
      {
        students = students.OrderByDescending(e => EF.Property<object>(e, sortOrder));
      }
      else
      {
        students = students.OrderBy(e => EF.Property<object>(e, sortOrder));
      }

      int pageSize = 3;

      // Hvis GetStudents() sættes til at returnere IQueryable<Student> kan
      // AsNoTracking anvendes.
      return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(),
          pageNumber ?? 1, pageSize));

      // Hvis GetStudents() sættes til at returnere IEnumerable<Student> kan
      // AsNoTracking ikke anvendes og vi bliver nødt til at lave vores
      // IEnumerable om til en IQueryable !!!.
      //return View(await PaginatedList<Student>.CreateAsync(students as IQueryable<Student>,
      //    pageNumber ?? 1, pageSize));
    }

    // GET: Students/Details/5
    //public async Task<IActionResult> Details(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    //var student = await _context.Students
    //    //    .FirstOrDefaultAsync(m => m.ID == id);
    //    var student = await _context.Students
    //    .Include(s => s.Enrollments)
    //      .ThenInclude(e => e.Course)
    //      .AsNoTracking()
    //      .FirstOrDefaultAsync(m => m.ID == id);
    //    if (student == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(student);
    //}

    public async Task<IActionResult> Details(int id)
    {
      if (id == null)
      {
        return NotFound();
      }

      //var student = await _context.Students
      //    .FirstOrDefaultAsync(m => m.ID == id);
      //var student = await _context.Students
      //.Include(s => s.Enrollments)
      //  .ThenInclude(e => e.Course)
      //  .AsNoTracking()
      //  .FirstOrDefaultAsync(m => m.ID == id);
      Student student = studentRepository.GetStudentByID(id);
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

    // POST: Students/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("ID,LastName,FirstMidName,EnrollmentDate")] Student student)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _context.Add(student);
    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(student);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
    {
      try
      {
        if (ModelState.IsValid)
        {
          //_context.Add(student);
          //await _context.SaveChangesAsync();
          studentRepository.InsertStudent(student);
          studentRepository.Save();
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
    public async Task<IActionResult> Edit(int id)
    {
      if (id == null)
      {
        return NotFound();
      }

      //var student = await _context.Students.FindAsync(id);
      Student student = studentRepository.GetStudentByID(id);
      if (student == null)
      {
        return NotFound();
      }
      return View(student);
    }

    // POST: Students/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,EnrollmentDate")] Student student)
    //{
    //    if (id != student.ID)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(student);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!StudentExists(student.ID))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(student);
    //}

    //[HttpPost, ActionName("Edit")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> EditPost(int? id)
    //{
    //  if (id == null)
    //  {
    //    return NotFound();
    //  }
    //  var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
    //  if (await TryUpdateModelAsync<Student>(
    //      studentToUpdate,
    //      "",
    //      s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
    //  {
    //    try
    //    {
    //      await _context.SaveChangesAsync();
    //      return RedirectToAction(nameof(Index));
    //    }
    //    catch (DbUpdateException /* ex */)
    //    {
    //      //Log the error (uncomment ex variable name and write a log.)
    //      ModelState.AddModelError("", "Unable to save changes. " +
    //          "Try again, and if the problem persists, " +
    //          "see your system administrator.");
    //    }
    //  }
    //  return View(studentToUpdate);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(
         [Bind("LastName, FirstMidName, EnrollmentDate")]
         Student student)
    {
      try
      {
        if (ModelState.IsValid)
        {
          studentRepository.UpdateStudent(student);
          studentRepository.Save();
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


    // GET: Students/Delete/5
    //public async Task<IActionResult> Delete(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    var student = await _context.Students
    //        .FirstOrDefaultAsync(m => m.ID == id);
    //    if (student == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(student);
    //}

    //public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
    //{
    //  if (id == null)
    //  {
    //    return NotFound();
    //  }

    //  var student = await _context.Students
    //      .AsNoTracking()
    //      .FirstOrDefaultAsync(m => m.ID == id);
    //  if (student == null)
    //  {
    //    return NotFound();
    //  }

    //  if (saveChangesError.GetValueOrDefault())
    //  {
    //    ViewData["ErrorMessage"] =
    //        "Delete failed. Try again, and if the problem persists " +
    //        "see your system administrator.";
    //  }

    //  return View(student);
    //}

    //
    // GET: /Student/Delete/5

    public ActionResult Delete(bool? saveChangesError = false, int id = 0)
    {
      if (saveChangesError.GetValueOrDefault())
      {
        ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
      }
      Student student = studentRepository.GetStudentByID(id);
      return View(student);
    }

    // POST: Students/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int id)
    //{
    //    var student = await _context.Students.FindAsync(id);
    //    if (student != null)
    //    {
    //        _context.Students.Remove(student);
    //    }

    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

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
        Student student = studentRepository.GetStudentByID(id);
        studentRepository.DeleteStudent(id);
        studentRepository.Save();
      }
      catch (DbUpdateException /* ex */)
      {
        //Log the error (uncomment ex variable name and write a log.)
        return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
      }
      return RedirectToAction("Index");
    }

    //private bool StudentExists(int id)
    //{
    //  return _context.Students.Any(e => e.ID == id);
    //}

    protected override void Dispose(bool disposing)
    {
      studentRepository.Dispose();
      base.Dispose(disposing);
    }
  }
}
