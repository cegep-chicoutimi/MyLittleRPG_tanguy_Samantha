using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
    public class UtilisateursController : ControllerBase
    {
        private readonly MonsterContext _context;

        public UtilisateursController(MonsterContext context)
        {
            _context = context;
        }

        // GET: api/Utilisateurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await _context.Utilisateurs.ToListAsync();
        }

        // GET: api/Utilisateurs/5
        [HttpPost("login")]
        public async Task<ActionResult<Utilisateur>> Login([FromBody] Login login)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            if (utilisateur.MotDePasse != login.MotDePasse)
            {
                return BadRequest("Mot de passe incorrect");
            }

            return utilisateur;
        }

        // PUT: api/Utilisateurs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilisateur(int id, Utilisateur utilisateur)
        {
            if (id != utilisateur.Id)
            {
                return BadRequest();
            }

            _context.Entry(utilisateur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(id))
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

        // POST: api/Utilisateurs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur(Utilisateur utilisateur)
        {   
            if(utilisateur == null) return BadRequest();
            
            if(UtilisateurExists(utilisateur.Email))
            {
                return BadRequest("L'email est déjà utilisé");
            }

            utilisateur.DateInscription = DateTime.Now;
            
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUtilisateur", new { email = utilisateur.Email, mdp = utilisateur.MotDePasse }, utilisateur);
        }

        //// DELETE: api/Utilisateurs/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUtilisateur(int id)
        //{
        //    var utilisateur = await _context.Utilisateurs.FindAsync(id);
        //    if (utilisateur == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Utilisateurs.Remove(utilisateur);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool UtilisateurExists(int id)
        {
            return _context.Utilisateurs.Any(e => e.Id == id);
        }
        private bool UtilisateurExists(string email)
        {
            return _context.Utilisateurs.Any(e => e.Email == email);
        }
    }
}
