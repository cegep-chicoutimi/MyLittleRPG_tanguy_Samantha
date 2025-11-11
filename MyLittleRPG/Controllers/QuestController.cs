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
    public class QuestController : ControllerBase
    {
        private readonly MonsterContext _context;

        public QuestController(MonsterContext context)
        {
            _context = context;
        }

        // GET: api/QuestDTOes
        [HttpGet("QuetesPerso/{idPerso}")]
        public async Task<ActionResult<QuestDTO>> GetQuestDTO(int idPerso)
        {
            var personnage = await _context.Personnages.FirstOrDefaultAsync(p => p.Id == idPerso);

            if(personnage == null) return BadRequest("L'id du personnage est invalide");

            QuestDTO questDTO = new QuestDTO();

            var quetesNiveau = await _context.QuetesNiveauAtteint.ToListAsync();
            var quetesTuile = await _context.QuetesVisiterTuile.ToListAsync();
            var quetesMonstre = await _context.QuetesVaincreMonstres.ToListAsync();

            if ((quetesNiveau == null && quetesMonstre == null && quetesTuile == null)) return NotFound("Aucune quetes disponible");

            CheckQuetesNiveauAtteint(quetesNiveau, ref questDTO, personnage.Id);
            CheckQuetesVaincreMonstres(quetesMonstre, ref questDTO, personnage.Id);
            CheckQuetesVisiterTuiles(quetesTuile, ref questDTO, personnage.Id);


            return Ok(questDTO);
        }

        private ActionResult CheckQuetesNiveauAtteint(List<QueteNiveauAtteint>? quetes, ref QuestDTO questDTO, int idperso)
        {
            if (quetes != null && quetes.Count() > 0)
            {
                foreach (QueteNiveauAtteint quete in quetes)
                {
                    if (quete.IdPersonnage == idperso && quete.IsActive)
                    {
                        if (questDTO.nbQuetes >= questDTO.NbQuetesMAX) return Unauthorized("Le personnage a un nombre trop eleve de quetes");
                        questDTO.QueteNiveauAtteints.Add(quete);
                        questDTO.nbQuetes++;
                        if (questDTO.nbQuetes == questDTO.NbQuetesMAX) break;
                    }
                }
            }
            else
            {
                return BadRequest("Erreur lors de la verification des quetes");
            }
            return Ok();
        }
        private ActionResult CheckQuetesVaincreMonstres(List<QueteVaincreMonstres>? quetes, ref QuestDTO questDTO, int idperso)
        {
            if (quetes != null && quetes.Count() > 0)
            {
                foreach (QueteVaincreMonstres quete in quetes)
                {
                    if (quete.IdPersonnage == idperso && quete.IsActive)
                    {
                        if (questDTO.nbQuetes >= questDTO.NbQuetesMAX) return Unauthorized("Le personnage a un nombre trop eleve de quetes");
                        questDTO.QuetesVaincreMonstre.Add(quete);
                        questDTO.nbQuetes++;
                        if (questDTO.nbQuetes == questDTO.NbQuetesMAX) break;
                    }
                }
            }
            else
            {
                return BadRequest("Erreur lors de la verification des quetes");
            }
            return Ok();
        }
        private ActionResult CheckQuetesVisiterTuiles(List<QueteVisiterTuile>? quetes, ref QuestDTO questDTO, int idperso)
        {
            if (quetes != null && quetes.Count() > 0)
            {
                foreach (QueteVisiterTuile quete in quetes)
                {
                    if (quete.IdPersonnage == idperso && quete.IsActive)
                    {
                        if (questDTO.nbQuetes >= questDTO.NbQuetesMAX) return Unauthorized("Le personnage a un nombre trop eleve de quetes");
                        questDTO.QuetesVisiterTuile.Add(quete);
                        questDTO.nbQuetes++;
                        if (questDTO.nbQuetes == questDTO.NbQuetesMAX) break;
                    }
                }
            }
            else
            {
                return BadRequest("Erreur lors de la verification des quetes");
            }
            return Ok();
        }
    }
}
