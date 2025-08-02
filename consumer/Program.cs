using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

var appConfig = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

string? eventServer = appConfig["EventServer"];
string? consumerGroup = appConfig["ConsumerGroup"];
string? topic = appConfig["Topic"];

if (eventServer == null || consumerGroup == null || topic == null)
{
    throw new Exception($"Missing configuration variable. EventServer={eventServer}. ConsumerGroup={consumerGroup}. Topic={topic}");
}

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = eventServer,
    GroupId = consumerGroup,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

consumer.Subscribe(topic);

Console.WriteLine($"Subscribed to topic: {topic}");
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    consumer.Close();
    Console.WriteLine("Consumer closed.");
};

try
{
    while (true)
    {
        var result = consumer.Consume(CancellationToken.None);
        Console.WriteLine($"[{result.TopicPartitionOffset}] {result.Message.Value}");
        consumer.Commit(result);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation canceled.");
}