using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public string GetStudents(string orderBy)
        {
            return $"Kowalski, Malewski, Andrzejewski (orderBy={orderBy})";
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id ==2 )
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
    }
}