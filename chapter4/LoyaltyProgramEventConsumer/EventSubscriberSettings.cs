namespace LoyaltyProgramEventConsumer
{
    public class EventSubscriberSettings
    {
        public string SpecialOffersUrl { get; set; }
        public int CheckInterval { get; set; }
        public int ChunkSize { get; set; } = 100;
    }
}
