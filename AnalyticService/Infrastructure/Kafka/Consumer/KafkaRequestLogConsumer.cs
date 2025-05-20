using AnalyticService.Contracts.Services;
using Chr.Avro.Confluent;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Repositories;

namespace AnalyticService.Infrastructure.Kafka.Consumer;

public class KafkaRequestLogBatchConsumer : IRequestLogConsumer
{
    private readonly IRequestLogRepository _repository;
    private readonly IConsumer<string, HttpRequestLog> _consumer;
    private const string Topic = "http-request-logs";
    
    public KafkaRequestLogBatchConsumer(
        IRequestLogRepository logRepository,
        ISchemaRegistryClient schemaRegistryClient)
    {
        _repository = logRepository;

        var config = new ConsumerConfig
        {
            // BootstrapServers = "analytic-kafka:9092",
            BootstrapServers = "localhost:9092",
            GroupId = "http-request-logs",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var avroDeserializer = new AsyncSchemaRegistryDeserializer<HttpRequestLog>(schemaRegistryClient).AsSyncOverAsync();

        _consumer = new ConsumerBuilder<string, HttpRequestLog>(config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(avroDeserializer)
            .Build();

        _consumer.Subscribe(Topic);
    }
    
    public async Task ConsumeBatchAsync(int maxMessages, CancellationToken cancellationToken = default)
    {
        var messages = new List<ConsumeResult<string, HttpRequestLog>>();

        while (messages.Count < maxMessages && !cancellationToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(TimeSpan.FromMilliseconds(200));
            if (result != null)
            {
                messages.Add(result);
            }
            else
            {
                break; // нет больше сообщений
            }
        }

        if (messages.Count == 0)
            return;

        var logs = messages.Select(r => r.Message.Value).ToList();

        // обработка — сохраняем в ClickHouse или др.
        await _repository.SaveRequestsAsync(logs, cancellationToken);

        // ручной коммит — за каждый partition последний оффсет
        var offsets = messages
            .GroupBy(r => r.TopicPartition)
            .Select(g => 
                new TopicPartitionOffset(g.Key, g.Max(r => r.Offset) + 1));

        _consumer.Commit(offsets);
    }
}