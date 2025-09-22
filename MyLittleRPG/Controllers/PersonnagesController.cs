using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Models;

namespace MyLittleRPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonnagesController : ControllerBase
    {
        private readonly MonsterContext _context;
        private TileGeneration _tileGeneration;

        public PersonnagesController(MonsterContext context)
        {
            _context = context;
            _tileGeneration = new TileGeneration(context);
        }

        // GET: api/Personnages
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Personnage>>> GetPersonnages()
        //{
        //    return await _context.Personnages.ToListAsync();
        //}

        // GET: api/Personnages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personnage>> GetPersonnage(int id)
        {
            var personnage = await _context.Personnages.FindAsync(id);

            if (personnage == null)
            {
                return NotFound("Aucun utilisateur correspond à cet id");
            }

            return personnage;
        }

        [HttpGet]
        [Route("Deplacement")]
        public async Task<ActionResult<IEnumerable<Tile>>> Deplacement(int posX, int posY, int idPerso)
        {
            var personnage = await _context.Personnages.FindAsync(idPerso);

            if (personnage == null)
            {
                return NotFound( new { message = "Aucun personnage correspond à cet id" });
            }

            if (posX < 0 || posX > 50 || posY < 0 || posY > 50) 
                return BadRequest(new { messeage = "la position voulu est hors de la carte" });

            if ((Math.Abs(posX - personnage.PositionX) <= 1) && (Math.Abs(posY - personnage.PositionY) <= 1))
            {
                personnage.PositionX = posX;
                personnage.PositionY = posY;

                _context.Entry(personnage).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonnageExists(idPerso))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var tiles = await _tileGeneration.GenererTilesAutour(posX, posY);
                return (List<Tile>)tiles;
            }
            return BadRequest(new { message = "Déplacement non autorisé. Vous pouvez vous déplacer d'une case maximum." });        
        }

        // PUT: api/Personnages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPersonnage(int id, Personnage personnage)
        //{
        //    var oldPersonnage = await _context.Personnages.FindAsync(id);

        //    if (oldPersonnage == null)
        //    {
        //        return BadRequest("Mauvais id de personnage");
        //    }

        //    if (personnage.PV > oldPersonnage.PVMax || personnage.PV < oldPersonnage.PV || personnage.Force > 15 || personnage.Defense > 20 || 
        //        personnage.Force < oldPersonnage.Force || personnage.Defense < oldPersonnage.Defense || personnage.Niveau < oldPersonnage.Niveau || personnage.XP < oldPersonnage.XP)
        //    {
        //        return BadRequest("Les données entrées ne sont pas valide pour un personnage");
        //    }

        //    oldPersonnage.Nom = personnage.Nom;
        //    oldPersonnage.Niveau = personnage.Niveau;
        //    oldPersonnage.PV = personnage.PV;
        //    oldPersonnage.XP = personnage.XP;
        //    oldPersonnage.Force = personnage.Force;
        //    oldPersonnage.Defense = personnage.Defense;
        //    oldPersonnage.PositionX = personnage.PositionX;
        //    oldPersonnage.PositionY = personnage.PositionY;

        //    _context.Entry(oldPersonnage).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PersonnageExists(id))
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

        // POST: api/Personnages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{idUser},{nom}")]
        public async Task<ActionResult<Personnage>> PostPersonnage(int idUser, string nom)
        {
            Random random = new Random();

            Personnage personnage = new Personnage(0, nom,1,1,random.Next(10,15),50,random.Next(5,10),random.Next(5,10), 10, 10, idUser);

            _context.Personnages.Add(personnage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonnage", new { id = personnage.Id }, personnage);
        }

        // DELETE: api/Personnages/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePersonnage(int id)
        //{
        //    var personnage = await _context.Personnages.FindAsync(id);
        //    if (personnage == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Personnages.Remove(personnage);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool PersonnageExists(int id)
        {
            return _context.Personnages.Any(e => e.Id == id);
        }
    }
}
