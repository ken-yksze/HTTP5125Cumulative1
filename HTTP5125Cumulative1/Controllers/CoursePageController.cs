using HTTP5125Cumulative1.Models;
using Microsoft.AspNetCore.Mvc;

namespace HTTP5125Cumulative1.Controllers
{
    public class CoursePageController : Controller
    {
        private readonly CourseAPIController _api;

        public CoursePageController(CourseAPIController api)
        {
            _api = api;
        }

        // GET: CoursePage/List
        public IActionResult List()
        {
            List<Course> Courses = _api.ListCourses();
            return View(Courses);
        }

        // GET: CoursePage/Show/{id}
        public IActionResult Show(int id)
        {
            Course SelectedCourse = _api.ShowCourse(id);
            return View(SelectedCourse);
        }
    }
}
