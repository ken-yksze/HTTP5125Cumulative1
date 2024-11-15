using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HTTP5125Cumulative1.Models;
using System;
using MySql.Data.MySqlClient;

namespace HTTP5125Cumulative1.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public StudentAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of Students in the system.
        /// </summary>
        /// <example>
        /// GET api/Student/ListStudents -> [ { "studentId": 1, "studentFName": "Sarah", "studentLName": "Valdez", "studentNumber": "N1678", "enrolDate": "2018-06-18T00:00:00" }, { "studentId": 2, "studentFName": "Jennifer", "studentLName": "Faulkner", "studentNumber": "N1679", "enrolDate": "2018-08-02T00:00:00" }, ... ]
        /// </example>
        /// <returns>
        /// A list of students objects
        /// </returns>
        [HttpGet]
        [Route(template: "ListStudents")]
        public List<Student> ListStudents()
        {
            // Create an empty list of Students
            List<Student> Students = new List<Student>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // SQL QUERY
                Command.CommandText = "SELECT * FROM students";
                Command.Prepare();

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    // Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        int StudentId = Convert.ToInt32(ResultSet["studentid"]);
                        string? StudentFName = ResultSet["studentfname"]?.ToString();
                        string? StudentLName = ResultSet["studentlname"]?.ToString();
                        string? StudentNumber = ResultSet["studentnumber"]?.ToString();
                        DateTime? EnrolDate = ResultSet["enroldate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["enroldate"]);

                        Student CurrentStudent = new Student()
                        {
                            StudentId = StudentId,
                            StudentFName = StudentFName,
                            StudentLName = StudentLName,
                            StudentNumber = StudentNumber,
                            EnrolDate = EnrolDate
                        };

                        Students.Add(CurrentStudent);
                    }
                }
            }

            // Return the final list of students
            return Students;
        }

        /// <summary>
        /// Returns a student in the database by their ID
        /// </summary>
        /// <param name="id">
        /// Student ID
        /// </param>
        /// <example>
        /// GET api/Student/ShowStudent/1 -> { "studentId": 1, "studentFName": "Sarah", "studentLName": "Valdez", "studentNumber": "N1678", "enrolDate": "2018-06-18T00:00:00" }
        /// </example>
        /// <returns>
        /// A matching student object by its ID. Empty object if Student not found
        /// </returns>
        [HttpGet]
        [Route(template: "ShowStudent/{id}")]
        public Student ShowStudent(int id)
        {
            // Empty Student
            Student SelectedStudent = new Student();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "SELECT * FROM students WHERE studentid = @id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    // If there is a record where courseid = id
                    if (ResultSet.Read())
                    {
                        int StudentId = Convert.ToInt32(ResultSet["studentid"]);
                        string? StudentFName = ResultSet["studentfname"]?.ToString();
                        string? StudentLName = ResultSet["studentlname"]?.ToString();
                        string? StudentNumber = ResultSet["studentnumber"]?.ToString();
                        DateTime? EnrolDate = ResultSet["enroldate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["enroldate"]);

                        SelectedStudent.StudentId = StudentId;
                        SelectedStudent.StudentFName = StudentFName;
                        SelectedStudent.StudentLName = StudentLName;
                        SelectedStudent.StudentNumber = StudentNumber;
                        SelectedStudent.EnrolDate = EnrolDate;
                    }
                }
            }

            // Return the selected student
            return SelectedStudent;
        }
    }
}
