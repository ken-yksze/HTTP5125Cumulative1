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

- StudentPage/List

- StudentPage/Show/{id}

- CoursePage/List

- CoursePage/Show/{id}
