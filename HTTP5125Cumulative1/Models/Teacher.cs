namespace HTTP5125Cumulative1.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string? TeacherFName { get; set; }
        public string? TeacherLName { get; set; }
        public string? EmployeeNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public List<Course>? Courses { get; set; }
    }
}
