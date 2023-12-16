using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using OperationService.Business.Abstractions.Kafka;
using OperationService.Shared.DTO.Kafka;
using System.Text.Json;

namespace OperationService.Business.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(IConfiguration configuration)
        {
            _configuration = configuration;
            _producer = new ProducerBuilder<string, string>(new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
            }).Build();
        }

        public async Task ProducePostAsync(SendPostKafka sendPostKafka)
        {
            var topic = _configuration["Kafka:SendPost:Topic"];
            var value = JsonSerializer.Serialize(sendPostKafka);

            await _producer.ProduceAsync(topic, new Message<string, string> { Value = value });
        }

        public async Task ProduceNotificationAsync(SendNotificationKafka sendNotificationKafka)
        {
            var topic = _configuration["Kafka:SendNotification:Topic"];
            var value = JsonSerializer.Serialize(sendNotificationKafka);

            await _producer.ProduceAsync(topic, new Message<string, string> { Value = value });
        }
    }
}