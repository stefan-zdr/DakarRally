using DakarRally.BL;
using DakarRally.DAL;
using DakarRally.Interfaces.Services;
using DakarRally.Models.Races;
using System.Collections.Generic;
using System.Web.Http;

namespace DakarRally.API.Controllers
{
    [RoutePrefix("api/races")]
    public class RacesController : BaseController
    {
        private readonly IRaceService _raceService;
        private readonly ILeaderboardsService _leaderboardService;
        public RacesController(IRaceService raceService, ILeaderboardsService leaderboardService)
        {
            _raceService = raceService;
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Get race status
        /// </summary>
        /// <param name="id">Race identifier</param>
        /// <returns></returns>
        [Route(Name = "GetRaceStatus")]
        public IHttpActionResult GetRaceStatus(int id)
        {
            RaceStats vehicle = _leaderboardService.GetRaceStats(id);
            return OkOrNotFound(vehicle);
        }

        /// <summary>
        /// Create race for specific year
        /// </summary>
        /// <param name="year">Year in which race will start</param>
        /// <returns>New race</returns>
        [Route(Name = "CreateRace")]
        public IHttpActionResult Post(int year)
        {
            var race = _raceService.CreateRace(year);
            return CreatedAtOrNotFound("GetRaceStatus", new { id = race?.Id }, race);
        }

        /// <summary>
        /// Start the race
        /// </summary>
        /// <param name="id">Race identifier</param>
        [Route(Name = "StartRace")]
        public IHttpActionResult Patch(int id)
        {
            if (_raceService.StartRace(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
