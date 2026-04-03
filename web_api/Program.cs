using web_api;
class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapPost("/api/data", (MessageData data) =>
        { 
            var message = data.Text.StartsWith("/") ? data.Text : "/" + data.Text;

            var result = ControllerRequest.GetContent(message);
            return Results.Ok(result);
        });

        app.Run("http://localhost:5000");

    }
    public record MessageData(string Text);
}