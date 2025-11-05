using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Controllers;
using MyLittleRPG.Models;

namespace MyLittleRPG.Services
{
    public class QuestService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<MonstreMaintenanceService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private readonly SemaphoreSlim _mutex = new(1, 1); // évite le chevauchement

        public QuestService(IServiceProvider sp, ILogger<MonstreMaintenanceService> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1er run au démarrage
            await CheckNbQuest(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_interval, stoppingToken);
                    await CheckNbQuest(stoppingToken);
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
        private async Task CreateQuest(CancellationToken ct, Personnage personnage)
        {
            List<string> typesMonstre = new List<string>() { "grass", "poison", "fire", "water", "normal", "electric", "ground", "fairy", "fighting", "psychic", "rock", "steel",
            "ghost", "dragon", "flying", "bug", "ice", "dark"};
            Random rnd = new Random();
            int nbRandomQuete = rnd.Next(1, 4);

            switch(nbRandomQuete)
            {
                case 1:
                    QueteNiveauAtteint queteNiveau = new QueteNiveauAtteint(personnage.Niveau + rnd.Next(1, 3), personnage.Id);
                    break;
                case 2:
                    int type = (rnd.Next(0, typesMonstre.Count));
                    QueteVaincreMonstres queteMonstre = new QueteVaincreMonstres(rnd.Next(3, 10), typesMonstre[type], personnage.Id);
                    break;
                case 3:
                    //TODO: Refaire logique random tuile
                    int x = rnd.Next(0, 50);
                    int y = rnd.Next(0, 50);
                    QueteVisiterTuile queteTuile = new QueteVisiterTuile();
                    break;
                default:
                    break;
            }
        }

        private async Task CheckNbQuest(CancellationToken ct)
        {
            if (!await _mutex.WaitAsync(0, ct))
            {
                _logger.LogWarning("Un cycle de régénération des quêtes est déjà en cours. Skip.");
                return;
            }

            try
            {
                using var scope = _sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MonsterContext>();

                _logger.LogInformation("Régénérer les quêtes démaré");

                foreach(Personnage p in context.Personnages)
                {
                    if(p.NbQuetes >= 0 && p.NbQuetes < 3)
                    {
                        for(int i = 0; i < 3; i++)
                        {
                            await CreateQuest(ct, p);
                        }
                    }
                }

                //_logger.LogInformation("Régénération terminée: {ResultType}", result?.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec de la régénération des quêtes");
            }
            finally
            {
                _mutex.Release();
            }
            throw new NotImplementedException();
        }
    }
}
