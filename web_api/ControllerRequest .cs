using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Timers;

namespace web_api
{
    public static class ControllerRequest
    {

        private static readonly Dictionary<string, Func<object>> Routes = new()
        {
            ["/status"] = () => GetStatus(),
            ["/get_SQL_version"] = () => GetSQLVersion(),
            ["/connect_db"] = () => ConnectDB(),
            ["/disconnect_db"] = () => DisconnectBD(),
        };
        public static JsonArray GetContent(string message)
        {
            var jsonArray = new JsonArray();
            if (Routes.TryGetValue(message, out var func))
            {
                jsonArray.Add(JsonSerializer.SerializeToNode(func()));
            }
            else
            {
                jsonArray.Add(new JsonObject { ["Error"] = "Not Found" });
            }

            return jsonArray;
        }
        static object GetSQLVersion()
        {
            var status = SQLController.GetMsSqlVersion();
            if(!status.Status)
            {
                return new { 
                    Status = "ERROR", 
                    Info = "Connset SQL DB", 
                    Error = status.Exception?.ToString() ?? "Status = null",
                    Time = GetTime() 
                };
            }
            return new
            {
                Status = "OK",
                Info = "SQL version: " + status.Content,
                Time = GetTime()
            };

        }
        static object DisconnectBD()
        {
            var statucConnect = SQLController.DisconnectBD();
            if (statucConnect.Status)
            {
                return new { Status = "OK", Info = "Disconnset SQL DB ", Time = GetTime() };
            }
            else
            {
                var errorStr = statucConnect.Exception?.Message; 
                return new { Status = "ERROR", Info = "Disconnset SQL DB", Error = errorStr, Time = GetTime() };
            }

        }
        static object ConnectDB()
        {
            var statucConnect = SQLController.ConnectDB();
            if (statucConnect.Status)
            {
                return new { Status = "OK", Info = "Connset SQL DB", Time = GetTime() };
            }
            else
            {
                var errorStr = statucConnect.Exception?.Message ?? "Message = null, unknown error";
                return new { Status = "ERROR", Info = "Connset SQL DB", Error = errorStr, Time = GetTime() };
            }
        } 
        static object GetStatus()
        {
            var statuc = SQLController.GetStatus();
            if (statuc.Status)
            {
                return new { Status = "OK", Info = "Get status", Time = GetTime() };
            }
            else
            {
                return new
                {
                    Status = "ERROR",
                    Info = "Get status",
                    Error = statuc.Exception?.ToString() ?? "Status = null",
                    Time = GetTime()
                };
            }
        }
        static string GetTime() => DateTime.Now.ToString("HH:mm:ss");
    }
}
