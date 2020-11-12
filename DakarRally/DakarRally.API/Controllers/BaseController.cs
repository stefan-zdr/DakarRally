using DakarRally.API.Filters;
using System.Web.Http;

namespace DakarRally.API.Controllers
{
    [ValidationActionFilter]
    public class BaseController : ApiController
    {
        protected IHttpActionResult OkOrNotFound<T>(T model)
        {
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }
        protected IHttpActionResult CreatedAtOrNotFound<T>(string routeName, object routeValues, T model)
        {
            if (model == null)
            {
                return NotFound();
            }
            return CreatedAtRoute(routeName, routeValues, model);
        }
    }
}
