using web_api;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddScoped<SqlDataService>();

var app = builder.Build();

app.MapPost("/api/data", (Message data, SqlDataService sqlService) =>
{
    if (data.Text == string.Empty)
    {
        return Results.BadRequest("Text is required");
    }
    var command = data.Text.StartsWith("/") ? data.Text : "/" + data.Text;

    var result = sqlService.ExecuteCommand(command);
    return Results.Ok(result);
});

app.Run();
public record Message(string? Text);

