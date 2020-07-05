using KolokwiumPopr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.Services
{
    public interface IFirefighterDbService
    {
        public Firefighter GetFirefighterById(int id);

        public IEnumerable<Models.Action> GetActionsByFirefighterId(int id);

        public Models.Action GetActionById(int id);

        public Firetruck GetFiretruckById(int id);

        public bool IsFiretruckUsed(Firetruck firetruck, Models.Action action);

        public FiretruckAction CreateFiretruckActionAssignment(Firetruck firetruck, Models.Action action);
        
    }
}
