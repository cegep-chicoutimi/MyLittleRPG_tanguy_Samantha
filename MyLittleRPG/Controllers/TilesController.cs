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

        // GET: api/Tiles
        [HttpGet]
        public async Task<ActionResult<GrilleJeuDto>> GetTilesAutour(int PositionX, int PositionY)
        {
            List<Tile> tiles = (List<Tile>)await TileGeneration.GenererTilesAutour(PositionX, PositionY);

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
            grille.CentreX = PositionX;
            grille.CentreY = PositionY;

            return grille;
        }

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

        //// PUT: api/Tiles/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTile(int id, Tile tile)
        //{
        //    if (id != tile.PositionX)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tile).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TileExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Tiles
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Tile>> PostTile(Tile tile)
        //{
        //    _context.Tiles.Add(tile);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (TileExists(tile.PositionX))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetTile", new { id = tile.PositionX }, tile);
        //}

        //// DELETE: api/Tiles/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTile(int id)
        //{
        //    var tile = await _context.Tiles.FindAsync(id);
        //    if (tile == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Tiles.Remove(tile);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool TileExists(int id)
        {
            return _context.Tiles.Any(e => e.X == id);
        }
    }


}
