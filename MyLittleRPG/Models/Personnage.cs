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
        }
    }
}
