using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetEnrollment(String id)
        {
            var enrollments = new List<Enrollment>();

            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT E.IdEnrollment, E.Semester, E.StartDate
                                        FROM Enrollment E
                                        INNER JOIN Student S on S.IdEnrollment = E.IdEnrollment
                                        WHERE S.IndexNumber = " + id + ";";
                connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = int.Parse(dataReader["IdEnrollment"].ToString());
                    enrollment.Semester = int.Parse(dataReader["Semester"].ToString());
                    enrollment.StartDate = DateTime.Parse(dataReader["StartDate"].ToString());
                    enrollments.Add(enrollment);
                }
            }

            return Ok(enrollments);
        }
    }
}
