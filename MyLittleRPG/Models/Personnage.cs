using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace MyLittleRPG.Models
{
    public class Personnage
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int Niveau { get; set; }
        public int XP { get; set; }
        public int PV { get; set; }
        public int PVMax { get; set; }
        public int Force { get; set; }
        public int Defense { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateCreation { get; set; }

        public int PositionVilleX { get; set; }
        public int PositionVilleY { get; set; }

        public Personnage(int id, string nom, int niveau, int xP, int pV, int pVMax, int force, int defense, int positionX, int positionY, int utilisateurId)
        {
            Id = id;
            Nom = nom;
            Niveau = niveau;
            XP = xP;
            PV = pV;
            PVMax = pVMax;
            Force = force;
            Defense = defense;
            PositionX = positionX;
            PositionY = positionY;
            UtilisateurId = utilisateurId;
            DateCreation = DateTime.Now;
            PositionVilleX = 10;
            PositionVilleY = 10;
        }

        internal void getexp(int exp)
        {
            XP += exp;
            if(XP > 20 * Niveau)
            {
                XP -= 20 * Niveau;
                Niveau++;
                PVMax++;
                Force++;
                Defense++;
                PV = PVMax;
            }
        }

        internal void backToTown()
        {
            PositionX = PositionVilleX; 
            PositionY = PositionVilleY;
            PV = PVMax;
        }

        internal void savechanges(PersonnageDto personnage)
        {
            Niveau = personnage.Niveau;
            XP = personnage.XP;
            PV = personnage.PV;
            PVMax = personnage.PVMax;
            Force = personnage.Force;
            Defense = personnage.Defense;
            PositionX = personnage.PositionX;
            PositionY = personnage.PositionY;
        }
    }
}
