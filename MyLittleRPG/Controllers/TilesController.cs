using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLittleRPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {

        private TileGeneration TileGeneration;

        private readonly MonsterContext _context;

        public TilesController(MonsterContext context)
        {
            _context = context;
            TileGeneration = new TileGeneration(context);
        }

        /// <summary>
        /// Va chercher les tuiles autour du personnage
        /// </summary>
        /// <param name="PositionX">PositionX</param>
        /// <param name="PositionY">PositionY</param>
        /// <returns>grille des tuiles dévoilés</returns>
        // GET: api/Tiles
        [HttpGet]
        public async Task<ActionResult<GrilleJeuDto>> GetTilesAutour(int X, int Y,int UserId)
        {
            if (!_context.Utilisateurs.FirstOrDefault(e => e.Id == UserId).isConnected)
            {
                return Unauthorized();
            }


            List<Tile> tiles = (List<Tile>)await TileGeneration.GenererTilesAutour(X, Y);

            if(tiles == null) return NotFound(new {message = "Les tuiles n'ont pas pu se générer"});

            List<TuileAvecInfosDto> Tuiles = new List<TuileAvecInfosDto>();

            foreach (var tile in tiles)
            {
                TuileAvecInfosDto tileDTO = new TuileAvecInfosDto(tile);

                var InstanceMonstre = await _context.InstanceMonstres
                        .Include(im => im.Monster)
                        .FirstOrDefaultAsync(im => im.X == tile.X && im.Y == tile.Y);

                if (InstanceMonstre != null)
                {
                    InstanceMonstreDto monsterDTO = new InstanceMonstreDto(InstanceMonstre);
                    tileDTO.Monstre = monsterDTO;
                }

                Tuiles.Add(tileDTO);
            }

            GrilleJeuDto grille = new GrilleJeuDto();

            grille.Tuiles = Tuiles;
            grille.CentreX = X;
            grille.CentreY = Y;

            return grille;
        }
        /// <summary>
        /// Va chercher une tuile selon la position
        /// </summary>
        /// <param name="X">PositionX</param>
        /// <param name="Y">PositionY</param>
        /// <returns>tuile</returns>
        // GET: api/Tiles/5
        [HttpGet("{PositionX,PositionY}")]
        public async Task<ActionResult<TuileAvecInfosDto>> GetTile(int X, int Y)
        {

            Tile? tile =  await TileGeneration.GenererTile(X, Y);
            if (tile == null) return BadRequest(new { message = "Les positions entrées ne sont pas valide" });

            CreatedAtAction("GetTile", new { id = tile.X }, tile);

            TuileAvecInfosDto tileDTO = new TuileAvecInfosDto(tile);
            var InstanceMonstre = await _context.InstanceMonstres
                .Include(im => im.Monster)
                .FirstOrDefaultAsync(im => im.X == tile.X && im.Y == tile.Y);

            if (InstanceMonstre != null)
            {
                InstanceMonstreDto monsterDTO = new InstanceMonstreDto(InstanceMonstre);

                tileDTO.Monstre = monsterDTO;
            }

            return tileDTO;
        }
        /// <summary>
        /// Générer toutes les tuiles du jeu 
        /// </summary>
        /// <param name="minX">taille minimum grille X</param>
        /// <param name="maxX">taille maximum grille X</param>
        /// <param name="minY">taille minimum grille Y</param>
        /// <param name="maxY">taille maximum grille Y</param>
        /// <returns>résultat</returns>
        // GET: api/Tiles/generate/all?minX=1&maxX=50&minY=1&maxY=50
        [HttpGet("generate/all")]
        public async Task<IActionResult> AddAllTiles(
            int minX = 1, int maxX = 50,
            int minY = 1, int maxY = 50)
        {
            if (minX > maxX || minY > maxY)
                return BadRequest(new { message = "Bornes invalides." });

            int created = 0;

            // Accélère les insertions massives EF
            var oldDetect = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        // GenererTile évite les doublons (FindAsync d'abord)
                        var tile = await TileGeneration.GenererTile(x, y);
                        if (tile != null) created++;
                    }
                }

                await _context.SaveChangesAsync();

                var total = await _context.Tiles.CountAsync();
                return Ok(new
                {
                    message = "Génération terminée",
                    created,
                    totalTiles = total,
                    bounds = new { minX, maxX, minY, maxY }
                });
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = oldDetect;
            }
        }
    }


}
