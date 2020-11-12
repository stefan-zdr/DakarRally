using DakarRally.Interfaces.Services;
using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DakarRally.API.Controllers
{
    /// <summary>
    /// Race leaderboard statistics
    /// </summary>
    [RoutePrefix("api/races/{raceId}/leaderboards")]
    public class LeaderboardsController : BaseController
    {
        private readonly ILeaderboardsService _leaderboardService;
        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="leaderboardService"></param>
        public LeaderboardsController(ILeaderboardsService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Get race leaderboard.
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <returns>Leaderboard of specific race.</returns>
        [Route("", Name = "GetRaceLeaderboard")]
        public IHttpActionResult Get(int raceId)
        {
            return OkOrNotFound(_leaderboardService.GetLeaderboards(raceId));
        }

        /// <summary>
        /// Get race leaderboard for specific type.
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <param name="type">Vehicle type</param>
        /// <returns>Leaderboard of specific race.</returns>
        [Route("{type}", Name = "GetRaceLeaderboardFOrType")]
        public IHttpActionResult Get(int raceId, string type)
        {
            return OkOrNotFound(_leaderboardService.GetLeaderboardsForType(raceId, type));
        }
    }
}
