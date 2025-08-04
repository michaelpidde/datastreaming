using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Order;

public static class Database
{
    private static string _connection = "";
    
    public static void Initialize(IConfiguration appConfig)
    {
        string? connection = appConfig["ConnectionString"];
        
        if (connection == null)
        {
            throw new Exception($"Missing Database configuration variable. ConnectionString={connection}.");
        }
        
        _connection = connection;
    }
    
    public static void UpdateOrderRollup(string op, long customerId, long productId, int count)
    {
        string query = $"update order_rollup set openOrdersInventoryCount = openOrdersInventoryCount {op} @count, openOrders = openOrders - 1 where customerId = @customerId and productId = @productId";
        using var connection = new SqlConnection(_connection);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@count", count);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@productId", productId);
        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"{rowsAffected} order rollup rows updated for customerId {customerId}, productId {productId}.");
    }
    
    public static void UpdateInventoryRollup(string op, long productId, int count)
    {
        string query = $"update inventory_rollup set openOrdersInventoryCount = openOrdersInventoryCount {op} @count where productId = @productId";
        using var connection = new SqlConnection(_connection);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@count", count);
        command.Parameters.AddWithValue("@productId", productId);
        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"{rowsAffected} order rollup rows updated for productId {productId}.");
    }
}
