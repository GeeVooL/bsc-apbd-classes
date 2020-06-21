using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kolokwium.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kolokwium.Controllers
{
    [Route("api/artists")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public ArtistsController(EventsDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var artist = _context.Artist.Find(id);
            if (artist == null)
            {
                return NotFound("Missing artist.");
            }

            List<Event> events;

            try
            {
                events = _context.ArtistEvent
                    .Include(e => e.Event)
                    .Where(ae => ae.IdArtist == artist.IdArtist)
                    .OrderByDescending(ae => ae.Event.StartDate)
                    .Select(ae => ae.Event)
                    .ToList();
            }
            catch (Exception)
            {
                return BadRequest("Cannot fetch events.");
            }

            return Ok(new
            {
                artist.IdArtist,
                artist.Nickname,
                Events = events
            });
        }

        [HttpPut("{artistId}/events/{eventId}")]
        public IActionResult Put(int artistId, int eventId, ArtistEvent artistEvent)
        {
            if (artistId != artistEvent.IdArtist || eventId != artistEvent.IdEvent)
            {
                return BadRequest("Inconsistent data.");
            }

            ArtistEvent oldArtistEvent = _context.ArtistEvent.Find(eventId, artistId);
            if (oldArtistEvent == null)
            {
                return BadRequest("Artist event does not exists");
            }

            if (artistEvent.PerformanceDate >= ev.StartDate)
            {
                return BadRequest("The event is in progress");
            }





            return Ok();
        }
    }
}
