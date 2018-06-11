using System;

namespace ShoppingCart.Domains
{
    public struct Event
    {
        public Event(long sequenceNumber, DateTimeOffset occurredAt, string name, object content)
        {
            SequenceNumber = sequenceNumber;
            OccurredAt = occurredAt;
            Name = name;
            Content = content;
        }

        public long SequenceNumber { get; }
        public DateTimeOffset OccurredAt { get; }
        public string Name { get; }
        public object Content { get; }
    }
}
