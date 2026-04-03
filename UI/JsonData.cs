using System.Text;
using System.Text.Json;

namespace UI
{
    internal class JsonData
    {
        public string? Status { get; set; }
        public string? Time { get; set; }
        public string? Info { get; set; }
        public string? Error { get; set; }
        public JsonData() { }
        public JsonData(JsonElement item)
        { 
            var properties = typeof(JsonData).GetProperties();

            foreach (var property in properties)
            {
                // проверка на возможность записи
                if (property.CanWrite)
                {
                    if (item.TryGetProperty(property.Name, out var jsonValue))
                    {
                        property.SetValue(this, jsonValue.GetString());
                    }
                }

                
            }
        }

        public override string ToString()
        {
            StringBuilder text = new();
            var properties = typeof(JsonData).GetProperties();

            foreach(var property in properties)
            {
                if (property.CanRead)
                {
                    var value = property.GetValue(this) ?? "null";
                    text.AppendLine($"{property.Name}: {value}");
                }
            }
            return text.ToString();
        }

    }

}
