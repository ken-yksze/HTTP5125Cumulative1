using HTTP5125Cumulative1.Models;
using Microsoft.AspNetCore.Mvc;

namespace HTTP5125Cumulative1.Controllers
{
    public class StudentPageController : Controller
    {
        private readonly StudentAPIController _api;

        public StudentPageController(StudentAPIController api)
        {
            _api = api;
        }

        // GET: StudentPage/List
        public IActionResult List()
        {
            List<Student> Students = _api.ListStudents();
            return View(Students);
        }

        // GET: StudentPage/Show/{id}
        public IActionResult Show(int id)
        {
            Student SelectedStudent = _api.ShowStudent(id);
            return View(SelectedStudent);
        }
    }
}
