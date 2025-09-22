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
        //private int probaHerbe = 20;
        //private int probaEau = 10;
        //private int probaMontagne = 15;
        //private int probaForet = 15;
        //private int probaVille = 05;
        //private int probaRoute = 35;
        private TileGeneration TileGeneration;

        private readonly MonsterContext _context;

        public TilesController(MonsterContext context)
        {
            _context = context;
            TileGeneration = new TileGeneration(context);
        }

        // GET: api/Tiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tile>>> GetTilesAutour(int PositionX, int PositionY)
        {
            List<Tile> tiles = (List<Tile>)await TileGeneration.GenererTilesAutour(PositionX, PositionY);

            if(tiles == null) return NotFound(new {message = "Les tuiles n'ont pas pu se générer"});

            return tiles;
        }

        // GET: api/Tiles/5
        [HttpGet("{PositionX,PositionY}")]
        public async Task<ActionResult<Tile>> GetTile(int PositionX, int PositionY)
        {

            Tile? tile =  await TileGeneration.GenererTile(PositionX, PositionY);
            if (tile == null) return BadRequest(new { message = "Les positions entré ne sont pas valide" });

            CreatedAtAction("GetTile", new { id = tile.PositionX }, tile);          

            return tile;
        }

        //private void resetproba()
        //{
        //    probaHerbe = 20;
        //    probaEau = 10;
        //    probaMontagne = 15;
        //    probaForet = 15;
        //    probaVille = 05;
        //    probaRoute = 35;
        //}

        //private void reglerproba(int positionX, int positionY)
        //{
        //    var tileW = _context.Tiles.Find(positionX - 1, positionY);
        //    checkTile(tileW);
        //    var tileE = _context.Tiles.Find(positionX + 1, positionY);
        //    checkTile(tileE);
        //    var tileN = _context.Tiles.Find(positionX, positionY + 1);
        //    checkTile(tileN);
        //    var tileS = _context.Tiles.Find(positionX, positionY - 1);
        //    checkTile(tileS);

        //}


        //private void checkTile(Tile? tile)
        //{
        //    if (tile != null)
        //    {
        //        if (tile.Type == TileType.FORET)
        //        {
        //            probaForet += 10;
        //            probaHerbe -= 2;
        //            probaEau -= 2;
        //            probaMontagne -= 2;
        //            probaVille -= 2;
        //            probaRoute -= 2;
        //        }
        //        else if (tile.Type == TileType.EAU)
        //        {
        //            probaEau += 10;
        //            probaHerbe -= 2;
        //            probaForet -= 2;
        //            probaMontagne -= 2;
        //            probaVille -= 2;
        //            probaRoute -= 2;
        //        }
        //        else if (tile.Type == TileType.MONTAGNE)
        //        {
        //            probaMontagne += 10;
        //            probaHerbe -= 2;
        //            probaForet -= 2;
        //            probaEau -= 2;
        //            probaVille -= 2;
        //            probaRoute -= 2;
        //        }

        //    }
        //}


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
            return _context.Tiles.Any(e => e.PositionX == id);
        }
    }
}
