using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Customer;

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

    public static void CancelCustomerOrders(long customerId)
    {
        string query = $"update [order] set cancelled = getdate() where customerId = @customerId and cancelled is null and fulfilled is null";
        using var connection = new SqlConnection(_connection);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"Cancelled {rowsAffected} orders for customerId {customerId}.");
    }
}