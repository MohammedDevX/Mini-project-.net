using Shared.Contracts.Events;

namespace Products_service.Services.Kafka
{
    public interface IKafkaService
    {
        public Task UpdatedProductPub(ProductUpdatedEvent ev);
    }
}
