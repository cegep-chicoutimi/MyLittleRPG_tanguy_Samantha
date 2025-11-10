using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLittleRPG.Controllers;
using MyLittleRPG.Data.Context;
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
        private async Task CreateQuest(MonsterContext context, CancellationToken ct, Personnage personnage)
        {
            List<string> typesMonstre = new List<string>() { "grass", "poison", "fire", "water", "normal", "electric", "ground", "fairy", "fighting", "psychic", "rock", "steel",
            "ghost", "dragon", "flying", "bug", "ice", "dark"};
            Random rnd = new Random();
            int nbRandomQuete = rnd.Next(1, 4);

            switch (nbRandomQuete)
            {
                case 1:
                    QueteNiveauAtteint queteNiveau = new QueteNiveauAtteint(personnage.Niveau + rnd.Next(1, 3), personnage.Id);
                    context.QuetesNiveauAtteint.Add(queteNiveau);
                    break;
                case 2:
                    int type = (rnd.Next(0, typesMonstre.Count));
                    QueteVaincreMonstres queteMonstre = new QueteVaincreMonstres(rnd.Next(3, 10), typesMonstre[type], personnage.Id);
                    context.QuetesVaincreMonstres.Add(queteMonstre);
                    break;
                case 3:
                    Tile? tileRandom = await GetTuileRandom();
                    QueteVisiterTuile queteTuile = new QueteVisiterTuile(tileRandom.X, tileRandom.Y, tileRandom.Type, personnage.Id);
                    context.QuetesVisiterTuile.Add(queteTuile);
                    break;
                default:
                    break;
            }
        }
        private async Task<Tile?> GetTuileRandom()
        {
            Tile? tileRandom;
            bool valide = false;
            Random rnd = new Random();
            do
            {
                int x = rnd.Next(0, 50);
                int y = rnd.Next(0, 50);

                using var scope = _sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MonsterContext>();

                tileRandom = await context.Tiles.FindAsync(x, y);

                if(tileRandom == null || tileRandom.estTraversable == false)
                {
                    valide = false;
                }else
                {
                    valide = true;
                }

            } while (!valide);

            return tileRandom;
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

                var personnages = await context.Personnages.ToListAsync(ct);

                foreach (Personnage p in personnages)
                {
                    if (p.NbQuetes >= 0 && p.NbQuetes < 3)
                    {
                        for (int i = p.NbQuetes; i < 3; i++)
                        {
                            await CreateQuest(context, ct, p);
                            p.NbQuetes++;
                        }
                    }
                }
                await context.SaveChangesAsync(ct);

                _logger.LogInformation("Régénération terminée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec de la régénération des quêtes");
            }
            finally
            {
                _mutex.Release();
            }
        }
    }
}
