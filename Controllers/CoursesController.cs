using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;

namespace Contoso_MVC_8_0_VS2022.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
          //var schoolContext = _context.Courses.Include(c => c.Department);
          //return View(await schoolContext.ToListAsync());
          //var courses = _context.Courses
          //  .Include(c => c.Department)
          //  .AsNoTracking();
          //IQueryable<Course> courses = _context.Courses
          //  .Include(c => c.Department)
          //  .AsNoTracking();
          // return View(await courses.ToListAsync());
          IEnumerable<Course> courses = _context.Courses
            .Include(c => c.Department)
            .AsNoTracking();
          return View(courses);
    }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
          //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
          PopulateDepartmentsDropDownList();
          return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,DepartmentID")] Course course)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
           
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            //return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", course.DepartmentID);
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

          // POST: Courses/Edit/5
          // To protect from overposting attacks, enable the specific properties you want to bind to.
          // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Edit(int id, [Bind("CourseID,Title,Credits,DepartmentID")] Course course)
          {
            if (id != course.CourseID)
            {
              return NotFound();
            }

            //if (ModelState.IsValid)
            //{
              try
              {
                _context.Update(course);
                await _context.SaveChangesAsync();
              }
              catch (DbUpdateConcurrencyException)
              {
                if (!CourseExists(course.CourseID))
                {
                  return NotFound();
                }
                else
                {
                  throw;
                }
              }
              return RedirectToAction(nameof(Index));
            //}
            ////ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", course.DepartmentID);
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            //return View(course);
          }

    //[HttpPost, ActionName("Edit")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> EditPost(int? id)
    //{
    //  if (id == null)
    //  {
    //    return NotFound();
    //  }

    //  var courseToUpdate = await _context.Courses
    //      .FirstOrDefaultAsync(c => c.CourseID == id);

    //  if (await TryUpdateModelAsync<Course>(courseToUpdate,
    //      "",
    //      c => c.Credits, c => c.DepartmentID, c => c.Title))
    //  {
    //    try
    //    {
    //      await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateException /* ex */)
    //    {
    //      //Log the error (uncomment ex variable name and write a log.)
    //      ModelState.AddModelError("", "Unable to save changes. " +
    //          "Try again, and if the problem persists, " +
    //          "see your system administrator.");
    //    }
    //    return RedirectToAction(nameof(Index));
    //  }
    //  PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
    //  return View(courseToUpdate);
    //}

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
          var departmentsQuery = from d in _context.Departments
                                 orderby d.Name
                                 select d;
          ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }
  }
}
