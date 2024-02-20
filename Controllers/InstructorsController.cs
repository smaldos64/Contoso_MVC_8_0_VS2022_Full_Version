using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;

using Contoso_MVC_8_0_VS2022.Models.SchoolViewModels;

namespace Contoso_MVC_8_0_VS2022.Controllers
{
  public class InstructorsController : Controller
  {
    private readonly SchoolContext _context;

    public InstructorsController(SchoolContext context)
    {
      _context = context;
    }

    // Eager Loading
    // GET: Instructors
    //public async Task<IActionResult> Index(int? id, int? courseID)
    //{
    //    //return View(await _context.Instructors.ToListAsync());

    //    var viewModel = new InstructorIndexData();
    //    viewModel.Instructors = await _context.Instructors
    //          .Include(i => i.OfficeAssignment)
    //          .Include(i => i.CourseAssignments)
    //            .ThenInclude(i => i.Course)
    //                .ThenInclude(i => i.Enrollments)
    //                    .ThenInclude(i => i.Student)
    //          .Include(i => i.CourseAssignments)
    //            .ThenInclude(i => i.Course)
    //                .ThenInclude(i => i.Department)
    //          .AsNoTracking()
    //          .OrderBy(i => i.LastName)
    //          .ToListAsync();

    //    if (id != null)
    //    {
    //      ViewData["InstructorID"] = id.Value;
    //      Instructor instructor = viewModel.Instructors.Where(
    //          i => i.ID == id.Value).Single();
    //      viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
    //    }

    //    if (courseID != null)
    //    {
    //      ViewData["CourseID"] = courseID.Value;
    //      viewModel.Enrollments = viewModel.Courses.Where(
    //          x => x.CourseID == courseID).Single().Enrollments;
    //    }

    //    return View(viewModel);
    //  }

    // Explicit Loading
    public async Task<IActionResult> Index(int? id, int? courseID)
    {
      var viewModel = new InstructorIndexData();
      viewModel.Instructors = await _context.Instructors
            .Include(i => i.OfficeAssignment)
            .Include(i => i.CourseAssignments)
              .ThenInclude(i => i.Course)
                  .ThenInclude(i => i.Department)
            .OrderBy(i => i.LastName)
            .ToListAsync();

      if (id != null)
      {
        ViewData["InstructorID"] = id.Value;
        Instructor instructor = viewModel.Instructors.Where(
            i => i.ID == id.Value).Single();
        viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
      }

      if (courseID != null)
      {
        ViewData["CourseID"] = courseID.Value;
        var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
        await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
        foreach (Enrollment enrollment in selectedCourse.Enrollments)
        {
          await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
        }
        viewModel.Enrollments = selectedCourse.Enrollments;
      }

      return View(viewModel);
    }

    // GET: Instructors/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var instructor = await _context.Instructors
          .FirstOrDefaultAsync(m => m.ID == id);
      if (instructor == null)
      {
        return NotFound();
      }

      return View(instructor);
    }

    // GET: Instructors/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Instructors/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,LastName,FirstMidName,HireDate")] Instructor instructor)
    {
      if (ModelState.IsValid)
      {
        _context.Add(instructor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(instructor);
    }

    // GET: Instructors/Edit/5
    //public async Task<IActionResult> Edit(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    //var instructor = await _context.Instructors.FindAsync(id);
    //    var instructor = await _context.Instructors
    //      .Include(i => i.OfficeAssignment)
    //      .AsNoTracking()
    //      .FirstOrDefaultAsync(m => m.ID == id);
    //    if (instructor == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(instructor);
    //}

    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var instructor = await _context.Instructors
          .Include(i => i.OfficeAssignment)
          .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
          .AsNoTracking()
          .FirstOrDefaultAsync(m => m.ID == id);
      if (instructor == null)
      {
        return NotFound();
      }
      PopulateAssignedCourseData(instructor);
      return View(instructor);
    }


    // POST: Instructors/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //  [HttpPost]
    //  [ValidateAntiForgeryToken]
    //  public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,HireDate,OfficeAssignment.Location")] Instructor instructor)
    //  {
    //    if (id != instructor.ID)
    //    {
    //      return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //      try
    //      {
    //        _context.Update(instructor);
    //        await _context.SaveChangesAsync();
    //      }
    //      catch (DbUpdateConcurrencyException)
    //      {
    //        if (!InstructorExists(instructor.ID))
    //        {
    //          return NotFound();
    //        }
    //        else
    //        {
    //          throw;
    //        }
    //      }
    //      return RedirectToAction(nameof(Index));
    //    }
    //    return View(instructor);
    //}

    //[HttpPost, ActionName("Edit")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> EditPost(int? id)
    //{
    //  if (id == null)
    //  {
    //    return NotFound();
    //  }

    //  var instructorToUpdate = await _context.Instructors
    //      .Include(i => i.OfficeAssignment)
    //      .FirstOrDefaultAsync(s => s.ID == id);

    //  // LTPE
    //  //if ( (instructorToUpdate != null) && (instructorToUpdate.OfficeAssignment == null) )
    //  //{
    //  //  instructorToUpdate.OfficeAssignment = new OfficeAssignment();
    //  //}

    //  // LTPE 
    //  // Fjernet check på retur værdi, da den giver false, hvis OfficeAssignment
    //  // ikke var sat før. I praksis virkder det alligevel.
    //  await TryUpdateModelAsync<Instructor>(
    //  instructorToUpdate,
    //  "",
    //  i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment);

    //  //if (await TryUpdateModelAsync<Instructor>(
    //  //    instructorToUpdate,
    //  //    "",
    //  //    i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
    //  //{
    //  if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
    //  {
    //    instructorToUpdate.OfficeAssignment = null;
    //  }
    //  try
    //  {
    //    await _context.SaveChangesAsync();
    //    // LTPE herunder
    //    return RedirectToAction(nameof(Index));
    //  }
    //  catch (DbUpdateException /* ex */)
    //  {
    //    //Log the error (uncomment ex variable name and write a log.)
    //    ModelState.AddModelError("", "Unable to save changes. " +
    //        "Try again, and if the problem persists, " +
    //        "see your system administrator.");
    //  }
    //  return RedirectToAction(nameof(Index));
    //  //}
    //  //return View(instructorToUpdate);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
    {
      if (id == null)
      {
        return NotFound();
      }

      var instructorToUpdate = await _context.Instructors
          .Include(i => i.OfficeAssignment)
          .Include(i => i.CourseAssignments)
              .ThenInclude(i => i.Course)
          .FirstOrDefaultAsync(m => m.ID == id);

      await TryUpdateModelAsync<Instructor>(
          instructorToUpdate,
          "",
          i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment);
      if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
      {
        instructorToUpdate.OfficeAssignment = null;
      }
      UpdateInstructorCourses(selectedCourses, instructorToUpdate);
      try
      {
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      catch (DbUpdateException /* ex */)
      {
        //Log the error (uncomment ex variable name and write a log.)
        ModelState.AddModelError("", "Unable to save changes. " +
            "Try again, and if the problem persists, " +
            "see your system administrator.");
      }
      UpdateInstructorCourses(selectedCourses, instructorToUpdate);
      PopulateAssignedCourseData(instructorToUpdate);
      return View(instructorToUpdate);
    }

    // GET: Instructors/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var instructor = await _context.Instructors
          .FirstOrDefaultAsync(m => m.ID == id);
      if (instructor == null)
      {
        return NotFound();
      }

      return View(instructor);
    }

    // POST: Instructors/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var instructor = await _context.Instructors.FindAsync(id);
      if (instructor != null)
      {
        _context.Instructors.Remove(instructor);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool InstructorExists(int id)
    {
      return _context.Instructors.Any(e => e.ID == id);
    }

    private void PopulateAssignedCourseData(Instructor instructor)
    {
      var allCourses = _context.Courses;
      var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));
      var viewModel = new List<AssignedCourseData>();
      foreach (var course in allCourses)
      {
        viewModel.Add(new AssignedCourseData
        {
          CourseID = course.CourseID,
          Title = course.Title,
          Assigned = instructorCourses.Contains(course.CourseID)
        });
      }
      ViewData["Courses"] = viewModel;
    }

    private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
    {
      if (selectedCourses == null)
      {
        instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
        return;
      }

      var selectedCoursesHS = new HashSet<string>(selectedCourses);
      var instructorCourses = new HashSet<int>
          (instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
      foreach (var course in _context.Courses)
      {
        if (selectedCoursesHS.Contains(course.CourseID.ToString()))
        {
          if (!instructorCourses.Contains(course.CourseID))
          {
            instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.CourseID });
          }
        }
        else
        {

          if (instructorCourses.Contains(course.CourseID))
          {
            CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.FirstOrDefault(i => i.CourseID == course.CourseID);
            _context.Remove(courseToRemove);
          }
        }
      }
    }
  }
}
