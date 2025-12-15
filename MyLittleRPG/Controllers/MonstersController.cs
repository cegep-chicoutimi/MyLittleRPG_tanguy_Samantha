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
        private void GetMonstresByFiltre(string type)
        {
            type = type.Trim().ToLower();
            List<Monster> monsters = new List<Monster>();
            foreach (Monster m in _context.Monsters)
            {
                switch (type)
                {
                    case "fire":
                        if (m.type1 == type || m.type2 == type)
                        {
                            monsters.Add(m);
                        }
                        break;
                    default:
                        monsters.Add(m);
                        break;
                }
            }
        }

        [HttpGet]
        [Route("{idPerso}/{numeroPage}/{typeFiltre}")]
        public async Task<IActionResult> GetMonstres(int idPerso, int numeroPage, string typeFiltre)
        {
            var monstres = _context.Monsters;
            var monstresChasses = await _context.MonsterHunted
                .Where(monstre => monstre.IdPerso == idPerso)
                .ToListAsync();

            for (int i = 0; i < 20; i++)
            {
                
            }

            PokedexDTO pokeDTO = new PokedexDTO(numeroPage, typeFiltre);


            foreach (MonsterHunted m in monstresChasses)
            {
                var monstre = await _context.Monsters.FindAsync(m.IdMonstre);
                if (monstre != null)
                {
                    pokeDTO.monstres.Add(new MonstrePokedexDTO(monstre, true));
                }
            }
            foreach(Monster m in monstres)
            {
                
            }

            return Ok();
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

    }
}
