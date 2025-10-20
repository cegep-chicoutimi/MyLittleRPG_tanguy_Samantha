using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Migrations;
using MyLittleRPG.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: api/Personnages/5
        [HttpGet("User/{UserId}")]
        public async Task<ActionResult<Personnage>> GetPersonnageUserId(int UserId)
        {
            var personnage = await _context.Personnages.FirstOrDefaultAsync(p => p.UtilisateurId == UserId);

            if (personnage == null)
            {
                return NotFound("Aucun utilisateur correspond à cet id");
            }

            return personnage;
        }

        //// GET: api/Personnages/5
        //[HttpGet("{Id}")]
        //public async Task<ActionResult<Personnage>> GetPersonnage(int Id)
        //{
        //    var personnage = await _context.Personnages.FirstOrDefaultAsync(p => p.Id == Id);

        //    if (personnage == null)
        //    {
        //        return NotFound("Aucun personnage correspond à cet id");
        //    }

        //    return personnage;
        //}

        [HttpGet("Deplacement")]
        public async Task<ActionResult<GrilleJeuDto>> Deplacement(int posX, int posY, int idPerso)
        {
            GrilleJeuDto grille = new GrilleJeuDto();
            var personnage = await _context.Personnages.FindAsync(idPerso);

            if (personnage == null)
            {
                return NotFound( new { message = "Aucun personnage correspond à cet id" });
            }

            if (posX < 0 || posX > 50 || posY < 0 || posY > 50) 
                return BadRequest(new { messeage = "La position voulu est hors de la carte" });

            if (!_tileGeneration.GenererTile(posX, posY).Result.estTraversable)
                return BadRequest(new { messeage = "la case nest pas traversable" });

            var monstre = await _context.InstanceMonstres
                        .Include(im => im.Monster)
                        .FirstOrDefaultAsync(im => im.X == posX && im.Y == posY);

            if (monstre != null)
            {
                grille.resultFight = fight(posX, posY, idPerso);
                personnage.savechanges(grille.resultFight.Personnage);
                monstre.PVactuels = grille.resultFight.Monstre.PointsVieActuels;
            }
            else
            {
                grille.resultFight = null;
            }

            if (grille.resultFight == null|| grille.resultFight.code=="Win")
            {
                if ((Math.Abs(posX - personnage.X) <= 1) && (Math.Abs(posY - personnage.Y) <= 1))
                {
                    personnage.X = posX;
                    personnage.Y = posY;


                    _context.Entry(personnage).State = EntityState.Modified;

                    saveDeplacement(grille, posX, posY, idPerso);
                }
            }
            else if(grille.resultFight.code == "Lose"|| grille.resultFight.code == "Draw")
            {
                saveDeplacement(grille, posX, posY, idPerso);
            }
            return BadRequest(new { message = "Déplacement non autorisé. Vous pouvez vous déplacer d'une case maximum." });        
        }

        private async Task<ActionResult<GrilleJeuDto>> saveDeplacement(GrilleJeuDto grille ,int posX,int posY,int idPerso)
        {
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



            grille.Tuiles = Tuiles;
            grille.CentreX = posX;
            grille.CentreY = posY;

            return grille;
        }
        

        private ResultDto fight(int X, int Y, int id)
        {
            ResultDto resultat = new ResultDto();
            var personnage = _context.Personnages.Find(id);
            var enemy = _context.InstanceMonstres
               .Include(im => im.Monster)
               .FirstOrDefault(im => im.X == X && im.Y == Y);

            Random random = new Random();
            double factmonster = (random.Next(80, 125) / 100.0);
            double factperso = (random.Next(80, 125) / 100.0);

            int damageMonster = (int)((personnage.Force - (enemy.Monster.defenseBase + enemy.niveaux)) * factmonster);
            int damagePlayer = (int)(((enemy.Monster.forceBase + enemy.niveaux) - personnage.Defense) * factperso);

            if (damageMonster > 0)
            {
                enemy.PVactuels -= damageMonster;
            }

            if (damagePlayer > 0)
            {
                personnage.PV -= damagePlayer;
            }

            if (enemy.PVactuels <= 0)
            {
                _context.InstanceMonstres.Remove(enemy);
                personnage.getexp(enemy.Monster.experienceBase+(enemy.niveaux*10));
                resultat.code = "Win";
            }else if(personnage.PV <= 0)
            {
                personnage.backToTown();
                resultat.code = "Lose";
            }else
            {
                resultat.code = "Draw";
            }

            _context.SaveChanges();

            PersonnageDto personnageDto = new PersonnageDto(personnage);
            resultat.Personnage = personnageDto;
            InstanceMonstreDto instanceMonstre = new InstanceMonstreDto(enemy);
            resultat.Monstre = instanceMonstre;


            Console.WriteLine(enemy);

            checkMonstreVaincu();
            return resultat;
        }

        private void checkMonstreVaincu()
        {
            int nbrMonstre = _context.InstanceMonstres.Count();
            MonsterGeneration generator = new MonsterGeneration(_context);
            if (nbrMonstre < 290 ) { generator.generate10(); }
        }



        // POST: api/Personnages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Personnage>> PostPersonnage([FromBody] CreatePersonnageDto persoDto)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Id == persoDto.IdUser);

            if (utilisateur == null)
            {
                return NotFound("L'id de l'utilisateur n'est pas valide");
            }

            if (string.IsNullOrEmpty(persoDto.Nom)) return BadRequest(new { message = "Le nom de l'utilisateur ne doit pas être vide" });
            Random random = new Random();

            Personnage personnage = new Personnage(0, persoDto.Nom, 1,1,random.Next(10,15),50,random.Next(5,10),random.Next(5,10), 10, 10, persoDto.IdUser);

            _context.Personnages.Add(personnage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonnage", new { id = personnage.Id }, personnage);
        }



        private bool PersonnageExists(int id)
        {
            return _context.Personnages.Any(e => e.Id == id);
        }

        
    }
}
