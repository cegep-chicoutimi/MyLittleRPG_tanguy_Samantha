using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Controllers;

namespace MyLittleRPG.Services
{
    public class MonstreMaintenanceService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<MonstreMaintenanceService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);
        private readonly SemaphoreSlim _mutex = new(1, 1); // évite le chevauchement

        public MonstreMaintenanceService(IServiceProvider sp, ILogger<MonstreMaintenanceService> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1er run au démarrage
            //await RunGenerateAllSafe(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_interval, stoppingToken);
                    await RunGenerateAllSafe(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break; // arrêt propre
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur dans MonstreMaintenanceService");
                }
            }
        }

        private async Task RunGenerateAllSafe(CancellationToken ct)
        {
            if (!await _mutex.WaitAsync(0, ct))
            {
                _logger.LogWarning("Un cycle de régénération est déjà en cours. Skip.");
                return;
            }

            try
            {
                using var scope = _sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MonsterContext>();

                // Instancie le controller et appelle directement generateall()
                var controller = new MonstersController(context);

                _logger.LogInformation("Régénération des monstres (300) démarrée...");
                var result = await controller.generateall(); // appelle ta méthode existante
                _logger.LogInformation("Régénération terminée: {ResultType}", result?.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec de la régénération des monstres");
            }
            finally
            {
                _mutex.Release();
            }
        }
    }
}
