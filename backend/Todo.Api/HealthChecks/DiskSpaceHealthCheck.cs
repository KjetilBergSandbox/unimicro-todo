using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;
    private readonly double _minFreePercent;

    public DiskSpaceHealthCheck(IConfiguration config)
    {
        _config = config;
        _minFreePercent = config.GetValue("HealthCheckSettings:DiskSpace:MinFreePercent", 10.0);
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        double free;
        try
        {
            DriveInfo drive = new DriveInfo(Path.GetPathRoot("/")!);
            free = (drive.AvailableFreeSpace / (double)drive.TotalSize) * 100;
        }
        catch
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Disk space free: N/A"));
        }

        string result = $"Disk space free: {free:F1}%";
        if (free < _minFreePercent) return Task.FromResult(HealthCheckResult.Unhealthy(result));
        return Task.FromResult(HealthCheckResult.Healthy(result));
    }
}