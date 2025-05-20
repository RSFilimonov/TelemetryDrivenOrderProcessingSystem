using Chr.Avro.Confluent;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Order.Application.Contracts.Producer;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;

namespace Order.Infrastructure.Kafka.Producer;

public class KafkaRequestLogProducer : IRequestLogProducer
{
    private readonly IProducer<string, HttpRequestLog> _producer;
    private const string Topic = "http-request-logs";

    public KafkaRequestLogProducer(ISchemaRegistryClient schemaRegistryClient)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };
        
        var producerBuilder = new ProducerBuilder<string, HttpRequestLog>(producerConfig)
            .SetAvroKeySerializer(schemaRegistryClient, registerAutomatically: AutomaticRegistrationBehavior.Always)
            .SetAvroValueSerializer(schemaRegistryClient, registerAutomatically: AutomaticRegistrationBehavior.Always);

        _producer = producerBuilder.Build();
    }

    public async Task SendAsync(HttpRequestLog log, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, HttpRequestLog>
        {
            Key = log.ClientIp,
            Value = log
        };

        await _producer.ProduceAsync(Topic, message, cancellationToken);
    }
}