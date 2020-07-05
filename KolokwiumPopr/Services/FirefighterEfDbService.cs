using KolokwiumPopr.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.Services
{
    public class FirefighterEfDbService : IFirefighterDbService
    {
        private readonly FirefightersDbContext _context;

        public FirefighterEfDbService(FirefightersDbContext context)
        {
            _context = context;
        }

        public Firefighter GetFirefighterById(int id)
        {
            return _context.Firefighter.Find(id);
        }

        public IEnumerable<Models.Action> GetActionsByFirefighterId(int id)
        {
            return _context.FirefighterAction
                           .Include(e => e.Action)
                           .Where(e => e.IdFirefighter == id)
                           .OrderByDescending(e => e.Action.EndDate)
                           .Select(e => e.Action)
                           .ToList();
        }

        public Models.Action GetActionById(int id)
        {
            return _context.Action.Find(id);
        }

        public Firetruck GetFiretruckById(int id)
        {
            return _context.Firetruck.Find(id);
        }

        public bool IsFiretruckUsed(Firetruck firetruck, Models.Action action)
        {
            return _context.FiretruckAction
                .Include("Action")
                .Where(e => (e.IdFireTruck == firetruck.IdFireTruck
                       && e.Action.StartDate >= action.StartDate
                       && (e.Action.EndDate == null || e.Action.EndDate <= action.EndDate)))
                .Count() > 0;
        }

        public FiretruckAction CreateFiretruckActionAssignment(Firetruck firetruck, Models.Action action)
        {
            var assignment = new FiretruckAction
            {
                IdFireTruck = firetruck.IdFireTruck,
                IdAction = action.IdAction,
                AssignmentDate = DateTime.Now,
            };

            _context.FiretruckAction.Add(assignment);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return null;
            }

            return assignment;
        }
    }
}
