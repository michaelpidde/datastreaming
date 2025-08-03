using Model;
using Service;

namespace Handler;

public static class CustomerHandler
{
    public static void HandleChange(Message<Customer> message)
    {
        if (message.Op == "u" && message.Before!.Active != message.After!.Active && message.After!.Active == false)
        {
            CancelOrders(message.After!.Id);
        }
        // Do not handle create op - there are no orders to manage when a customer is added
        // Do not handle delete op - customer records are handled via Active flag
        // Do not handle Active turning back to True - previously cancelled orders do not need to be reactivated
    }

    public static void CancelOrders(long customerId)
    {
        Console.WriteLine($"Cancelling orders for customerId {customerId}...");
        Database.CancelCustomerOrders(customerId);
    }
}