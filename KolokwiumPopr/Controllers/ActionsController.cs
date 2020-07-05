using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using KolokwiumPopr.DTOs.Requests;
using KolokwiumPopr.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KolokwiumPopr.Controllers
{
    [Route("api/actions")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly IFirefighterDbService _firefighterDbService;

        public ActionsController(IFirefighterDbService firefighterDbService)
        {
            _firefighterDbService = firefighterDbService;
        }

        [HttpPost("{id}/fire-trucks")]
        public IActionResult AssignFiretruck(int actionId, AssignFiretruckRequest request)
        {
            if (actionId != request.IdAction)
            {
                return BadRequest("Data mismatch.");
            }

            var firetruck = _firefighterDbService.GetFiretruckById(request.IdFireTruck);
            if (firetruck == null)
            {
                return NotFound("Firetruck not found.");
            }

            var action = _firefighterDbService.GetActionById(request.IdAction);
            if (action == null)
            {
                return NotFound("Action not found.");
            }

            if (_firefighterDbService.IsFiretruckUsed(firetruck, action))
            {
                return BadRequest("Firetruck is currently used");
            }

            if (action.NeedSpecialEquipment && !firetruck.SpecialEquipment)
            {
                return BadRequest("Firetruck does not contain required special equipment.");
            }

            var assignment = _firefighterDbService.CreateFiretruckActionAssignment(firetruck, action);
            if (assignment == null)
            {
                return BadRequest("Cannot create assignment.");
            }

            return Created("", assignment);
        }
    }
}
