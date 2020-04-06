using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var students = new List<Student>();

            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT S.IndexNumber, S.FirstName, S.LastName, S.BirthDate, SDS.Name as 'SDSName', E.Semester
                                        FROM Student S
                                        INNER JOIN Enrollment E on S.IdEnrollment = E.IdEnrollment
                                        INNER JOIN Studies SDS on E.IdStudy = SDS.IdStudy;";
                connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var student = new Student();
                    student.IndexNumber = dataReader["IndexNumber"].ToString();
                    student.FirstName = dataReader["FirstName"].ToString();
                    student.LastName = dataReader["LastName"].ToString();
                    student.BirthDate = DateTime.Parse(dataReader["BirthDate"].ToString());
                    student.Course = dataReader["SDSName"].ToString();
                    student.Semester = int.Parse(dataReader["Semester"].ToString());
                    students.Add(student);
                }
            }

            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }

            return NotFound("Student with given ID not found");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to DB
            //... generate index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, Student student)
        {
            return Ok("Update finished");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Remove finished");
        }
    }
}
