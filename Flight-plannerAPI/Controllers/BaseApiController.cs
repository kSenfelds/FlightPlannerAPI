using Microsoft.AspNetCore.Mvc;

namespace Flight_plannerAPI.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected FlightPlannerDbContext _context; 
        public BaseApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }
    }
}