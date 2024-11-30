using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HTTP5125Cumulative1.Models;
using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HTTP5125Cumulative1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return a list of Teachers in the system
        /// If StartHireDate is included, search for teachers hired on or after StartHireDate
        /// If EndHireDate is included, search for teachers hired on or before EndHireDate
        /// </summary>
        /// <param name="StartHireDate">
        /// Teacher's start hire date filter, inclusive
        /// </param>
        /// <param name="EndHireDate">
        /// Teacher's end hire date filter, inclusive
        /// </param>
        /// <example>
        /// GET api/Teacher/ListTeachers?StartHireDate=2016-08-10 -> [ { "teacherId": 6, "teacherFName": "Thomas", "teacherLName": "Hawkins", "employeeNumber": "T393", "hireDate": "2016-08-10T00:00:00", "salary": 54.45, "courses": null } ]
        /// GET api/Teacher/ListTeachers?StartHireDate=2016-08-01&EndHireDate=2016-08-10 -> [ { "teacherId": 1, "teacherFName": "Alexander", "teacherLName": "Bennett", "employeeNumber": "T378", "hireDate": "2016-08-05T00:00:00", "salary": 55.3, "courses": null }, { "teacherId": 6, "teacherFName": "Thomas", "teacherLName": "Hawkins", "employeeNumber": "T393", "hireDate": "2016-08-10T00:00:00", "salary": 54.45, "courses": null } ]
        /// </example>
        /// <returns>
        /// A list of teacher objects
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers(DateTime? StartHireDate = null, DateTime? EndHireDate = null)
        {
            // Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // Initial query
                string query = "SELECT * FROM teachers";

                // Filter criteria, add start or end or both date inclusive bound to the query if not null
                if (StartHireDate != null && EndHireDate != null)
                {
                    query += " WHERE DATE(hiredate) BETWEEN @start AND @end";
                    Command.Parameters.AddWithValue("@start", StartHireDate?.ToString("yyyy-MM-dd"));
                    Command.Parameters.AddWithValue("@end", EndHireDate?.ToString("yyyy-MM-dd"));
                }
                else if (StartHireDate != null)
                {
                    query += " WHERE DATE(hiredate) >= @start";
                    Command.Parameters.AddWithValue("@start", StartHireDate?.ToString("yyyy-MM-dd"));
                }
                else if (EndHireDate != null)
                {
                    query += " WHERE DATE(hiredate) <= @end";
                    Command.Parameters.AddWithValue("@end", EndHireDate?.ToString("yyyy-MM-dd"));
                }

                // SQL QUERY
                Command.CommandText = query;
                Command.Prepare();

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    // Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        string? TeacherFName = ResultSet["teacherfname"]?.ToString();
                        string? TeacherLName = ResultSet["teacherlname"]?.ToString();
                        string? EmployeeNumber = ResultSet["employeenumber"]?.ToString();
                        DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal? Salary = ResultSet["salary"] == DBNull.Value ? null : Convert.ToDecimal(ResultSet["salary"]);

                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = TeacherId,
                            TeacherFName = TeacherFName,
                            TeacherLName = TeacherLName,
                            EmployeeNumber = EmployeeNumber,
                            HireDate = HireDate,
                            Salary = Salary
                        };

                        Teachers.Add(CurrentTeacher);
                    }
                }
            }

            // Return the final list of teachers
            return Teachers;
        }

        /// <summary>
        /// Return a teacher and the courses taught by that teacher in the database by their ID
        /// </summary>
        /// <param name="id">
        /// Teacher ID
        /// </param>
        /// <example>
        /// GET api/Teacher/ShowTeacher/1 -> { "teacherId": 1, "teacherFName": "Alexander", "teacherLName": "Bennett", "employeeNumber": "T378", "hireDate": "2016-08-05T00:00:00", "salary": 55.3, "courses": [ { "courseId": 1, "courseCode": "http5101", "teacherId": 1, "startDate": "2018-09-04T00:00:00", "finishDate": "2018-12-14T00:00:00", "courseName": "Web Application Development" } ] }
        /// </example>
        /// <returns>
        /// A matching teacher object by its ID wrapped by OkObjectResult. NotFoundObjectResult if Teacher not found
        /// </returns>
        [HttpGet]
        [Route(template: "ShowTeacher/{id}")]
        public IActionResult ShowTeacher(int id)
        {
            // Null Teacher
            Teacher? SelectedTeacher = null;

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database to get the selected teacher
                MySqlCommand TeacherCommand = Connection.CreateCommand();
                TeacherCommand.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                TeacherCommand.Parameters.AddWithValue("@id", id);

                // Gather Teacher Result Set of Query into a variable
                using (MySqlDataReader TeacherResultSet = TeacherCommand.ExecuteReader())
                {
                    // If there is a record where teacherid = id
                    if (TeacherResultSet.Read())
                    {
                        int TeacherId = Convert.ToInt32(TeacherResultSet["teacherid"]);
                        string? TeacherFName = TeacherResultSet["teacherfname"]?.ToString();
                        string? TeacherLName = TeacherResultSet["teacherlname"]?.ToString();
                        string? EmployeeNumber = TeacherResultSet["employeenumber"]?.ToString();
                        DateTime? HireDate = TeacherResultSet["hiredate"] == DBNull.Value ? null : Convert.ToDateTime(TeacherResultSet["hiredate"]);
                        decimal? Salary = TeacherResultSet["salary"] == DBNull.Value ? null : Convert.ToDecimal(TeacherResultSet["salary"]);
                        string? TeacherWorkPhone = TeacherResultSet["teacherworkphone"]?.ToString();

                        SelectedTeacher = new Teacher()
                        {
                            TeacherId = TeacherId,
                            TeacherFName = TeacherFName,
                            TeacherLName = TeacherLName,
                            EmployeeNumber = EmployeeNumber,
                            HireDate = HireDate,
                            Salary = Salary,
                            Courses = new List<Course>(),
                            TeacherWorkPhone = TeacherWorkPhone
                        };
                    }
                }

                // Return NotFoundObjectResult if there is no teacher with that ID
                if (SelectedTeacher == null)
                {
                    return NotFound($"Teacher with ID {id} not found.");
                }

                // Establish a new command (query) for our database to get the courses taught by this teacher
                MySqlCommand CoursesCommand = Connection.CreateCommand();
                CoursesCommand.CommandText = "SELECT * FROM courses WHERE teacherid = @id";
                CoursesCommand.Parameters.AddWithValue("@id", id);

                // Gather Courses Result Set of Query into a variable
                using (MySqlDataReader CoursesResultSet = CoursesCommand.ExecuteReader())
                {
                    // Loop Through Each Row the Courses Result Set
                    while (CoursesResultSet.Read())
                    {
                        int CourseId = Convert.ToInt32(CoursesResultSet["courseid"]);
                        string? CourseCode = CoursesResultSet["coursecode"]?.ToString();
                        int? TeacherId = CoursesResultSet["teacherid"] == DBNull.Value ? null : Convert.ToInt32(CoursesResultSet["teacherid"]);
                        DateTime? StartDate = CoursesResultSet["startdate"] == DBNull.Value ? null : Convert.ToDateTime(CoursesResultSet["startdate"]);
                        DateTime? FinishDate = CoursesResultSet["finishdate"] == DBNull.Value ? null : Convert.ToDateTime(CoursesResultSet["finishdate"]);
                        string? CourseName = CoursesResultSet["coursename"]?.ToString();

                        Course CurrentCourse = new Course()
                        {
                            CourseId = CourseId,
                            CourseCode = CourseCode,
                            TeacherId = TeacherId,
                            StartDate = StartDate,
                            FinishDate = FinishDate,
                            CourseName = CourseName
                        };

                        SelectedTeacher.Courses?.Add(CurrentCourse);
                    }
                }

            }

            // Return the selected teacher wrapped by OkObjectResult
            return Ok(SelectedTeacher);
        }

        /// <summary>
        /// Add a new teacher to the database after data validation
        /// </summary>
        /// <param name="TeacherData">
        /// TeacherData of type Teacher Object
        /// </param>
        /// <example>
        /// curl -H "Content-Type: application/json" -d "{ \"TeacherFName\": \"Foo\", \"TeacherLName\": \"Bar\", \"EmployeeNumber\": \"T123\", \"HireDate\": \"2024-11-22T00:00:00.000Z\", \"Salary\": 100000, \"TeacherWorkPhone\": \"1234567890\" }" "https://localhost:xxxx/api/Teacher/AddTeacher"
        /// </example>
        /// <returns>
        /// The new teacher ID of type int wrapped by OkObjectResult. UnprocessableEntityObjectResult if there is invalid data. ConflictObjectResult if there exists duplicated record.
        /// </returns>
        [HttpPost(template: "AddTeacher")]
        [Consumes("application/json")]
        public IActionResult AddTeacher([FromBody] Teacher TeacherData)
        {
            // Error handling when the teacher first name is empty
            if (string.IsNullOrWhiteSpace(TeacherData.TeacherFName))
            {
                return UnprocessableEntity(new HTTPErrorBody()
                {
                    Field = "TeacherFName",
                    Message = "Please provide valid teacher first name."
                });
            }

            // Error handling when the teacher last name is empty
            if (string.IsNullOrWhiteSpace(TeacherData.TeacherLName))
            {
                return UnprocessableEntity(new HTTPErrorBody()
                {
                    Field = "TeacherLName",
                    Message = "Please provide valid teacher last name."
                });
            }

            // Error handling when the teacher hire date is null or in the future
            if (TeacherData.HireDate == null || TeacherData.HireDate > DateTime.Now)
            {
                return UnprocessableEntity(new HTTPErrorBody()
                {
                    Field = "HireDate",
                    Message = "Please provide valid teacher hire date."
                });
            }

            // Error handling when the teacher employee number is null or not started with 'T' and followed by digits
            if (TeacherData.EmployeeNumber == null || !Regex.IsMatch(TeacherData.EmployeeNumber, @"^T\d+$"))
            {
                return UnprocessableEntity(new HTTPErrorBody()
                {
                    Field = "EmployeeNumber",
                    Message = "Please provide valid teacher employee number."
                });
            }

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database to get the duplicated results as there is no unique constraint in db
                MySqlCommand DuplicateCheckCommand = Connection.CreateCommand();
                DuplicateCheckCommand.CommandText = "SELECT * FROM teachers WHERE employeenumber = @employeenumber";
                DuplicateCheckCommand.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);

                // Gather Duplicated Result Set of Query into a variable
                using (MySqlDataReader DuplicatedResultSet = DuplicateCheckCommand.ExecuteReader())
                {
                    // If there is a record where employeenumber = @employeenumber
                    if (DuplicatedResultSet.Read())
                    {
                        return Conflict(new HTTPErrorBody()
                        {
                            Field = "EmployeeNumber",
                            Message = $"Teacher with employee number {TeacherData.EmployeeNumber} already exists."
                        });
                    }
                }

                // Establish a new command (query) for our database to insert data
                MySqlCommand InsertCommand = Connection.CreateCommand();
                InsertCommand.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone) values (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary, @teacherworkphone)";
                InsertCommand.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                InsertCommand.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                InsertCommand.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);
                InsertCommand.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);
                InsertCommand.Parameters.AddWithValue("@salary", TeacherData.Salary);
                InsertCommand.Parameters.AddWithValue("@teacherworkphone", TeacherData.TeacherWorkPhone);

                InsertCommand.ExecuteNonQuery();

                return Ok(Convert.ToInt32(InsertCommand.LastInsertedId));
            }
        }

        /// <summary>
        /// Deletes a Teacher from the database after existence check
        /// </summary>
        /// <param name="id">
        /// Teacher ID
        /// </param>
        /// <example>
        /// DELETE: api/Teacher/DeleteTeacher/1 -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by the delete operation of type int wrapped by OkObjectResult. NotFoundObjectResult if teacher with such ID does not exist.
        /// </returns>
        [HttpDelete(template: "DeleteTeacher/{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database to get the existing record in db
                MySqlCommand ExistCheckCommand = Connection.CreateCommand();
                ExistCheckCommand.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                ExistCheckCommand.Parameters.AddWithValue("@id", id);

                // Gather Exist Result Set of Query into a variable
                using (MySqlDataReader ExistResultSet = ExistCheckCommand.ExecuteReader())
                {
                    // If there is no record where teacherid = @id
                    if (!ExistResultSet.Read())
                    {
                        return NotFound(new HTTPErrorBody()
                        {
                            Field = "id",
                            Message = $"Teacher with ID {id} not found."
                        });
                    }
                }

                // Establish a new command (query) for our database to delete the existing record in db
                MySqlCommand DeleteCommand = Connection.CreateCommand();
                DeleteCommand.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                DeleteCommand.Parameters.AddWithValue("@id", id);
                return Ok(DeleteCommand.ExecuteNonQuery());
            }
        }
    }
}
