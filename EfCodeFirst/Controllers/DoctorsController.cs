using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EfCodeFirst.Models;

namespace EfCodeFirst.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly MedicamentsDbContext _context;

        public DoctorsController(MedicamentsDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetDoctor(int id)
        {
            var doctor = _context.Doctor.Find(id);

            if (doctor == null)
            {
                return NotFound("Doctor does not exists.");
            }

            return Ok(doctor);
        }

        [HttpPut("{id}")]
        public IActionResult PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.IdDoctor)
            {
                return BadRequest();
            }

            _context.Entry(doctor).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Doctor.Any(e => e.IdDoctor == id))
                {
                    return NotFound("Doctor does not exists.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public IActionResult PostDoctor(Doctor doctor)
        {
            _context.Doctor.Add(doctor);
            _context.SaveChanges();

            return Created("", doctor);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = _context.Doctor.Find(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctor.Remove(doctor);
            _context.SaveChanges();

            return Ok(doctor);
        }
    }
}
