using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace LoyaltyProgramEventConsumer
{
    public class EventSubscriberService : BackgroundService
    { 
        private long _start = 0;
        private readonly EventSubscriberSettings _settings;
        private readonly ILogger<EventSubscriberService> _logger;

        public EventSubscriberService(IOptions<EventSubscriberSettings> settings,
            ILogger<EventSubscriberService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _logger.LogDebug("SpecialOffers url: {Url}", _settings.SpecialOffersUrl);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("EventSubscriberService is starting");

            stoppingToken.Register(() => _logger.LogDebug("EventSubscriberService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"EventSubscriber task doing background work.");

                await SubscriptionCycleCallback(stoppingToken);
                
                await Task.Delay(_settings.CheckInterval, stoppingToken);
            }

            _logger.LogDebug("EventSubscriber background task is stopping.");
        }
        
        private async Task SubscriptionCycleCallback(CancellationToken stoppingToken)
        {
            // Awaits the HTTP GET to the event feed
            var response = await ReadEvents(stoppingToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
                HandleEvents(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> ReadEvents(CancellationToken stoppingToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_settings.SpecialOffersUrl);
                
                // Awaits getting new events
                // Uses query parameters to limit the number of events read
                var response = await httpClient.GetAsync($"/api/events/?start={_start}&end={_start + _settings.ChunkSize}", stoppingToken)
                    .ConfigureAwait(false);
                
                return response;
            }
        }

        private void HandleEvents(string content)
        {
            var events = JsonConvert.DeserializeObject<IEnumerable<SpecialOfferEvent>>(content);
            foreach (var ev in events)
            {
                // Treats the content property as a dynamic object
                dynamic eventData = ev.Content;
                
                _logger.LogInformation("Event [{Id}]: {EventData}", ev.SequenceNumber, (object)eventData);

                // Keeps tracks of the highest event number handled
                _start = Math.Max(_start, ev.SequenceNumber + 1);
            }
        }
    }
    
    public struct SpecialOfferEvent
    {
        public long SequenceNumber { get; set; }
        public string Name { get; set; }
        public object Content { get; set; }
    }
}
