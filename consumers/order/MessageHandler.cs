using Shared;

namespace Order;

public static class MessageHandler
{
    public static async Task HandleChange(Message<Order> message)
    {
        if (message.Op == Constants.Update && message.Before!.Cancelled != message.After!.Cancelled && message.After!.Cancelled != null)
        {
            UpdateOrderRollup(Constants.Decrement, message.After!.CustomerId, message.After!.ProductId, message.After!.Count);
            await UpdateInventoryRollup(Constants.Decrement, message.After!.ProductId, message.After!.Count);
        }
        
        if (message.Op == Constants.Create)
        {
            UpdateOrderRollup(Constants.Increment, message.After!.CustomerId, message.After!.ProductId, message.After!.Count);
            await UpdateInventoryRollup(Constants.Increment, message.After!.ProductId, message.After!.Count);
        }
        
        // Do not handle delete op - order records are cancelled, not deleted
        // Do not handle Cancelled turning back to null - previously cancelled orders cannot be reactivated
    }
    
    private static void UpdateOrderRollup(string op, long customerId, long productId, int count)
    {
        Console.WriteLine($"Updating order rollup for customerId {customerId}, productId {productId}...");
        Database.UpdateOrderRollup(op, customerId, productId, count);
    }
    
    private static async Task UpdateInventoryRollup(string op, long productId, int count)
    {
        Console.WriteLine($"Updating inventory rollup for productId {productId}...");
        await Database.UpdateInventoryRollup(op, productId, count);
    }
}
