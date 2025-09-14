using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Data;
using Todo.Api.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite("Data Source=todo_dev.db"));
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else if (builder.Environment.IsEnvironment("Testing"))
{
    // Testing environment is handled by the testing framework
}
else
{
    throw new Exception($"Unknown environment {builder.Environment.EnvironmentName}. Cannot configure database.");
}

var origins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOriginsPolicy", policy =>
    {
        policy.WithOrigins(origins ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ITaskQueryEngine, TaskQueryEngine>();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<TodoDbContext>("DbContext")
    .AddCheck<CpuHealthCheck>("CPU")
    .AddCheck<RamHealthCheck>("RAM")
    .AddCheck<DiskSpaceHealthCheck>("Disk Space");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    //db.Database.Migrate();
    db.Database.EnsureCreated();
}

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(e => new {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowedOriginsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();

// For Accessibility Testing
public partial class Program { }