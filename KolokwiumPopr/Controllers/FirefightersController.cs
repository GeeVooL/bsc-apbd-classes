using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KolokwiumPopr.DTOs.Responses;
using KolokwiumPopr.Models;
using KolokwiumPopr.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KolokwiumPopr.Controllers
{
    [Route("api/firefighters")]
    [ApiController]
    public class FirefightersController : ControllerBase
    {
        private readonly IFirefighterDbService _firefighterDbService;

        public FirefightersController(IFirefighterDbService firefighterDbService)
        {
            _firefighterDbService = firefighterDbService;
        }

        [HttpGet("{id}/actions")]
        public IActionResult GetActions(int id)
        {
            var firefighter = _firefighterDbService.GetFirefighterById(id);
            if (firefighter == null)
            {
                return NotFound("Firefighter not found.");
            }

            IEnumerable<Models.Action> actions;
            try
            {
                actions = _firefighterDbService.GetActionsByFirefighterId(id);
            }
            catch (Exception)
            {
                return BadRequest("Cannot list actions.");
            }

            var response = new ActionResponse
            {
                Actions = actions.Select(e => new ActionResponseItem
                {
                    IdAction = e.IdAction,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
            };

            return Ok(response);
        }
    }
}
