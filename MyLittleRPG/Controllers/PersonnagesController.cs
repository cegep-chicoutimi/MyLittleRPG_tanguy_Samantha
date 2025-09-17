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

        public PersonnagesController(MonsterContext context)
        {
            _context = context;
        }

        // GET: api/Personnages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personnage>>> GetPersonnages()
        {
            return await _context.Personnages.ToListAsync();
        }

        // GET: api/Personnages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personnage>> GetPersonnage(int id)
        {
            var personnage = await _context.Personnages.FindAsync(id);

            if (personnage == null)
            {
                return NotFound();
            }

            return personnage;
        }

        // PUT: api/Personnages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonnage(int id, Personnage personnage)
        {
            if (id != personnage.Id)
            {
                return BadRequest();
            }

            _context.Entry(personnage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonnageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonnage(int id)
        {
            var personnage = await _context.Personnages.FindAsync(id);
            if (personnage == null)
            {
                return NotFound();
            }

            _context.Personnages.Remove(personnage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonnageExists(int id)
        {
            return _context.Personnages.Any(e => e.Id == id);
        }
    }
}
