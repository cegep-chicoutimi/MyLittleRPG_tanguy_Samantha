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
                TuileAvecInfosDto tileDTO = new TuileAvecInfosDto();
                tileDTO.X = tile.PositionX;
                tileDTO.Y = tile.PositionY;
                tileDTO.TypeTuile = tile.Type.ToString();
                tileDTO.EstAccessible = tile.estTraversable;
                var InstanceMonstre = await _context.InstanceMonstres.FindAsync(tile.PositionX, tile.PositionY);

                if (InstanceMonstre != null)
                {
                    InstanceMonstreDto monsterDTO = new InstanceMonstreDto();
                    monsterDTO.MonstreId = InstanceMonstre.Monster.Id;
                    monsterDTO.Nom = InstanceMonstre.Monster.Nom;
                    monsterDTO.SpriteUrl = InstanceMonstre.Monster.spriteUrl;
                    monsterDTO.Niveau = InstanceMonstre.niveaux;
                    monsterDTO.X = InstanceMonstre.PositionX;
                    monsterDTO.Y = InstanceMonstre.PositionY;
                    monsterDTO.PointsVieActuels = InstanceMonstre.PVactuels;
                    monsterDTO.PointsVieMax = InstanceMonstre.PVMax;
                    monsterDTO.Attaque = InstanceMonstre.Monster.forceBase + InstanceMonstre.niveaux;
                    monsterDTO.Defense = InstanceMonstre.Monster.defenseBase + InstanceMonstre.niveaux;
                    monsterDTO.ExperienceDonnee = InstanceMonstre.Monster.experienceBase + (InstanceMonstre.niveaux * 10);

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
        public async Task<ActionResult<TuileAvecInfosDto>> GetTile(int PositionX, int PositionY)
        {

            Tile? tile =  await TileGeneration.GenererTile(PositionX, PositionY);
            if (tile == null) return BadRequest(new { message = "Les positions entrées ne sont pas valide" });

            CreatedAtAction("GetTile", new { id = tile.PositionX }, tile);

            TuileAvecInfosDto tileDTO = new TuileAvecInfosDto();
            tileDTO.X = tile.PositionX;
            tileDTO.Y = tile.PositionY;
            tileDTO.TypeTuile = tile.Type.ToString();
            tileDTO.EstAccessible = tile.estTraversable;
            var InstanceMonstre = await _context.InstanceMonstres
                .Include(im => im.Monster)
                .FirstOrDefaultAsync(im => im.PositionX == tile.PositionX && im.PositionY == tile.PositionY);

            if (InstanceMonstre != null)
            {
                InstanceMonstreDto monsterDTO = new InstanceMonstreDto();
                monsterDTO.MonstreId = InstanceMonstre.Monster.Id;
                monsterDTO.Nom = InstanceMonstre.Monster.Nom;
                monsterDTO.SpriteUrl = InstanceMonstre.Monster.spriteUrl;
                monsterDTO.Niveau = InstanceMonstre.niveaux;
                monsterDTO.X = InstanceMonstre.PositionX;
                monsterDTO.Y = InstanceMonstre.PositionY;
                monsterDTO.PointsVieActuels = InstanceMonstre.PVactuels;
                monsterDTO.PointsVieMax = InstanceMonstre.PVMax;
                monsterDTO.Attaque = InstanceMonstre.Monster.forceBase + InstanceMonstre.niveaux;
                monsterDTO.Defense = InstanceMonstre.Monster.defenseBase + InstanceMonstre.niveaux;
                monsterDTO.ExperienceDonnee = InstanceMonstre.Monster.experienceBase + (InstanceMonstre.niveaux * 10);

                tileDTO.Monstre = monsterDTO;
            }

            return tileDTO;
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
            return _context.Tiles.Any(e => e.PositionX == id);
        }
    }
}
