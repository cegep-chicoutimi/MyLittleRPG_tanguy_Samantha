using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Migrations;
using MyLittleRPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLittleRPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly MonsterContext _context;
        private MonsterGeneration generator;

        public MonstersController(MonsterContext context)
        {
            _context = context;
            generator = new MonsterGeneration(context);
        }

        [HttpPut]
        [Route("monstre/generateall")]
        public async Task<IActionResult> GenerateAll()
        {
            const int N = 300;

            // 1) Reset (éviter les triggers lents : DELETE direct)
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM InstanceMonstres");

            // 2) Préchargements SANS tracking (un seul round-trip chacun)
            var tiles = await _context.Tiles
                .AsNoTracking()
                .Select(t => new { t.X, t.Y, t.estTraversable, t.Type })
                .ToListAsync();

            var spawnables = tiles
                .Where(t => t.estTraversable && t.Type != TileType.VILLE && t.Type != TileType.ROUTE)
                .Select(t => (t.X, t.Y))
                .ToList();

            if (spawnables.Count < N)
                return BadRequest(new { message = $"Pas assez de tuiles spawnables ({spawnables.Count}) pour {N} monstres." });

            var villes = tiles
                .Where(t => t.Type == TileType.VILLE)
                .Select(t => (t.X, t.Y))
                .ToList();

            var monsters = await _context.Monsters
                .AsNoTracking()
                .Select(m => new { m.Id, m.pointsVieBase })
                .ToListAsync();

            if (monsters.Count == 0)
                return BadRequest(new { message = "Aucun monstre maître dans la table Monsters." });

            // 3) Mélange et sélection de N cases distinctes (pas de do/while ni HashSet string)
            var rng = new Random();
            // Fisher-Yates
            for (int i = spawnables.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (spawnables[i], spawnables[j]) = (spawnables[j], spawnables[i]);
            }
            var picked = spawnables.Take(N).ToList();

            // 4) Calcul du niveau = min distance manhattan à une ville (en mémoire)
            int DistanceVille(int x, int y)
            {
                if (villes.Count == 0) return 0;
                int best = int.MaxValue;
                foreach (var (vx, vy) in villes)
                {
                    int d = Math.Abs(x - vx) + Math.Abs(y - vy);
                    if (d < best) best = d;
                    if (best == 0) break;
                }
                return best;
            }

            // 5) Construction en mémoire
            var instances = new List<InstanceMonster>(N);
            foreach (var (x, y) in picked)
            {
                var m = monsters[rng.Next(monsters.Count)];
                var level = DistanceVille(x, y) / 3;

                instances.Add(new InstanceMonster
                {
                    X = x,
                    Y = y,
                    MonsterId = m.Id,
                    niveaux = level,
                    PVMax = m.pointsVieBase,
                    PVactuels = m.pointsVieBase
                });
            }

            // 6) Insertion en une passe (accélération EF)
            var oldDetect = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                await _context.InstanceMonstres.AddRangeAsync(instances);
                await _context.SaveChangesAsync();
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = oldDetect;
            }

            return Ok($"{N} monstres régénérés");
        }

        [HttpGet("pokedex/{personnageId:int}")]
        public async Task<ActionResult<PokedexDto>> GetPokedex(
            int personnageId,
            int page = 1,
            int pageSize = 20,
            string? type = null,
            string? nom = null)
        {
            // 1) Vérifier que le personnage existe
            var personnage = await _context.Personnages
                .FirstOrDefaultAsync(p => p.Id == personnageId);

            if (personnage == null)
            {
                return NotFound(new { message = $"Personnage {personnageId} introuvable." });
            }

            // 2) Récupérer les IDs des monstres déjà vaincus par ce personnage
            var monstresVaincusIds = await _context.Pokedex
                .Where(p => p.PersonnageId == personnageId)
                .Select(p => p.MonsterId)
                .ToListAsync();

            var vaincusSet = new HashSet<int>(monstresVaincusIds);

            // 3) Construire la requête de base sur les monstres
            IQueryable<Monster> query = _context.Monsters;

            // --- Filtres ---
            if (!string.IsNullOrWhiteSpace(type))
            {
                var typeLower = type.ToLower();

                // adapte les noms de propriétés en C# selon ton modèle (Type1 / type1, Type2 / type2, etc.)
                query = query.Where(m =>
                    m.type1.ToLower() == typeLower ||
                    (m.type2 != null && m.type2.ToLower() == typeLower));
            }

            if (!string.IsNullOrWhiteSpace(nom))
            {
                var nomLower = nom.ToLower();
                query = query.Where(m => m.Nom.ToLower().Contains(nomLower));
                // si ta propriété C# est "Nom", change en m.Nom.ToLower()
            }

            // 4) Normalisation des paramètres de pagination
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100; // petit max raisonnable

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // 5) Appliquer la pagination
            var monstersPage = await query
                .OrderBy(m => m.Id) // pour avoir un ordre stable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 6) Construire la liste MonstreVaincuDto sur la page seulement
            var pokedexPage = monstersPage
                .Select(m => new MonstreVaincuDto
                {
                    monstre = new MonsterDto
                    {
                        Id = m.Id,
                        PokemonId = m.PokemonId,
                        Nom = m.Nom,
                        pointsVieBase = m.pointsVieBase,
                        forceBase = m.forceBase,
                        defenseBase = m.defenseBase,
                        experienceBase = m.experienceBase,
                        spriteUrl = m.spriteUrl,
                        type1 = m.type1,
                        type2 = m.type2,
                        rarity = (int)m.Rarity
                    },
                    EstVaincu = vaincusSet.Contains(m.Id)
                })
                .ToList();

            // 7) Construire le DTO de réponse
            var result = new PokedexDto
            {
                Personnage = new PersonnageDto(personnage),
                pokedex = pokedexPage
            };

            return Ok(result);
        }

    }

}
