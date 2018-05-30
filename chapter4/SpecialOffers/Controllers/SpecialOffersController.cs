using Microsoft.AspNetCore.Mvc;
using SpecialOffers.Domains;
using SpecialOffers.Stores;

namespace SpecialOffers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialOffersController : ControllerBase
    {
        private readonly ISpecialOfferStore _specialOfferStore;

        public SpecialOffersController(ISpecialOfferStore specialOfferStore)
        {
            _specialOfferStore = specialOfferStore;
        }

        [HttpGet("{offerId:int}", Name = "GetOffer")]
        public ActionResult Get(int offerId)
        {
            var offer = _specialOfferStore.Get(offerId);
            if (offer == null)
                return NoContent();

            return Ok(offer);
        }

        [HttpPost]
        public ActionResult Post([FromBody] SpecialOffer specialOffer)
        {
            if (specialOffer == null)
                return BadRequest();

            _specialOfferStore.Add(specialOffer);
            _specialOfferStore.Save();

            return CreatedAtRoute("GetOffer", new {offerId = specialOffer.Id}, specialOffer);
        }

        [HttpPut("{offerId:int}")]
        public ActionResult Put(int offerId, SpecialOffer specialOffer)
        {
            if (specialOffer == null || specialOffer.Id != offerId)
                return BadRequest();
            
            var offer = _specialOfferStore.Get(offerId);
            if (offer == null)
                return NotFound();

            _specialOfferStore.Update(specialOffer);
            _specialOfferStore.Save();

            return NoContent();
        }
    }
}
