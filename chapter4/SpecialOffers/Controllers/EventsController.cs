using Microsoft.AspNetCore.Mvc;
using SpecialOffers.Stores;

namespace SpecialOffers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventStore _eventStore;

        public EventsController(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        
        // GET api/events
        [HttpGet]
        public IActionResult Get(
            [FromQuery] long start = 0,
            [FromQuery] long end = long.MaxValue)
        {
            var events = _eventStore.GetEvents(start, end);
            return Ok(events);
        }
    }
}
