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
else
{
    builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

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

app.UseAuthorization();

app.MapControllers();

app.Run();
