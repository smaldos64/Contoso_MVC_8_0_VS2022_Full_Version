using System;
using System.Collections.Generic;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Contoso_MVC_8_0_VS2022.Models;
using Contoso_MVC_8_0_VS2022.DAL;
using Contoso_MVC_8_0_VS2022.Data;

namespace Contoso_MVC_8_0_VS2022.Controllers
{
   public class CoursesController : Controller
   {
      // LTPE
      private UnitOfWork unitOfWork;
      //private UnitOfWork unitOfWork = new UnitOfWork();

      public CoursesController(SchoolContext context)
      {
        unitOfWork = new UnitOfWork(context);
      }

    //
    // GET: /Course/

      public ViewResult Index()
      {
         var courses = unitOfWork.CourseRepository.Get(includeProperties: "Department");
         return View(courses.ToList());
      }

      //
      // GET: /Course/Details/5

      public ViewResult Details(int id)
      {
         // LTPE
         //Course course = unitOfWork.CourseRepository.GetByID(id);

         //var course = unitOfWork.CourseRepository.Get(includeProperties: "Department", filter: co => co.CourseID == id, Id: id);
         //return View(course.FirstOrDefault());

         Course course = unitOfWork.CourseRepository.GetByFilter(filter: co => co.CourseID == id,
                                                                 includeProperties: "Department");
         return View(course);
      }

      //
      // GET: /Course/Create

      public ActionResult Create()
      {
         PopulateDepartmentsDropDownList();
         return View();
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Create(
          // LTPE
          [Bind("CourseID,Title,Credits,DepartmentID")]
          Course course)
      {
         try
         {
            if (ModelState.IsValid)
            {
               unitOfWork.CourseRepository.Insert(course);
               unitOfWork.Save();
               return RedirectToAction("Index");
            }
         }
         catch (DataException /* dex */)
         {
            //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
         }
         PopulateDepartmentsDropDownList(course.DepartmentID);
         return View(course);
      }

      public ActionResult Edit(int id)
      {
         Course course = unitOfWork.CourseRepository.GetByID(id);
         PopulateDepartmentsDropDownList(course.DepartmentID);
         return View(course);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult Edit(
           // LTPE
           [Bind("CourseID,Title,Credits,DepartmentID")]
         Course course)
      {
         try
         {
            if (ModelState.IsValid)
            {
               unitOfWork.CourseRepository.Update(course);
               unitOfWork.Save();
               return RedirectToAction("Index");
            }
         }
         catch (DataException /* dex */)
         {
            //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
         }
         PopulateDepartmentsDropDownList(course.DepartmentID);
         return View(course);
      }

      private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
      {
         var departmentsQuery = unitOfWork.DepartmentRepository.Get(
             orderBy: q => q.OrderBy(d => d.Name));
         ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
      }

      //
      // GET: /Course/Delete/5
      public ActionResult Delete(int id)
      {
         Course course = unitOfWork.CourseRepository.GetByID(id);
         return View(course);
      }

      //
      // POST: /Course/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public ActionResult DeleteConfirmed(int id)
      {
         Course course = unitOfWork.CourseRepository.GetByID(id);
         unitOfWork.CourseRepository.Delete(id);
         unitOfWork.Save();
         return RedirectToAction("Index");
      }

      protected override void Dispose(bool disposing)
      {
         unitOfWork.Dispose();
         base.Dispose(disposing);
      }
   }
}