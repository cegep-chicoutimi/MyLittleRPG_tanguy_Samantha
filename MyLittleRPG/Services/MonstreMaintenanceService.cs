using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Services;

namespace MyLittleRPG.Services
{
    public class MonstreMaintenanceService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MonstreMaintenanceService> _logger;

        public MonstreMaintenanceService(IServiceProvider serviceProvider, ILogger<MonstreMaintenanceService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private async Task ValidateMonsterCount(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            {
                var context = scope.ServiceProvider.GetRequiredService<MonsterContext>();

                //On peut utiliser le context de la mÃªme faÃ§on que dans nos controllers Ã  partir d'ici
                var monsterCount = await context.InstanceMonstres.CountAsync(cancellationToken);

                if (monsterCount <= 300)
                {
                    //On a accÃ¨s Ã  un logger pour sortir de l'information dans la console.
                    _logger.LogError($"blablabla j'aime les patates");
                }
            }
        }

        //ConÃ§u pour s'exÃ©cuter une seule fois et contenir une boucle.
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            TimeSpan _checkInterval = TimeSpan.FromMinutes(30); // Check every 30 minutes
                                                                // Perform initial check on startup
            await ValidateMonsterCount(cancellationToken);

            // Continue checking periodically
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_checkInterval, cancellationToken);
                    await ValidateMonsterCount(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during periodic monster count validation");
                    // Continue the loop - don't let one failure kill the service
                }
            }
        }
    }
}