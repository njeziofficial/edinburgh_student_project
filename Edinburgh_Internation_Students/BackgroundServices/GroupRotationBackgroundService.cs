using Edinburgh_Internation_Students.Configuration;
using Edinburgh_Internation_Students.Services;

namespace Edinburgh_Internation_Students.BackgroundServices;

public class GroupRotationBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<GroupRotationBackgroundService> logger,
    GroupRotationSettings settings) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!settings.Enabled)
        {
            logger.LogInformation("Group rotation is disabled in configuration");
            return;
        }

        var interval = TimeSpan.FromHours(settings.IntervalInHours) + TimeSpan.FromMinutes(settings.IntervalInMinutes);
        
        logger.LogInformation("Group rotation background service started. Running every {Interval}", interval);

        // Run on startup if configured
        if (settings.RunOnStartup)
        {
            logger.LogInformation("Running initial group rotation on startup...");
            await PerformRotationAsync(stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                await PerformRotationAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Group rotation service is stopping");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in group rotation background service");
                // Continue running despite errors
            }
        }

        logger.LogInformation("Group rotation background service stopped");
    }

    private async Task PerformRotationAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var groupService = scope.ServiceProvider.GetRequiredService<IGroupService>();

        try
        {
            logger.LogInformation("Starting group member rotation at {Time}", DateTime.UtcNow);

            await groupService.ShuffleGroupMembersAsync();

            logger.LogInformation("Group member rotation completed successfully at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during group rotation at {Time}", DateTime.UtcNow);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Group rotation background service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
