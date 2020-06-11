using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Cw3.Services;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace Cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly ApbdDbContext _dbContext;

        public StudentsController(IDbService dbService, ApbdDbContext dbContext)
        {
            _dbService = dbService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Models.EF.Student> students;
            try
            {
                students = _dbContext.Student.ToList();
            }
            catch (Exception)
            {
                return BadRequest("Cannot fetch students");
            }
            return Ok(students);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var student = _dbService.GetStudent(indexNumber);
            if (student == null)
            {
                return NotFound("Student with the given ID not found.");
            }

            return Ok(student);
        }

        [HttpPost]
        public IActionResult CreateStudent(Models.EF.Student student)
        {
            //... add to DB
            //... generate index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(string id, Models.EF.Student student)
        {
            if (id != student.IndexNumber)
            {
                return BadRequest();
            }

            _dbContext.Attach(student);
            _dbContext.Entry(student).State = EntityState.Modified;

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Student.Any(e => e.IndexNumber == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var student = _dbContext.Student.Find(id);
            if (student == null)
            {
                return BadRequest("No student with given ID exists");
            }

            // Attach and cascade remove
            _dbContext.Student.Remove(student);
            _dbContext.SaveChanges();
            return Ok("Student removed");
        }
    }
}
