using Confluent.Kafka;
using Shared.Contracts.Events;
using System.Text.Json;

namespace Products_service.Services.Kafka
// Kafka logiquqe : We have two actors in every pip event => producer service and consumer service
// Evey event is described by a (topic) for ex => update-prod-event, and the topic instance multiple partitions
// like the class and the object, for ex in our systeme we are going to work in products, kafka put every updated product 
// in a specefice partition, based on prod id, so evey request coming from prod-id = 1, is filled in partition1
// ex : prod-id = 1, price = 120 => in partition1 {offset0 : id = 1, price = 120}, next request prod-id = 1, price = 130 => 
// in partition1 {offset0 : id = 1, price = 120 / offset1 : id = 1, price = 130}, at the end the partition is filled 
// in a (broker)
{
    public class KakfaService: IKafkaService
    {
        private IProducer<string, string> producer;
        private string topic;
        public KakfaService(IConfiguration config)
        {
            var kafkaConfig = new ProducerConfig()
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };
            producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
            topic = config["Kafka:ProductTopic"];
        }

        public async Task UpdatedProductPub(ProductUpdatedEvent ev)
        {
            var message = new Message<string, string>()
            {
                Key = ev.ProductId.ToString(),
                Value = JsonSerializer.Serialize(ev)
            };

            await producer.ProduceAsync(topic, message);
        }
    }
}
