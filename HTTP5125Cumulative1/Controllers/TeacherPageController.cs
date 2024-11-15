﻿using HTTP5125Cumulative1.Models;
using Microsoft.AspNetCore.Mvc;

namespace HTTP5125Cumulative1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        // GET: TeacherPage/List?StartHireDate={StartHireDate}&EndHireDate={EndHireDate}
        public IActionResult List(DateTime? StartHireDate = null, DateTime? EndHireDate = null)
        {
            List<Teacher> Teachers = _api.ListTeachers(StartHireDate, EndHireDate);
            return View(Teachers);
        }

        // GET: TeacherPage/Show/{id}
        public IActionResult Show(int id)
        {
            IActionResult response = _api.ShowTeacher(id);

            if (response is OkObjectResult okResult)
            {
                Teacher? SelectedTeacher = okResult.Value == null ? null : (Teacher)okResult.Value;
                return View(SelectedTeacher);
            }
            else
            {
                return View(null);
            }
        }
    }
}
