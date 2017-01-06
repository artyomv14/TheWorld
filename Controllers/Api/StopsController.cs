using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using The_World.Models;
using The_World.Services;
using The_World.ViewModels;

namespace The_World.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    [Authorize]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger _logger;
        private GeoLocationService _geoService;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoLocationService geoService)
        {
            _logger = logger;
            _repository = repository;
            _geoService = geoService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetUserTripByName(tripName, User.Identity.Name);

                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get stops: {ex}");
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody] StopViewModel stopModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(stopModel);

                    var result = await _geoService.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;
                        _repository.AddStop(tripName, newStop, User.Identity.Name);
                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"/api/trips/{tripName}/stops/{newStop.Name}", Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save new Stop {ex}");
            }
            return BadRequest("Failed to save new Stop");
        }
    }
}
