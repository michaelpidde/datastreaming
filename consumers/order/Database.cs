using System.Linq.Expressions;
using System.Transactions;
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
    
    private static async Task<byte[]?> GetOrderRollupVersion(SqlConnection connection, long customerId, long productId)
    {
        string query = "select [rowVersion] from order_rollup where customerId = @customerId and productId = @productId";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@productId", productId);
        await using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return (byte[])reader["rowVersion"];
        }
        
        return null;
    }
    
    public static async Task UpdateOrderRollup(string op, long customerId, long productId, int count)
    {
        // TODO: If customer+product combination does not exist, insert it.

        string query = $"""
                        update r set openOrdersInventoryCount = openOrdersInventoryCount {op} @count, openOrders = openOrders - 1
                        from order_rollup r
                        where customerId = @customerId and productId = @productId and rowVersion = @rowVersion
                        """;
        await using var connection = new SqlConnection(_connection);
        connection.Open();

        int retryCount = 3;
        while (retryCount-- > 0)
        {
            byte[]? rowVersion = await GetOrderRollupVersion(connection, customerId, productId);
            
            await using var transaction = connection.BeginTransaction();
            await using var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@count", count);
            command.Parameters.AddWithValue("@customerId", customerId);
            command.Parameters.AddWithValue("@productId", productId);
            command.Parameters.AddWithValue("@rowVersion", rowVersion);
            
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                transaction.Commit();
                if(rowsAffected == 1) {
                    Console.WriteLine($"{rowsAffected} order rollup rows updated for customerId {customerId}, productId {productId}.");
                } else {
                    transaction.Rollback();
                    Console.WriteLine($"No order rollup rows updated for customerId {customerId}, productId {productId} using rowVersion. Retrying...");
                    continue;
                }
                
                break;
            }
            // Catch Deadlock exception
            catch (SqlException e) when (e.Number == 1205)
            {
                transaction.Rollback();

                if (retryCount == 0)
                {
                    throw;
                }

                Console.WriteLine("Caught deadlock exception - retrying...");

                await Task.Delay(200);
            }
            catch (SqlException e)
            {
                transaction.Rollback();
                Console.WriteLine($"SQL Exception number {e.Number}");
                throw;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    private static async Task<byte[]?> GetInventoryRollupVersion(SqlConnection connection, long productId)
    {
        string query = "select [rowVersion] from inventory_rollup where productId = @productId";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@productId", productId);
        await using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return (byte[])reader["rowVersion"];
        }
        
        return null;
    }
    
    public static async Task UpdateInventoryRollup(string op, long productId, int count) {
        string query = $"""
                        update r set openOrdersInventoryCount = openOrdersInventoryCount {op} @count
                        from inventory_rollup r with (rowlock, updlock)
                        where productId = @productId
                        """;
        
        await using var connection = new SqlConnection(_connection);
        connection.Open();
        
        int retryCount = 3;
        while(retryCount-- > 0) {
            byte[]? rowVersion = await GetInventoryRollupVersion(connection, productId);
            
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@count", count);
            command.Parameters.AddWithValue("@productId", productId);
            command.Parameters.AddWithValue("@rowVersion", rowVersion);
            await using var transaction = connection.BeginTransaction();
            try
            {
                command.Transaction = transaction;
                int rowsAffected = command.ExecuteNonQuery();
                transaction.Commit();
                if(rowsAffected == 1) {
                    Console.WriteLine($"{rowsAffected} inventory rollup rows updated for productId {productId}.");
                } else {
                    transaction.Rollback();
                    Console.WriteLine($"No inventory rollup rows updated for productId {productId} using rowVersion. Retrying...");
                    continue;
                }
                
                break;
            }
            // Catch Deadlock exception
            catch (SqlException e) when (e.Number == 1205)
            {
                transaction.Rollback();

                if (retryCount == 0)
                {
                    throw;
                }

                Console.WriteLine("Caught deadlock exception - retrying...");

                await Task.Delay(200);
            }
            catch (SqlException e)
            {
                transaction.Rollback();
                Console.WriteLine($"SQL Exception number {e.Number}");
                throw;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
    }
}
