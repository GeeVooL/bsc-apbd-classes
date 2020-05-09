using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
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
                                        WHERE S.IndexNumber = @id;";
                command.Parameters.AddWithValue("id", id);

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

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            Study study = null;
            Student student = new Student();
            Enrollment enrollment = new Enrollment();
            EnrollStudentResponse response = new EnrollStudentResponse();

            student.IndexNumber = request.IndexNumber;
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.BirthDate = request.Birthdate;
            student.Course = request.Studies;
            student.Semester = 1;

            // Check if studies exist
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                try
                {
                    command.Connection = connection;
                    connection.Open();

                    command.CommandText = "SELECT IdStudy, Name FROM Studies WHERE name=@name";
                    command.Parameters.AddWithValue("name", student.Course);
                    var reader = command.ExecuteReader();
                    reader.Read();

                    study = new Study();
                    study.IdStudy = (int)reader["IdStudy"];
                    study.Name = reader["Name"].ToString();
                } 
                catch (Exception)
                {
                    study = null;
                }
            }

            if (study == null)
            {
                return BadRequest("Study does not exist");
            }

            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;

                // Check if the enrollment exists
                try {
                    var SELECT_ENROLLMENT = @"SELECT TOP 1 IdEnrollment, Semester, IdStudy, StartDate 
                                              FROM Enrollment 
                                              WHERE IdStudy=@idStudy AND Semester=@semester 
                                              ORDER BY StartDate DESC;";      
                    command.CommandText = SELECT_ENROLLMENT;
                    command.Parameters.AddWithValue("semester", student.Semester);
                    command.Parameters.AddWithValue("idStudy", study.IdStudy);
                    var reader = command.ExecuteReader();
                    bool exists = reader.Read();

                    if (!exists)
                    {
                        reader.Close();

                        // If not exists, create a new one
                        command.CommandText = @"INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) 
                                                SELECT MAX(IdEnrollment) + 1, @semester, @idStudy, @startDate 
                                                FROM Enrollment WITH (ROWLOCK, XLOCK, HOLDLOCK)";
                        command.Parameters.AddWithValue("startDate", DateTime.Now);
                        command.ExecuteNonQuery();

                        // Get the new value
                        command.CommandText = SELECT_ENROLLMENT;
                        var tmpReader = command.ExecuteReader();
                        tmpReader.Read();

                        enrollment.IdEnrollment = (int)tmpReader["IdEnrollment"];
                        enrollment.Semester = (int)tmpReader["Semester"];
                        enrollment.StartDate = DateTime.Parse(tmpReader["StartDate"].ToString());

                        tmpReader.Close();
                    }
                    else
                    {
                        enrollment.IdEnrollment = (int)reader["IdEnrollment"];
                        enrollment.Semester = (int)reader["Semester"];
                        enrollment.StartDate = DateTime.Parse(reader["StartDate"].ToString());

                        reader.Close();
                    }
                } 
                catch (Exception)
                {
                    transaction.Rollback();
                    return BadRequest("Cannot create or read enrollment");
                }

                // Check if the student's id is unique
                try
                {
                    command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexNumber";
                    command.Parameters.AddWithValue("indexNumber", student.IndexNumber);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        return BadRequest("Student with the given ID already exists");
                    }
                    reader.Close();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return BadRequest("Cannot read student");
                }

                // Enroll student
                try
                {
                    command.CommandText = @"INSERT INTO Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) 
                                            VALUES (@indexNumber, @firstName, @lastName, @birthDate, @idEnrollment)";
                    command.Parameters.AddWithValue("firstName", student.FirstName);
                    command.Parameters.AddWithValue("lastName", student.LastName);
                    command.Parameters.AddWithValue("birthDate", student.BirthDate);
                    command.Parameters.AddWithValue("idEnrollment", enrollment.IdEnrollment);
                    command.ExecuteNonQuery();

                    response.LastName = student.LastName;
                    response.Study = study.Name;
                    response.Semester = enrollment.Semester;
                    response.StartDate = enrollment.StartDate;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return BadRequest("Cannot enroll student");
                }

                transaction.Commit();
            }

            return Created("", response);
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudent(PromoteStudentsRequest request)
        {
            PromoteStudentsResponse response = new PromoteStudentsResponse();
            Enrollment enrollment = null;

            // Check if studies exist
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                try
                {
                    command.Connection = connection;
                    connection.Open();

                    command.CommandText = @"SELECT TOP 1 e.IdEnrollment, e.Semester, e.IdStudy, e.StartDate FROM Enrollment e 
                                            INNER JOIN Studies s ON s.IdStudy = e.IdStudy 
                                            WHERE e.Semester = @semester AND s.Name = @name 
                                            ORDER BY e.StartDate DESC;";
                    command.Parameters.AddWithValue("semester", request.Semester);
                    command.Parameters.AddWithValue("name", request.Studies);
                    var reader = command.ExecuteReader();
                    reader.Read();

                    enrollment = new Enrollment();
                    enrollment.IdEnrollment = (int)reader["IdEnrollment"];
                    enrollment.Course = (int)reader["IdStudy"];
                    enrollment.Semester = (int)reader["Semester"];
                    enrollment.StartDate = DateTime.Parse(reader["StartDate"].ToString());

                    reader.Close();
                }
                catch (Exception)
                {
                    enrollment = null;
                }
            }

            if (enrollment == null)
            {
                return NotFound("Enrollment does not exist");
            }

            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                command.Connection = connection;
                command.Transaction = transaction;

                // Execute procedure
                try
                {
                    command.CommandText = "[s19682].[PromoteStudents]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdStudy", enrollment.Course);
                    command.Parameters.AddWithValue("@CurrentSemester", enrollment.Semester);

                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return BadRequest("Cannot promote students");
                }

                // Get the new enrollment
                try
                {
                    command.CommandText = @"SELECT TOP 1 IdEnrollment, Semester, IdStudy, StartDate FROM Enrollment 
                                            WHERE Semester = @CurrentSemester + 1 AND IdStudy = @IdStudy 
                                            ORDER BY StartDate DESC;";
                    command.CommandType = CommandType.Text;
                    var reader = command.ExecuteReader();
                    reader.Read();
                    
                    response.IdEnrollment = (int)reader["IdEnrollment"];
                    response.Course = request.Studies;
                    response.Semester = (int)reader["Semester"];

                    reader.Close();
                } 
                catch (Exception e)
                {
                    transaction.Rollback();
                    return BadRequest("Cannot get the new enrollment");
                }

                transaction.Commit();
            }

            return Created("", response);
        }
    }
}

