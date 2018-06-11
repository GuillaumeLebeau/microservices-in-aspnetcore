using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ShoppingCart.Domains;

namespace ShoppingCart.Stores
{
    public class EventStore : IEventStore, IDisposable
    {
        private const string ShoppingCartStream = "ShoppingCart";
        private const int MaxPageSize = 4096;
        private readonly IEventStoreConnection _connection;

        public EventStore(string connectionString)
        {
            _connection = EventStoreConnection.Create(connectionString);
            
            // Opens the connection to EventStore
            _connection.ConnectAsync().Wait();
        }

        public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            var count = (int)(lastEventSequenceNumber - firstEventSequenceNumber);
            if (count > MaxPageSize)
                count = MaxPageSize;
            else if (count <= 0)
                count = 1;
            
            Stopwatch sw = Stopwatch.StartNew();
            
            sw.Stop();
            Console.WriteLine("connect took: " + sw.ElapsedMilliseconds);
            sw.Restart();

            // Reads events from the Event Store
            var streamEvents = await _connection.ReadStreamEventsForwardAsync(
                    ShoppingCartStream,
                    firstEventSequenceNumber,
                    count,
                    false
                )
                .ConfigureAwait(false);
            
            sw.Stop();
            Console.WriteLine("read events took: " + sw.ElapsedMilliseconds);
            sw.Restart();

            var converter = new ExpandoObjectConverter();

            // Accesses the events on the result from the Event Store
            var events = streamEvents.Events.Select(
                    ev => new
                    {
                        Content = JsonConvert.DeserializeObject<ExpandoObject>(
                            Encoding.UTF8.GetString(ev.Event.Data),
                            converter
                        ),

                        // Deserializes the metadata part of each event
                        Metadata = JsonConvert.DeserializeObject<EventMetadata>(
                            Encoding.UTF8.GetString(ev.Event.Metadata)
                        )
                    }
                )
                // Maps to events from Event Store Event objects
                .Select(
                    (ev, i) => new Event(
                        i + firstEventSequenceNumber,
                        ev.Metadata.OccurredAt,
                        ev.Metadata.EventName,
                        ev.Content
                    )
                );
            
            sw.Stop();
            Console.WriteLine("convert events took: " + sw.ElapsedMilliseconds);

            return events;
        }

        public async Task Raise(string eventName, object content)
        {
            var contentJson = JsonConvert.SerializeObject(content);
            
            // Maps OccurredAt and EventName to metadata to be stored along with the event
            var metaDataJson = JsonConvert.SerializeObject(new EventMetadata
            {
                OccurredAt = DateTimeOffset.Now,
                EventName = eventName
            });
            
            // EventData is EventStore's representation of an event
            var eventData = new EventData(
                Guid.NewGuid(),
                "ShoppingCartEvent",
                true,
                Encoding.UTF8.GetBytes(contentJson),
                Encoding.UTF8.GetBytes(metaDataJson)
            );
            
            // Writes the event to EventStore
            await _connection.AppendToStreamAsync(ShoppingCartStream, ExpectedVersion.Any, eventData).ConfigureAwait(false);
        }
        
        private class EventMetadata
        {
            public DateTimeOffset OccurredAt { get; set; }
            public string EventName { get; set; }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}