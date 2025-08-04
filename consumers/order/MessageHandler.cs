using Shared;

namespace Order;

public static class MessageHandler
{
    private const string Increment = "+";
    private const string Decrement = "-";
    
    public static Task HandleChange(Message<Order> message)
    {
        if (message.Op == "u" && message.Before!.Cancelled != message.After!.Cancelled && message.After!.Cancelled != null)
        {
            UpdateOrderRollup(Decrement, message.After!.CustomerId, message.After!.ProductId, message.After!.Count);
            UpdateInventoryRollup(Decrement, message.After!.ProductId, message.After!.Count);
        }
        
        if(message.Op == "c") {
            UpdateOrderRollup(Increment, message.After!.CustomerId, message.After!.ProductId, message.After!.Count);
            UpdateInventoryRollup(Increment, message.After!.ProductId, message.After!.Count);
        }
        
        // Do not handle delete op - order records are cancelled, not deleted
        // Do not handle Cancelled turning back to null - previously cancelled orders cannot be reactivated
        
        return Task.CompletedTask;
    }
    
    private static void UpdateOrderRollup(string op, long customerId, long productId, int count)
    {
        Console.WriteLine($"Updating order rollup for customerId {customerId}, productId {productId}...");
        Database.UpdateOrderRollup(op, customerId, productId, count);
    }
    
    private static void UpdateInventoryRollup(string op, long productId, int count) {
        Console.WriteLine($"Updating inventory rollup for productId {productId}...");
        Database.UpdateInventoryRollup(op, productId, count);
    }
}
