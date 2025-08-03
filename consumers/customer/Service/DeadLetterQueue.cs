using System.Text.Json;
using Confluent.Kafka;

namespace Service;

public static class DeadLetterQueue
{
    private static IProducer<Null, string>? _producer;
    private static string? _topic;

    public static void Initialize(string server, string topic)
    {
        _topic = topic;
        var config = new ProducerConfig { BootstrapServers = server };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public static async Task SendAsync(string message, string error, string originalTopic)
    {
        if (_producer == null || _topic == null) return;

        var payload = new Message<Null, string>
        {
            Value = message,
            Headers = new Headers
            {
                { "error", System.Text.Encoding.UTF8.GetBytes(error) },
                { "original-topic", System.Text.Encoding.UTF8.GetBytes(originalTopic) },
                { "timestamp", BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) }
            }
        };
        await _producer.ProduceAsync(_topic, payload);
    }
}