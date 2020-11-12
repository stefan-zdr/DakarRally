using DakarRally.Interfaces.Services;
using System.Collections.Generic;
using System.Web.Http;

namespace DakarRally.API.Controllers
{
    public class RacesController : ApiController
    {
        private readonly IRaceService _raceService;
        public RacesController(IRaceService raceService)
        {
            _raceService = raceService;
        }

        // GET: api/Races
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Races/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Races
        public IHttpActionResult Post([FromBody] int year)
        {
            return Ok(_raceService.CreateRace(year));
        }

        // PUT: api/Races/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Races/5
        public void Delete(int id)
        {
        }
    }
}
