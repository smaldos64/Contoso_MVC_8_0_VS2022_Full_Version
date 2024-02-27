using System;
using System.Data;
using System.Linq;
using Contoso_MVC_8_0_VS2022;

//using PagedList;
using Contoso_MVC_8_0_VS2022.DAL;
using Contoso_MVC_8_0_VS2022.Data;
using Contoso_MVC_8_0_VS2022.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Controllers
{
  public class StudentsController : Controller
  {
    private IStudentRepository studentRepository;
    //private SchoolContext context;

    //public StudentController()
    //{
    //  this.studentRepository = new StudentRepository(new SchoolContext());
    //}

    //public StudentController(IStudentRepository studentRepository, SchoolContext context)
    public StudentsController(IStudentRepository studentRepository)
    {
      this.studentRepository = studentRepository;
      //this.context = context; 
    }

    //
    // GET: /Student/
    public async Task<IActionResult> Index(
          string sortOrder,
          string currentFilter,
          string searchString,
          int? pageNumber)

    //public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
      ViewBag.CurrentSort = sortOrder;
      ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
      ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

      if (searchString != null)
      {
        pageNumber = 1;
      }
      else
      {
        searchString = currentFilter;
      }
      ViewBag.CurrentFilter = searchString;

      var students = from s in studentRepository.GetStudents()
                     select s;
      //IQueryable<Student> students1 = from s in context.Students
      //                                select s;

      if (!String.IsNullOrEmpty(searchString))
      {
        students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                               || s.FirstMidName.ToUpper().Contains(searchString.ToUpper()));
      }
      switch (sortOrder)
      {
        case "name_desc":
          students = students.OrderByDescending(s => s.LastName);
          break;
        case "Date":
          students = students.OrderBy(s => s.EnrollmentDate);
          break;
        case "date_desc":
          students = students.OrderByDescending(s => s.EnrollmentDate);
          break;
        default:  // Name ascending 
          students = students.OrderBy(s => s.LastName);
          break;
      }
      int pageSize = 3;
      // LTPE : Når der returneres en IEnumerable list fra StudentRepository, er 
      // Vi nødt til at lave en "kunstig" IQueryable liste her for at få koden 
      // i PaginatedList.cs til at virke. Vi er dog også nødt til at lave et par
      // små ændringer i metoden CreateAsync i PaginatedList.cs for at få
      // tingene til at virke korrekt. Netop på grund af at den IQueryable
      // liste vi får lavet her som før nævnt er "kunstig".
      // Me af hensyn til at kunne lave automatiseret test af vores repositories er vi
      // nødt til at returnere IEnumerable lister og ikke IQueryable lister fra
      // metoderne i vores Repositories !!!
      IQueryable<Student> studentsIQueryable = students.AsQueryable<Student>();
      return View(await PaginatedList<Student>.CreateAsync(studentsIQueryable, pageNumber ?? 1, pageSize));
      //return View(await PaginatedList<Student>.CreateAsync(students, pageNumber ?? 1, pageSize));
    }

    //
    // GET: /Student/Details/5

    public ViewResult Details(int id)
    {
      Student student = studentRepository.GetStudentByID(id);
      return View(student);
    }

    //
    // GET: /Student/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /Student/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(
       // LTPE
       //[Bind(Include = "LastName, FirstMidName, EnrollmentDate")]
       //    Student student)
       [Bind("LastName, FirstMidName, EnrollmentDate")]
           Student student)
    {
      try
      {
        if (ModelState.IsValid)
        {
          studentRepository.InsertStudent(student);
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

    //
    // GET: /Student/Edit/5

    public ActionResult Edit(int id)
    {
      Student student = studentRepository.GetStudentByID(id);
      return View(student);
    }

    //
    // POST: /Student/Edit/5

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(
       // LTPE 
       //[Bind(Include = "LastName, FirstMidName, EnrollmentDate")]
       //  Student student)
       // LTPE => ID
       [Bind("LastName, FirstMidName, EnrollmentDate, ID")]
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

    //
    // POST: /Student/Delete/5

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
      try
      {
        Student student = studentRepository.GetStudentByID(id);
        studentRepository.DeleteStudent(id);
        studentRepository.Save();
      }
      catch (DataException /* dex */)
      {
        //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
        return RedirectToAction("Delete", new { id = id, saveChangesError = true });
      }
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      studentRepository.Dispose();
      base.Dispose(disposing);
    }
  }
}