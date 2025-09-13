using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

public class RamHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;
    private readonly double _maxRamMB;

    public RamHealthCheck(IConfiguration config)
    {
        _config = config;
        _maxRamMB = config.GetValue("HealthCheckSettings:Ram:MaxRamMB", 512.0);
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var process = Process.GetCurrentProcess();
        double ramMB = process.WorkingSet64 / (1024.0 * 1024.0);

        string result = $"RAM usage: {ramMB:F1} MB";
        if (ramMB > _maxRamMB) return Task.FromResult(HealthCheckResult.Unhealthy(result));
        return Task.FromResult(HealthCheckResult.Healthy(result));
    }
}