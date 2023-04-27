using Flight_plannerAPI.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flight_plannerAPI.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class CleanupController : ControllerBase
    {
        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            FlightStorage.Clear();
            return Ok();
        }
    }
}
