using System.Text.Json.Serialization;

namespace MyLittleRPG.Models
{
    // DTO pour la connexion
    public class LoginDTO()
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class RegisterDTO()
    {
        public string pseudo {  get; set; }
        public string email {  get; set; }
        public string password { get; set; }
    }

    public class ResultDto
    {
        public string code { get; set; }
        public InstanceMonstreDto Monstre { get; set; }
        public PersonnageDto Personnage { get; set; }
    } 

    public class MonstreVaincuDto
    {
        public Monster monstre { get; set; }
        public Boolean EstVaincu { get; set; }
    }
    public class PokedexDto
    {
        public PersonnageDto Personnage { get; set; }

        public List<MonstreVaincuDto> pokedex { get; set;}
    }

    // DTO pour représenter un monstre instancié sur la carte
    public class InstanceMonstreDto
    {

        [JsonConstructor]
        public InstanceMonstreDto(
        int id,
        int monstreId,
        string nom,
        string spriteUrl,
        int niveau,
        int x,
        int y,
        int pointsVieActuels,
        int pointsVieMax,
        int attaque,
        int defense,
        int experienceDonnee,
        bool estVivant)
        {
            Id = id;
            MonstreId = monstreId;
            Nom = nom;
            SpriteUrl = spriteUrl;
            Niveau = niveau;
            X = x;
            Y = y;
            PointsVieActuels = pointsVieActuels;
            PointsVieMax = pointsVieMax;
            Attaque = attaque;
            Defense = defense;
            ExperienceDonnee = experienceDonnee;
            EstVivant = estVivant;
        }

        public InstanceMonstreDto(InstanceMonster enemy)
        {
            this.MonstreId = enemy.MonsterId;
            this.Nom = enemy.Monster.Nom;
            this.SpriteUrl = enemy.Monster.spriteUrl;
            this.Niveau = enemy.niveaux;
            this.X = enemy.X;
            this.Y = enemy.Y;
            this.PointsVieActuels = enemy.PVactuels;
            this.PointsVieMax = enemy.PVMax;
            this.Attaque = enemy.Monster.forceBase;
            this.Defense = enemy.Monster.defenseBase;
            this.ExperienceDonnee = enemy.Monster.experienceBase+(enemy.niveaux*10);
            this.EstVivant = enemy.PVactuels>0;
        }

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
        private Tile tile;

        [JsonConstructor]
        public TuileAvecInfosDto(int x, int y, string? typeTuile, string? imageUrl,
                             InstanceMonstreDto? monstre, bool estAccessible)
        {
            X = x; Y = y; TypeTuile = typeTuile; this.imageUrl = imageUrl;
            Monstre = monstre; EstAccessible = estAccessible;
        }

        public TuileAvecInfosDto(Tile tile)
        {
            this.tile = tile;
            this.X = tile.X;
            this.Y = tile.Y;
            this.TypeTuile = tile.Type.ToString();
            this.imageUrl = tile.imageURL;
            this.EstAccessible = tile.estTraversable;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public string? TypeTuile { get; set; }

        public string? imageUrl { get; set; }

        // Monstre présent sur la tuile (null si aucun)
        public InstanceMonstreDto? Monstre { get; set; }

        // Indique si le joueur peut se déplacer sur cette tuile
        public bool EstAccessible { get; set; }
    }

    // DTO pour une grille 3x3 autour du joueur (comme demandé précédemment)
    public class GrilleJeuDto
    {
        public GrilleJeuDto()
        {
            Tuiles = new List<TuileAvecInfosDto>();
            resultFight = new ResultDto();
            questResult = new QuestResultDTO();
        }
        public List<TuileAvecInfosDto>? Tuiles { get; set; }

        // Position centrale de la grille (position du joueur)
        public int CentreX { get; set; }
        public int CentreY { get; set; }
        public ResultDto resultFight { get; set; }
        public QuestResultDTO questResult { get; set; }
    }
    public class CreatePersonnageDto
    {
        public int IdUser { get; set; }
        public string Nom { get; set; } = string.Empty;
    }

    public class PersonnageDto
    {
        public PersonnageDto(Personnage personnage)
        {
            this.id = personnage.Id;
            this.Niveau = personnage.Niveau;
            this.XP = personnage.XP;
            this.PV = personnage.PV;
            this.PVMax = personnage.PVMax;
            this.Force = personnage.Force;
            this.Defense = personnage.Defense;
            this.X = personnage.X;
            this.Y = personnage.Y;
        }
        public int id { get; set; }
        public int Niveau { get; set; }
        public int XP { get; set; }
        public int PV { get; set; }
        public int PVMax { get; set; }
        public int Force { get; set; }
        public int Defense { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
    //DTO quest
    public class QuestDTO
    {
        public QuestDTO()
        {
            this.QuetesVaincreMonstre = new List<QueteVaincreMonstres>();
            this.QuetesVisiterTuile = new List<QueteVisiterTuile>();
            this.QueteNiveauAtteints = new List<QueteNiveauAtteint>();
            this.nbQuetes = 0;
            this.NbQuetesMAX = 3;
        }
        public int Id { get; set; }
        public List<QueteNiveauAtteint> QueteNiveauAtteints { get; set; }
        public List<QueteVaincreMonstres> QuetesVaincreMonstre {  get; set; }
        public List<QueteVisiterTuile> QuetesVisiterTuile { get; set; }

        public int nbQuetes { get; set; }
        public int NbQuetesMAX { get; }

    }
    public class QuestResultDTO
    {
        public QuestResultDTO()
        {
            this.QuetesVaincreMonstreReussi = new List<QueteVaincreMonstres>();
            this.QuetesVisiterTuileReussi = new List<QueteVisiterTuile>();
            this.QueteNiveauAtteintsReussi = new List<QueteNiveauAtteint>();
            this.nbQuetesReussi = 0;
        }
        public int Id { get; set; }
        public List<QueteNiveauAtteint> QueteNiveauAtteintsReussi { get; set; }
        public List<QueteVaincreMonstres> QuetesVaincreMonstreReussi { get; set; }
        public List<QueteVisiterTuile> QuetesVisiterTuileReussi { get; set; }

        public int nbQuetesReussi { get; set; }

    }
}
