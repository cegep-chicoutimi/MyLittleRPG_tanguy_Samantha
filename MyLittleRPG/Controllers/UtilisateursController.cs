using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        //{
        //    return await _context.Utilisateurs.ToListAsync();
        //}

        [HttpGet("{email}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(string email)
        {
            var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);

            if (utilisateur == null)
            {
                return NotFound(new {message = "L'email entrée ne correspond à aucun utilisateur"});
            }

            return utilisateur;
        }

        // GET: api/Utilisateurs/5
        [HttpPost]
        [Route("auth/login")]
        public async Task<ActionResult<Utilisateur>> Login([FromBody] LoginUser login)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (utilisateur == null)
            {
                return Unauthorized(new { message = "L'email entrée ne correspond à aucun utilisateur" });
            }

            if (utilisateur.MotDePasse != login.MotDePasse)
            {
                return Unauthorized(new { message = "Mot de passe incorrect" });
            }

            utilisateur.TempsConexion = DateTime.Now;
            _context.Entry(utilisateur).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(utilisateur.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return utilisateur;
        }

        // PUT: api/Utilisateurs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut]
        //[Route("auth/logout/{id}")]
        //public async Task<IActionResult> PutUtilisateur(int id)
        //{
        //    Utilisateur? utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);

        //    if (utilisateur == null)
        //    {
        //        return BadRequest("Cet utilisateur n'existe pas");
        //    }

        //    utilisateur.TempsConexion = null;

        //    _context.Entry(utilisateur).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UtilisateurExists(id))
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

        // POST: api/Utilisateurs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("auth/register")]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur([FromBody] Utilisateur utilisateur)
        {   
            if(utilisateur == null) return Unauthorized(new {message = "L'utilisateur entré est null"});
            
            if(UtilisateurExists(utilisateur.Email))
            {
                return Unauthorized(new { message = "L'email est déjà utilisé" });
            }

            utilisateur.DateInscription = DateTime.Now;
            
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUtilisateur", new { email = utilisateur.Email }, utilisateur);
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
