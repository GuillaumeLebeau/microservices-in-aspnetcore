using System.Collections.Generic;
using System.Threading.Tasks;

using ShoppingCart.Domains;

namespace ShoppingCart.Stores
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);

        Task Raise(string eventName, object content);
    }
}
