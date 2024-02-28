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
  public class CoursesAsyncController : Controller
  {
    //private readonly SchoolContext _context;
    private readonly IUnitOfWorkAsync unitOfWorkAsync;

    public CoursesAsyncController(IUnitOfWorkAsync unitOfWorkAsync)
    {
      this.unitOfWorkAsync = unitOfWorkAsync;
    }

    // GET: Courses
    public async Task<IActionResult> Index()
    {
      var courses = await unitOfWorkAsync.CourseRepository.Get(includeProperties: "Department");
      return View(courses);
    }

    // GET: Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      Course course = await unitOfWorkAsync.CourseRepository.GetByFilter(filter: co => co.CourseID == id,
                                                                         includeProperties: "Department");
      if (course == null)
      {
        return NotFound();
      }

      return View(course);
    }

    // GET: Courses/Create
    public async Task <IActionResult> Create()
    {
      await PopulateDepartmentsDropDownList();
      return View();
    }

    // POST: Courses/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,DepartmentID")] Course course)
    {
      if (ModelState.IsValid)
      {
        await unitOfWorkAsync.CourseRepository.Insert(course);
        unitOfWorkAsync.Save();
        return RedirectToAction("Index");
      }

      await PopulateDepartmentsDropDownList(course.DepartmentID);
      return View(course);
    }

    // GET: Courses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      Course course = await unitOfWorkAsync.CourseRepository.GetByID(id);
      if (course == null)
      {
        return NotFound();
      }

      await PopulateDepartmentsDropDownList(course.DepartmentID);
      return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
           // LTPE
           [Bind("CourseID,Title,Credits,DepartmentID")]
         Course course)
    {
      try
      {
        if (ModelState.IsValid)
        {
          await unitOfWorkAsync.CourseRepository.Update(course);
          unitOfWorkAsync.Save();
          return RedirectToAction("Index");
        }
      }
      catch (DataException /* dex */)
      {
        //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
      }
      await PopulateDepartmentsDropDownList(course.DepartmentID);
      return View(course);
    }

    // GET: Courses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      Course course = await unitOfWorkAsync.CourseRepository.GetByID(id);
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
      Course course = await unitOfWorkAsync.CourseRepository.GetByID(id);
      if (course != null)
      {
        await unitOfWorkAsync.CourseRepository.Delete(id);
        unitOfWorkAsync.Save();
      }
      
      return RedirectToAction(nameof(Index));
    }

    //private bool CourseExists(int id)
    //{
    //  return _context.Courses.Any(e => e.CourseID == id);
    //}

    private async Task PopulateDepartmentsDropDownList(object selectedDepartment = null)
    {
      var departmentsQuery = await unitOfWorkAsync.DepartmentRepositoryAsync.Get(
             orderBy: q => q.OrderBy(d => d.Name));
      ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
    }

    public IActionResult UpdateCourseCredits()
    {
      return View();
    }

    //[HttpPost]
    //public async Task<IActionResult> UpdateCourseCredits(double? multiplier)
    //{
    //  if (multiplier != null)
    //  {
    //    ViewData["RowsAffected"] =
    //        await _context.Database.ExecuteSqlRawAsync(
    //            "UPDATE Course SET Credits = Credits * {0}",
    //            parameters: multiplier);
    //  }
    //  return View();
    //}
  }
}
