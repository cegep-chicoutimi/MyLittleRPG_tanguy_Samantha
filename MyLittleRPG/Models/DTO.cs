namespace MyLittleRPG.Models
{
    // DTO pour la connexion
    public class LoginDTO()
    {
        public string email;
        public string password;
    }

    public class ResultDto
    {
        public string code { get; set; }
        public InstanceMonstreDto Monstre { get; set; }
        public PersonnageDto Personnage { get; set; }
    } 


    // DTO pour représenter un monstre instancié sur la carte
    public class InstanceMonstreDto
    {
        public int Id { get; set; }
        public int MonstreId { get; set; }
        public string Nom { get; set; }
        public string SpriteUrl { get; set; }
        public int Niveau { get; set; }

        // Position sur la carte
        public int X { get; set; }
        public int Y { get; set; }

        // Statistiques calculées du monstre
        public int PointsVieActuels { get; set; }
        public int PointsVieMax { get; set; }
        public int Attaque { get; set; }
        public int Defense { get; set; }

        // Informations supplémentaires
        public int ExperienceDonnee { get; set; }
        public bool EstVivant { get; set; }
    }

    // DTO pour une tuile avec informations complètes (incluant monstre s'il y en a un)
    public class TuileAvecInfosDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string TypeTuile { get; set; }

        // Monstre présent sur la tuile (null si aucun)
        public InstanceMonstreDto? Monstre { get; set; }

        // Indique si le joueur peut se déplacer sur cette tuile
        public bool EstAccessible { get; set; }
    }

    // DTO pour une grille 3x3 autour du joueur (comme demandé précédemment)
    public class GrilleJeuDto
    {
        public List<TuileAvecInfosDto> Tuiles { get; set; }

        // Position centrale de la grille (position du joueur)
        public int CentreX { get; set; }
        public int CentreY { get; set; }
    }

    public class PersonnageDto
    {

        public int Niveau { get; set; }
        public int XP { get; set; }
        public int PV { get; set; }
        public int PVMax { get; set; }
        public int Force { get; set; }
        public int Defense { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
}
