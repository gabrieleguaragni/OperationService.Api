using OperationService.Shared.DTO.Kafka;

namespace OperationService.Business.Abstractions.Kafka
{
    public interface IKafkaProducerService
    {
        public Task ProducePostAsync(SendPostKafka sendPostKafka);
        public Task ProduceNotificationAsync(SendNotificationKafka sendNotificationKafka);
    }
}