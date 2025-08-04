namespace Order;

public class Order {
    public required long Id { get; set; }
    public required long CustomerId { get; set; }
    public required long ProductId { get; set; }
    public required int Count { get; set; }
    public required long Created { get; set; }
    public long? Fulfilled { get; set; }
    public long? Cancelled { get; set; }
}