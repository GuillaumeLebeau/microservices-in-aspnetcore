using System.Collections.Generic;
using ShoppingCart.Domains;

namespace ShoppingCart.Store
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);

        void Raise(string eventName, object content);
    }
}
