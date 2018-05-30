using SpecialOffers.Domains;

namespace SpecialOffers.Stores
{
    public interface ISpecialOfferStore
    {
        SpecialOffer Get(int offerId);

        void Add(SpecialOffer specialOffer);

        void Update(SpecialOffer specialOffer);

        void Save();
    }
}
