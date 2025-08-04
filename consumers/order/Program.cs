using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Order;
using Shared;

var appConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

string? eventServer = appConfig["EventServer"];
string? consumerGroup = appConfig["ConsumerGroup"];
string? topic = appConfig["Topic"];
string deadLetterTopic = topic + "-dlq";

if (eventServer == null || consumerGroup == null || topic == null)
{
    throw new Exception($"Missing Kafka configuration variable. EventServer={eventServer}. ConsumerGroup={consumerGroup}. Topic={topic}");
}

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = eventServer,
    GroupId = consumerGroup,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,
};

using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
DeadLetterQueue.Initialize(eventServer, deadLetterTopic);
Database.Initialize(appConfig);

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    consumer.Close();
    Console.WriteLine("Consumer closed.");
};

var deserializeOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

await KafkaMessageProcessor.ProcessLoopAsync<Order.Order>(
    consumer,
    topic,
    deserializeOptions,
    MessageHandler.HandleChange,
    DeadLetterQueue.SendAsync // optional
);
