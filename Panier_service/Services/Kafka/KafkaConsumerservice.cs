
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Panier_service.Data;
using Shared.Contracts.Events;
using System.Text.Json;

namespace Panier_service.Services.Kafka
{
    // N.B : A singleton service shouldnt depend to a scooped service 
    public class KafkaConsumerservice: BackgroundService
    {
        private IServiceScopeFactory factory;
        private IConfiguration config;
        public KafkaConsumerservice(IServiceScopeFactory factory, IConfiguration config)
        {
            this.factory = factory;
            this.config = config;
        }

        // CancellationToken : token generated whene the BackgroundService is runned 
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Configuration pret pour creer un consumer
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"], // Broker server endpoint
                GroupId = config["Kafka:GroupId"], // Kafka use GroupId consumer to know wish partitions give to the 
                // consumer
                AutoOffsetReset = AutoOffsetReset.Earliest // Lire depuis le debut de partition
            };

            // Here using builder designe pattern
            var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            // Listening to product-events topic
            consumer.Subscribe(config["Kafka:ProductTopic"]);

            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Pass the stoppingToken to the consumer, so whene the token is disabled thats meen the program 
                        // is stopped, so the consumer also stop listening events
                        var result = consumer.Consume(stoppingToken);

                        // Deserialise the string event message
                        var evt = JsonSerializer.Deserialize<ProductUpdatedEvent>(
                            result.Message.Value
                        );

                        // Using the factory service to instance scooped services => AppDbContext here in this 
                        // singloton service
                        using var scope = factory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        context.Panier
                            .Where(p => p.Id_produit == evt.ProductId)
                            .ExecuteUpdate(set =>
                                set.SetProperty(p => p.Prix, evt.NewPrice)
                            );

                        // Notice kafka that the event is handled
                        consumer.Commit(result);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error dans kafka: {ex.Message}");
                    }
                }
            }, stoppingToken);
        }
    }
}
