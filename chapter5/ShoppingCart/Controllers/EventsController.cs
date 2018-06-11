using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Stores;

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private IEventStore _eventStore;

        public EventsController(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] long start = 0, [FromQuery] long end = long.MaxValue)
        {
            var events = await _eventStore.GetEvents(start, end);
            return Ok(events);
        }
    }
}
