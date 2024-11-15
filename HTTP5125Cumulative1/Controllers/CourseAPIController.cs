using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HTTP5125Cumulative1.Models;
using System;
using MySql.Data.MySqlClient;

namespace HTTP5125Cumulative1.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public CourseAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of Courses in the system.
        /// </summary>
        /// <example>
        /// GET api/Course/ListCourses -> [ { "courseId": 1, "courseCode": "http5101", "teacherId": 1, "startDate": "2018-09-04T00:00:00", "finishDate": "2018-12-14T00:00:00", "courseName": "Web Application Development" }, { "courseId": 2, "courseCode": "http5102", "teacherId": 2, "startDate": "2018-09-04T00:00:00", "finishDate": "2018-12-14T00:00:00", "courseName": "Project Management" }, ... ]
        /// </example>
        /// <returns>
        /// A list of courses objects
        /// </returns>
        [HttpGet]
        [Route(template: "ListCourses")]
        public List<Course> ListCourses()
        {
            // Create an empty list of Courses
            List<Course> Courses = new List<Course>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // SQL QUERY
                Command.CommandText = "SELECT * FROM courses";
                Command.Prepare();

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    // Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        int CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        string? CourseCode = ResultSet["coursecode"]?.ToString();
                        int? TeacherId = ResultSet["teacherid"] == DBNull.Value ? null : Convert.ToInt32(ResultSet["teacherid"]);
                        DateTime? StartDate = ResultSet["startdate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["startdate"]);
                        DateTime? FinishDate = ResultSet["finishdate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["finishdate"]);
                        string? CourseName = ResultSet["coursename"]?.ToString();

                        Course CurrentCourse = new Course()
                        {
                            CourseId = CourseId,
                            CourseCode = CourseCode,
                            TeacherId = TeacherId,
                            StartDate = StartDate,
                            FinishDate = FinishDate,
                            CourseName = CourseName
                        };

                        Courses.Add(CurrentCourse);
                    }
                }
            }

            // Return the final list of courses
            return Courses;
        }

        /// <summary>
        /// Returns a course in the database by their ID
        /// </summary>
        /// <param name="id">
        /// Course ID
        /// </param>
        /// <example>
        /// GET api/Course/ShowCourse/1 -> { "courseId": 1, "courseCode": "http5101", "teacherId": 1, "startDate": "2018-09-04T00:00:00", "finishDate": "2018-12-14T00:00:00", "courseName": "Web Application Development" }
        /// </example>
        /// <returns>
        /// A matching course object by its ID. Empty object if Course not found
        /// </returns>
        [HttpGet]
        [Route(template: "ShowCourse/{id}")]
        public Course ShowCourse(int id)
        {
            // Empty Course
            Course SelectedCourse = new Course();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "SELECT * FROM courses WHERE courseid = @id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    // If there is a record where courseid = id
                    if (ResultSet.Read())
                    {
                        int CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        string? CourseCode = ResultSet["coursecode"]?.ToString();
                        int? TeacherId = ResultSet["teacherid"] == DBNull.Value ? null : Convert.ToInt32(ResultSet["teacherid"]);
                        DateTime? StartDate = ResultSet["startdate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["startdate"]);
                        DateTime? FinishDate = ResultSet["finishdate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["finishdate"]);
                        string? CourseName = ResultSet["coursename"]?.ToString();

                        SelectedCourse.CourseId = CourseId;
                        SelectedCourse.CourseCode = CourseCode;
                        SelectedCourse.TeacherId = TeacherId;
                        SelectedCourse.StartDate = StartDate;
                        SelectedCourse.FinishDate = FinishDate;
                        SelectedCourse.CourseName = CourseName;
                    }
                }
            }

            // Return the selected course
            return SelectedCourse;
        }
    }
}
