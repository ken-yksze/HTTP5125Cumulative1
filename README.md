# HTTP5125Cumulative1
## APIs
- GET /api/Teacher/ListTeachers
    - Summary
        - Return a list of Teachers in the system
        - If StartHireDate is included, search for teachers hired on or after StartHireDate
        - If EndHireDate is included, search for teachers hired on or before EndHireDate
    - Query Params
        - StartHireDate (DateTime), teacher's hire date start inclusive bound
        - EndHireDate (DateTime), teacher's hire date end inclusive bound
    - Usage
        - GET api/Teacher/ListTeachers?StartHireDate=2016-08-10 -> \[ { "teacherId": 6, ... } \]
        - GET api/Teacher/ListTeachers?StartHireDate=2016-08-01&EndHireDate=2016-08-10 -> \[ { "teacherId": 1, ... }, { "teacherId": 6, ... } \]
        - GET api/Teacher/ListTeachers?StartHireDate=2016-08-11 -> \[\]

- GET /api/Teacher/ShowTeacher/{id}
    - Summary
        - Return a teacher and the courses taught by that teacher in the database by their ID
    - Path Params
        - id (int), teacher ID
    - Usage
        - GET api/Teacher/ShowTeacher/1 -> { "teacherId": 1, ... }
        - GET api/Teacher/ShowTeacher/-1 -> 404 Not Found

- POST /api/Teacher/AddTeacher
    - Summary
        - Add a new teacher to the database after data validation
    - Body (JSON)
        - string? TeacherFName
        - string? TeacherLName
        - string? EmployeeNumber
        - DateTime? HireDate
        - decimal? Salary
        - string? TeacherWorkPhone
    - Return
        - The new teacher ID of type int wrapped by OkObjectResult.
        - UnprocessableEntityObjectResult if there is invalid data.
        - ConflictObjectResult if there exists duplicated record.
    - Usage
        - curl -H "Content-Type: application/json" -d "{ \"TeacherFName\": \"Foo\", \"TeacherLName\": \"Bar\", \"EmployeeNumber\": \"T123\", \"HireDate\": \"2024-11-22T00:00:00.000Z\", \"Salary\": 100000, \"TeacherWorkPhone\": \"1234567890\" }" "https://localhost:xxxx/api/Teacher/AddTeacher"

- DELETE /api/Teacher/DeleteTeacher/{id}
    - Summary
        - Deletes a Teacher from the database after existence check
    - Path Params
        - id (int), teacher ID
    - Return
        - Number of rows affected by the delete operation of type int wrapped by OkObjectResult.
        - NotFoundObjectResult if teacher with such ID does not exist.
    - Usage
        - DELETE: api/Teacher/DeleteTeacher/1 -> 1

- PUT /api/Teacher/UpdateTeacher/{id}
    - Summary
        - Update a Teacher in the database after data validation & existence check
    - Body (JSON)
        - string? TeacherFName
        - string? TeacherLName
        - DateTime? HireDate
        - decimal? Salary
    - Return
        - The updated Teacher object wrapped by OkObjectResult.
        - UnprocessableEntityObjectResult if there is invalid data.
        - NotFoundObjectResult if Teacher not found.
    - Usage
        - curl -X PUT -H "Content-Type: application/json" -d "{ \"TeacherFName\": \"Foo\", \"TeacherLName\": \"Bar\", \"HireDate\": \"2024-11-22T00:00:00.000Z\", \"Salary\": 100000 }" "https://localhost:xxxx/api/Teacher/UpdateTeacher/1"

- GET /api/Student/ListStudents
    - Summary
        - Return a list of Students in the system
    - Usage
        - GET api/Student/ListStudents -> \[ { "studentId": 1, ... }, { "studentId": 2, ... }, ... \]

- GET /api/Student/ShowStudent/{id}
    - Summary
        - Returns a student in the database by their ID
    - Path Params
        - id (int), student ID
    - Usage
        - GET api/Student/ShowStudent/1 -> { "studentId": 1, ... }
        - GET api/Student/ShowStudent/-1 -> { "studentId": 0, "studentFName": null, ... }

- GET /api/Course/ListCourses
    - Summary
        - Return a list of Courses in the system
    - Usage
        - GET api/Course/ListCourses -> \[ { "courseId": 1, ... }, { "courseId": 2, ... }, ... \]

- GET /api/Course/ShowCourse/{id}
    - Summary
        - Returns a course in the database by their ID
    - Path Params
        - id (int), course ID
    - Usage
        - GET api/Course/ShowCourse/1 -> { "courseId": 1, ... }
        - GET api/Course/ShowCourse/-1 -> { "courseId": 0, "courseCode": null, ... }

## Pages
- TeacherPage/List?StartHireDate={StartHireDate}&EndHireDate={EndHireDate}

- TeacherPage/Show/{id}

- TeacherPage/New

- TeacherPage/DeleteConfirm/{id}

- TeacherPage/Update/{id}

- StudentPage/List

- StudentPage/Show/{id}

- CoursePage/List

- CoursePage/Show/{id}
