using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Models;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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


        //[HttpGet("{email}")]
        //public async Task<ActionResult<Utilisateur>> GetUtilisateur(string email)
        //{
        //    var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);

        //    if (utilisateur == null)
        //    {
        //        return NotFound(new {message = "L'email entrée ne correspond à aucun utilisateur"});
        //    }

        //    return utilisateur;
        //}

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

            byte[] tmpSource;
            byte[] tmpHash;

            //Create a byte array from source data
            tmpSource = ASCIIEncoding.ASCII.GetBytes(login.MotDePasse);

            //Compute hash based on source data
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            Console.WriteLine(ByteArrayToString(tmpHash));

            if (utilisateur.MotDePasse != ByteArrayToString(tmpHash))//attention erreure potentielle
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

        static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

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

            byte[] tmpSource;
            byte[] tmpHash;

            //Create a byte array from source data
            tmpSource = ASCIIEncoding.ASCII.GetBytes(utilisateur.MotDePasse);

            //Compute hash based on source data
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            utilisateur.MotDePasse=ByteArrayToString(tmpHash);

            utilisateur.DateInscription = DateTime.Now;
            
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return Ok("utilisateur créer");
        }

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
