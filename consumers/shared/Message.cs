namespace Shared;

public class Message<T>
{
    public T? Before { get; set; }
    public T? After { get; set; }
    public string? Op { get; set; }
}