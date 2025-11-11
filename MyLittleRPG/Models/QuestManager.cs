using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Data.Context;

namespace MyLittleRPG.Models
{
    public class QuestManager
    {
        private MonsterContext Context;
        public QuestManager(MonsterContext context) 
        {
            Context = context;
        }

        public async Task<QuestResultDTO> UpdateQueteniveauAtteintState(Personnage perso, QuestResultDTO result)
        {
            List<QueteNiveauAtteint> quetes = await Context.QuetesNiveauAtteint.Where(p =>  p.IdPersonnage == perso.Id).ToListAsync();

            if(quetes == null || quetes.Count == 0) { return result; }

            foreach (var quete in quetes)
            {
                quete.NiveauPerso = perso.Niveau;
                if (quete.NiveauAAtteindre == quete.NiveauPerso)
                {
                    result.nbQuetesReussi++;
                    result.QueteNiveauAtteintsReussi.Add(quete);
                    quete.IsActive = false;
                    perso.NbQuetes--;
                }
            }
            return result;
        }
        public async Task<QuestResultDTO> UpdateQueteVisiterTuileState(Personnage perso, QuestResultDTO result)
        {
            List<QueteVisiterTuile>? quetes = await Context.QuetesVisiterTuile.Where(p => p.IdPersonnage == perso.Id).ToListAsync();

            if (quetes == null || quetes.Count == 0) { return result; }

            foreach (var quete in quetes)
            {
                if(quete.X == perso.X && quete.Y == perso.Y)
                {
                    result.nbQuetesReussi++;
                    result.QuetesVisiterTuileReussi.Add(quete);
                    quete.IsActive = false;
                    perso.NbQuetes--;
                }
                quete.DistanceX = quete.X - perso.X;
                quete.DistanceY = quete.Y - perso.Y;
            }
            return result;
        }
        public async Task<QuestResultDTO> UpdateQueteVaicreMonstreState(Personnage perso, Monster? monster, QuestResultDTO result)
        {
            if (monster == null) return result;
            List<QueteVaincreMonstres>? quetes = await Context.QuetesVaincreMonstres.Where(p => p.IdPersonnage == perso.Id).ToListAsync();

            if (quetes == null || quetes.Count == 0) { return result; }

            foreach (var quete in quetes)
            {
                if (quete.TypeMonstre == monster.type1 || quete.TypeMonstre == monster.type2)
                {
                    result.nbQuetesReussi++;
                    result.QuetesVaincreMonstreReussi.Add(quete);
                    quete.IsActive = false;
                    perso.NbQuetes--;
                }
            }
            return result;
        }
    }
}
