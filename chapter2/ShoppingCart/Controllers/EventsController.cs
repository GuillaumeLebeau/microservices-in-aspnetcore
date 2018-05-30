using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Store;

namespace ShoppingCart.Controllers
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

        [HttpGet]
        public IActionResult Get(
            [FromQuery] long firstEventSequenceNumber = 0,
            [FromQuery] long lastEventSequenceNumber = long.MaxValue)
        {
            var events = _eventStore.GetEvents(firstEventSequenceNumber, lastEventSequenceNumber);
            return Ok(events);
        }
    }
}
