using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

public class CpuHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;
    private readonly double _maxCpuPercent;

    public CpuHealthCheck(IConfiguration config)
    {
        _config = config;
        _maxCpuPercent = config.GetValue("HealthCheckSettings:Cpu:MaxCpuPercent", 80.0);
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var percent = GetCpuUsage();

        string result = $"CPU usage: {percent}%";
        if (percent > _maxCpuPercent) return Task.FromResult(HealthCheckResult.Unhealthy(result));
        return Task.FromResult(HealthCheckResult.Healthy(result));
    }

    private static double GetCpuUsage()
    {
        var process = Process.GetCurrentProcess();
        var startTime = DateTime.UtcNow;
        var startCpu = process.TotalProcessorTime;
        Thread.Sleep(500); // measure briefly
        var endTime = DateTime.UtcNow;
        var endCpu = process.TotalProcessorTime;

        double cpuUsedMs = (endCpu - startCpu).TotalMilliseconds;
        double totalMsPassed = (endTime - startTime).TotalMilliseconds;
        double cpuUsage = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;
        return Math.Round(cpuUsage, 2);
    }
}