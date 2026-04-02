using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace web_api
{
    public class SQLController
    {
        // Создал потому что просто Exception = null выглидит сомнительно 
        public class StatusResult
        {
            public Exception? Exception { get; set; }
            public bool Status { get; set; }
            public string? Content { get; set; }
        }

        private static SqlConnection? _connection;
        private static readonly string _connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WEB_API;Integrated Security=True;TrustServerCertificate=True;";
        public static readonly string ErrorConnectionNull = "Ошибка: Соединение не установлено.";

        public static StatusResult GetMsSqlVersion()
        {
            try
            {
                if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
                {
                    Exception exception = new(ErrorConnectionNull);
                    return new StatusResult { Exception = exception, Status = false};
                }
                
                var command = new SqlCommand("SELECT @@VERSION", _connection);
                return  new StatusResult { Content = command.ExecuteScalar()?.ToString() ?? "Unknown", Status = true };
            }
            catch (Exception ex)
            {
                return new StatusResult { Exception = ex, Status = false };
            }
        }
        static public StatusResult ConnectDB()
        {
            try
            {
                if(_connection == null)
                {
                    _connection = new SqlConnection(_connString);
                }

                if (_connection.ConnectionString == string.Empty)
                {
                    _connection.ConnectionString = _connString;
                }

                if (_connection.State == ConnectionState.Open)
                {
                    return new StatusResult { Status = true };
                }
                
                _connection.Open();
                return  new StatusResult  {Status = true }; 
            }
            catch (Exception ex)
            {
                return  new StatusResult { Status = false, Exception = ex }; 
            }
        }

        static public StatusResult GetStatus()
        {
            try
            {
                if(_connection == null)
                {
                    Exception ex = new("Подключение ещё не совершено");
                    return new StatusResult { Status = false, Exception = ex };
                }
                
                if (_connection.State == ConnectionState.Open)
                {
                    return new StatusResult { Status = true };
                }

                else
                {
                    Exception ex = new("Состояние не активное");
                    return new StatusResult { Status = false, Exception = ex };
                }
                
            }
            catch(Exception ex)
            {
                return new StatusResult { Status = false, Exception = ex };
            }
        }
        static public StatusResult DisconnectBD()
        {
            try
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
                return new StatusResult { Status = true };
            }
            catch (Exception ex)
            {
                return new StatusResult { Status = false, Exception = ex };
            }
        }


    }
}
