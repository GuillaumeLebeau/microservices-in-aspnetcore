using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ShoppingCart.Domains;

namespace ShoppingCart.Store
{
    public class EventStore : IEventStore
    {
        private static long _currentSequenceNumber;
        private static readonly IList<Event> database = new List<Event>();

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return database
                .Where(e => e.SequenceNumber >= firstEventSequenceNumber && e.SequenceNumber <= lastEventSequenceNumber)
                .OrderBy(e => e.SequenceNumber);
        }

        public void Raise(string eventName, object content)
        {
            // Gets a sequence number for the event
            var seqNumber = Interlocked.Increment(ref _currentSequenceNumber);
            database.Add(new Event(seqNumber, DateTimeOffset.UtcNow, eventName, content));
        }
    }
}
