using Confluent.Kafka;
using Handler;
using Microsoft.Extensions.Configuration;
using Model;
using Service;
using System.Text.Json;

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

consumer.Subscribe(topic);

Console.WriteLine($"Subscribed to topic: {topic}");
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

try
{
    while (true)
    {
        var result = consumer.Consume(CancellationToken.None);
        Console.WriteLine(result.Message.Value);

        try
        {
            var message = JsonSerializer.Deserialize<Message<Customer>>(result.Message.Value, deserializeOptions);
            if (message is not null)
            {
                CustomerHandler.HandleChange(message);
                consumer.Commit(result);
            }
            else
            {
                throw new Exception("Deserialization of message returned null.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error processing message: {e.Message}");
            Console.WriteLine("Sending to dead letter topic...");

            await DeadLetterQueue.SendAsync(result.Message.Value, e.Message, topic);
        }
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation canceled.");
}