using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace The_World.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting All Trips from the DB");
            return _context.Trips.ToList();
        }

        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            return _context
                .Trips
                .Include(t=> t.Stops)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public void AddStop(string tripName, Stop newStop, string userName)
        {
            var trip = GetUserTripByName(tripName, userName);
            trip?.Stops.Add(newStop);
            _context.Stops.Add(newStop);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public Trip GetTripByName(string tripName)
        {
            return _context
                .Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name == tripName);
        }

        public Trip GetUserTripByName(string tripName, string userName)
        {
            return _context.Trips
               .Include(t => t.Stops)
               .FirstOrDefault(t => t.Name == tripName && t.UserName == userName);
        }
    }
}
