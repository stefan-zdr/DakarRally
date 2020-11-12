using DakarRally.Interfaces.Services;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DakarRally.API.Controllers
{
    [RoutePrefix("api/races/{raceId}/vehicles")]
    public class VehiclesController : BaseController
    {
        private readonly IRaceService _raceService;
        private readonly ILeaderboardsService _leaderboardService;
        public VehiclesController(IRaceService raceService, ILeaderboardsService leaderboardService)
        {
            _raceService = raceService;
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Find vehicles that match parameters in specific race.
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <param name="team">Team name</param>
        /// <param name="model">Model name</param>
        /// <param name="manufacturingDate">Manufacturing date</param>
        /// <param name="status">Malfunction status</param>
        /// <param name="distance">Vehicles distance</param>
        /// <returns></returns>
        [Route()]
        public IHttpActionResult Get(int raceId, string team = "", string model = "", DateTime? manufacturingDate = null, MalfunctionStatus? status = null, decimal? distance = null)
        {
            List<VehicleStatistics> vehicles = _leaderboardService.GetVehicles(raceId, team, model, manufacturingDate, status, distance);
            return OkOrNotFound(vehicles);
        }

        /// <summary>
        /// Get vehicle statistics
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <param name="vehicleId">Vehicle identifier</param>
        /// <returns></returns>
        [Route("{vehicleId:int}", Name = "GetVehicle")]
        public IHttpActionResult Get(int raceId, int vehicleId)
        {
            VehicleStatistics vehicle = _leaderboardService.GetVehicle(raceId, vehicleId);
            return OkOrNotFound(vehicle);
        }

        /// <summary>
        /// Create new vehicle for the specific race.
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <param name="model">Vehicle model</param>
        /// <returns></returns>
        [Route(Name = "AddVehicle")]
        public IHttpActionResult Post(int raceId, Vehicle model)
        {
            Vehicle vehicle = _raceService.AddVehicle(raceId, model);
            return CreatedAtOrNotFound("GetVehicle", new { id = vehicle?.Id }, vehicle);
        }

        /// <summary>
        /// Update vehicle data
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <param name="id">Vehicle identifier</param>
        /// <param name="model">Vehicle model</param>
        /// <returns></returns>
        [Route("{id:int}", Name = "UpdateVehicle")]
        public IHttpActionResult Put(int raceId, int id, Vehicle model)
        {
            Vehicle vehicle = _raceService.UpdateVehicle(raceId, id, model);
            return OkOrNotFound(vehicle);
        }

        /// <summary>
        /// Remove vehicle from the race.
        /// </summary>
        /// <param name="raceId">Race identifier</param>
        /// <param name="id">Vehicle identifier</param>
        /// <returns></returns>
        [Route("{id:int}", Name = "RemoveVehicle")]
        public IHttpActionResult Delete(int raceId, int id)
        {
            if (_raceService.RemoveFromRace(raceId, id))
            {
                return Ok("Vehicle succesfully removed from the race.");
            }
            return NotFound();
        }
    }
}
