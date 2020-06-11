using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models.EF;
using Cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly ApbdDbContext _dbContext;

        public EnrollmentsController(ApbdDbContext dbContext, IDbService dbService)
        {
            _dbContext = dbContext;
            _dbService = dbService;
        }

        [HttpGet("{studentIndex}")]
        public IActionResult GetEnrollment(string studentIndex)
        {
            var enrollment = _dbService.GetStudentEnrollment(studentIndex);

            if (enrollment == null)
            {
                return NotFound("No enrollment found for given index.");
            }

            return Ok(enrollment);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            Studies studies;
            try
            {
                studies = _dbContext.Studies.Include(s => s.Enrollment).Where(s => s.Name == request.Studies).First();
            }
            catch (Exception)
            {
                return BadRequest("Study does not exist");
            }

            Enrollment enrollment;
            try
            {
                enrollment = studies.Enrollment.Where(e => e.Semester == 1).OrderByDescending(e => e.StartDate).First();
            }
            catch (Exception)
            {
                enrollment = new Enrollment
                {
                    IdEnrollment = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1,
                    Semester = 1,
                    StartDate = DateTime.Now,
                    IdStudyNavigation = studies
                };

                studies.Enrollment.Add(enrollment);
            }

            if (_dbContext.Student.Find(request.IndexNumber) != null)
            {
                return BadRequest("Student ID is not unique");
            }

            var student = new Student
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.Birthdate,
                IdEnrollmentNavigation = enrollment
            };

            enrollment.Student.Add(student);
            _dbContext.SaveChanges();

            var response = new EnrollStudentResponse
            {
                LastName = student.LastName,
                Study = studies.Name,
                Semester = enrollment.Semester,
                StartDate = enrollment.StartDate
            };

            return Created("", response);
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent(PromoteStudentsRequest request)
        {
            var enrollments = _dbContext.Enrollment.Include(e => e.IdStudyNavigation).ToList();

            Enrollment oldEnrollment;
            try
            {
                oldEnrollment = enrollments
                    .Where(e => e.Semester == request.Semester && e.IdStudyNavigation.Name == request.Studies)
                    .OrderByDescending(e => e.StartDate)
                    .First();
            }
            catch (Exception)
            {
                return NotFound("Enrollment does not exist");
            }

            Enrollment nextEnrollment;
            try
            {
                nextEnrollment = enrollments
                    .Where(e => e.Semester == request.Semester + 1 && e.IdStudyNavigation.Name == request.Studies)
                    .OrderByDescending(e => e.StartDate)
                    .First();
            }
            catch (Exception)
            {
                nextEnrollment = new Enrollment
                {
                    IdEnrollment = enrollments.Max(e => e.IdEnrollment) + 1,
                    Semester = request.Semester + 1,
                    StartDate = DateTime.Now,
                    IdStudyNavigation = oldEnrollment.IdStudyNavigation
                };
            }

            var students = _dbContext.Student.Where(s => s.IdEnrollmentNavigation == oldEnrollment).ToList();
            foreach (var s in students)
            {
                s.IdEnrollmentNavigation = nextEnrollment;

                _dbContext.Attach(s);
                _dbContext.Entry(s).State = EntityState.Modified;
            }

            _dbContext.SaveChanges();

            var response = new PromoteStudentsResponse
            {
                IdEnrollment = nextEnrollment.IdEnrollment,
                Course = request.Studies,
                Semester = nextEnrollment.Semester,
            };

            return Created("", response);
        }
    }
}

