using System.Text.Json;
using System.Text.Json.Serialization;

using Comments.Api.ExceptionHandlers;
using Comments.Application;
using Comments.Infrastructure;

using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var uploadsPath = builder.Configuration["Uploads:Path"]
    ?? throw new InvalidOperationException("'Uploads:Path' is not configured");
var uploadsAbsolutePath = Path.GetFullPath(uploadsPath);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            );
            o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        }
);
builder.Services.ConfigureHttpJsonOptions(o =>
    {
        o.SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );
        o.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    }
);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

await app.Services.ApplyMigrationsAsync();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

Directory.CreateDirectory(uploadsAbsolutePath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsAbsolutePath),
    RequestPath = "/uploads",
});
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");
app.MapFallback("/api/{**path}", () => Results.NotFound());

app.Run();
