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
        /// <summary>
        /// Va chercher l'utilisateur selon l'identifiant de l'utilisateur
        /// </summary>
        /// <param name="UserId">id de l'utilisateur</param>
        /// <returns>utilisateru</returns>
        [HttpGet("User/{email}")]
        public async Task<ActionResult<Utilisateur>> GetUserByEmail(string email)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("Aucun utilisateur correspond à cet email");
            }

            return Ok(user);
        }

        /// <summary>
        /// Permet à l'utilisateur de se connecter
        /// </summary>
        /// <param name="login">email et mot de passe</param>
        /// <returns>Utilisateur connecté</returns>
        // GET: api/Utilisateurs/5
        [HttpPost]
        [Route("auth/login")]
        public async Task<ActionResult<Utilisateur>> Login([FromBody] LoginUser login)
        {

            if(string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.MotDePasse))
            {
                return BadRequest("Le email ou  le mot de passe entre n'est pas valide");
            }

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
            utilisateur.isConnected = true;

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

            return Ok(utilisateur);
        }
        /// <summary>
        /// Pour hash le mot de passe
        /// </summary>
        /// <param name="arrInput">mot de passe</param>
        /// <returns>output</returns>
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
        /// <summary>
        /// Création d'un utilisateur
        /// </summary>
        /// <param name="utilisateur">utilisateur</param>
        /// <returns>résultat</returns>
        // POST: api/Utilisateurs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("auth/register")]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur([FromBody] RegisterDTO utilisateur)
        {   
            if(utilisateur == null) return Unauthorized(new {message = "L'utilisateur entré est null"});

            if (string.IsNullOrEmpty(utilisateur.password) || string.IsNullOrEmpty(utilisateur.pseudo))
            {
                return BadRequest("Le pseudo ou le mot de passe de l'utilisateur entré ne sont pas valide");
            }
            if (!utilisateur.email.Contains('@') || string.IsNullOrEmpty(utilisateur.email))
            {
                return BadRequest("L'email entré de l'utilisateur n'est pas valide");
            }

            if (UtilisateurExists(utilisateur.email))
            {
                return Unauthorized(new { message = "L'email est déjà utilisé" });
            }

            byte[] tmpSource;
            byte[] tmpHash;

            //Create a byte array from source data
            tmpSource = ASCIIEncoding.ASCII.GetBytes(utilisateur.password);

            //Compute hash based on source data
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            utilisateur.password=ByteArrayToString(tmpHash);

            Utilisateur newUser = new Utilisateur(utilisateur);
            
            _context.Utilisateurs.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserByEmail", new { email = utilisateur.email }, newUser);
        }

        /// <summary>
        /// Permet à l'utilisateur de se déconnecter
        /// </summary>
        /// <param name="id">Id de l'utilisateur</param>
        /// <returns>Résultat de la déconnexion</returns>
        [HttpPost]
        [Route("auth/logout/{id}")]
        public async Task<ActionResult> Logout(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);

            if (utilisateur == null)
            {
                return NotFound(new { message = "Utilisateur non trouvé" });
            }

            if (!utilisateur.isConnected)
            {
                return BadRequest(new { message = "L'utilisateur est déjà déconnecté" });
            }

            utilisateur.isConnected = false;
            utilisateur.TempsConexion = DateTime.Now;

            _context.Entry(utilisateur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(id))
                {
                    return NotFound(new { message = "Utilisateur introuvable lors de la mise à jour" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Déconnexion réussie" });
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
