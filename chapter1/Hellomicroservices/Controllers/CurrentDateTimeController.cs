using System;
using Microsoft.AspNetCore.Mvc;

namespace Hellomicroservices.Controllers
{
    [Route("/api/[controller]")]
    public class CurrentDateTimeController : Controller
    {
        [HttpGet]
        public DateTime Get()
        {
            return DateTime.UtcNow;;
        }
    }
}