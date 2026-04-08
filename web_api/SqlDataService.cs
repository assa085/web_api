using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using System.Data;

namespace web_api
{
    public class SqlDataService : IDisposable
    {
        private readonly string _connString;
        private SqlConnection? _connection;

        public SqlDataService(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString("DefaultConnection")
                          ?? throw new InvalidOperationException("Conn string not found");
        }

        public JsonArray ExecuteCommand(string message)
        {
            var routes = new Dictionary<string, Func<object>>
            {
                ["/status"] = GetStatus,
                ["/version"] = GetSqlVersion,
                ["/connect"] = Connect 
            };

            var jsonArray = new JsonArray();
            if (routes.TryGetValue(message, out var func))
            {
                jsonArray.Add(JsonSerializer.SerializeToNode(func()));
            }
            else
            {
                jsonArray.Add(new JsonObject { ["Error"] = "Not Found" });
            }
            return jsonArray;
        }

        private object GetSqlVersion()
        {
            try
            {
                EnsureConnected();
                using var command = new SqlCommand("SELECT @@VERSION", _connection);
                return new { Status = "OK", Info = command.ExecuteScalar()?.ToString() };
            }
            catch (Exception ex) { return new { Status = "ERROR", Error = ex.Message }; }
        }

        private object GetStatus()
        {
            return new { Status = "OK", State = _connection?.State.ToString() ?? "Closed" };
        }

        private object Connect()
        {
            try { 
                EnsureConnected();
                return new { Status = "OK" };
            }
            catch (Exception ex) 
            { 
                return new { Status = "ERROR", Error = ex.Message }; 
            }
        }

        private void EnsureConnected()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connString);
            }
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }          
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
