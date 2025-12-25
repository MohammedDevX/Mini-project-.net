namespace Shared.Contracts.Events
{
    public class ProductUpdatedEvent
    {
        public int ProductId { get; set; }
        public double NewPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}