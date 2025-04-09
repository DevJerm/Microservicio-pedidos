// Infrastructure/Messaging/KafkaProducer.cs
using Confluent.Kafka;
using System.Text.Json;
using pedidos_service.Application.Interfaces;
public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["KAFKA_BOOTSTRAP_SERVERS"] ?? "kafka:9092"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);

        var kafkaMessage = new Message<Null, string> { Value = json };

        var result = await _producer.ProduceAsync(topic, kafkaMessage);

        Console.WriteLine($"✅ Mensaje enviado a Kafka. Topic: {result.Topic}, Offset: {result.Offset}");
    }
}
