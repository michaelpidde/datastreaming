using System.Text.Json;
using Confluent.Kafka;

namespace Shared;

public static class KafkaMessageProcessor
{
    public static async Task ProcessLoopAsync<T>(
    IConsumer<Ignore, string> consumer,
    string topic,
    JsonSerializerOptions options,
    Func<Message<T>, Task> handleMessageAsync,
    Func<string, string, string, Task>? handleDeadLetterAsync = null,
    CancellationToken cancellationToken = default)
    {
        consumer.Subscribe(topic);
        Console.WriteLine($"Subscribed to topic: {topic}");
        
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = consumer.Consume(cancellationToken);
                Console.WriteLine(result.Message.Value);
                
                try
                {
                    var message = JsonSerializer.Deserialize<Message<T>>(result.Message.Value, options);
                    if (message is not null)
                    {
                        await handleMessageAsync(message);
                        consumer.Commit(result);
                    }
                    else
                    {
                        throw new Exception("Deserialization of message returned null.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    if (handleDeadLetterAsync is not null)
                    {
                        await handleDeadLetterAsync(result.Message.Value, ex.Message, topic);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled.");
        }
        finally
        {
            consumer.Close();
        }
    }
}
