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
using System.Threading;
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
        private List<Monster> GetMonstersSection(int page, string type)
        {
            int x = 0;
            List<Monster> monsters = GetMonstresByFiltre(type.Trim().ToLower());
            List<Monster> monstersInPage = new List<Monster>();

            if (page == 1 || page <= 0) x = 0; else x = (page - 1) * 20;

            for (int i = x; i< x + 20 && i < monsters.Count; i++)
            {
                monstersInPage.Add(monsters[i]);
            }
            return monstersInPage;
        }
        private List<Monster> GetMonstresByFiltre(string type)
        {
            List<Monster> monsters = new List<Monster>();
            type = type?.Trim().ToLower();

            if (string.IsNullOrEmpty(type) || type == "all")
                return _context.Monsters.ToList();

            return _context.Monsters
                .Where(m => m.type1 == type || m.type2 == type)
                .ToList();
        }

        [HttpGet]
        [Route("{idPerso}/{numeroPage}/{typeFiltre}")]
        public async Task<IActionResult> GetMonstres(int idPerso, int numeroPage, string typeFiltre)
        {
            try
            {
                bool isHunted = false;
                List<Monster> monsters = GetMonstersSection(numeroPage, typeFiltre);

                int nbPages = (int)Math.Ceiling(_context.Monsters.Count() / 20.0);
                var monstresChasses = await _context.MonsterHunted
                    .Where(monstre => monstre.IdPerso == idPerso)
                    .ToListAsync();


                PokedexDTO pokeDTO = new PokedexDTO(numeroPage, typeFiltre, nbPages);
                foreach (var monster in monsters)
                {
                    isHunted = false;
                    foreach (var mHunted in monstresChasses)
                    {
                        if (monster.Id == mHunted.IdMonstre)
                        {
                            isHunted = true;
                            pokeDTO.monstres.Add(new MonstrePokedexDTO(monster, isHunted));
                            break;
                        }
                        else
                        {
                            isHunted = false;
                        }
                    }
                    if (isHunted == false)
                    {
                        pokeDTO.monstres.Add(new MonstrePokedexDTO(monster, isHunted));
                    }
                }

                return Ok(pokeDTO);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("{idPerso}/{nomRecherche}")]
        public async Task<IActionResult> GetMonstres(int idPerso, string nomRecherche)
        {
            try
            {
                bool isHunted = false;
                List<Monster> monstres = GetMonstresByFiltre("all");

                var monstresChasses = await _context.MonsterHunted
                    .Where(monstre => monstre.IdPerso == idPerso)
                    .ToListAsync();

                PokedexDTO pokedex = new PokedexDTO(0, "all", _context.Monsters.Count()/20 );
                foreach(Monster m in _context.Monsters)
                {
                    if(m.Nom.Trim().ToLower().Contains(nomRecherche.Trim().ToLower()))
                    {
                        isHunted = false;
                        foreach (var mHunted in monstresChasses)
                        {
                            if (m.Id == mHunted.IdMonstre)
                            {
                                isHunted = true;
                                pokedex.monstres.Add(new MonstrePokedexDTO(m, isHunted));
                                break;
                            }
                            else
                            {
                                isHunted = false;
                            }
                        }
                        if (isHunted == false)
                        {
                            pokedex.monstres.Add(new MonstrePokedexDTO(m, isHunted));
                        }
                    }
                }
                return Ok(pokedex);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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
