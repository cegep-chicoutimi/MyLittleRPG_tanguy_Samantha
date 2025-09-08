namespace MyLittleRPG.Models
{
    public class Monster
    {

        public int Id {  get; set; }
        public int PokemonId { get; set; }
        public string Nom {  get; set; }
        public int pointsVieBase { get; set; }
        public int forceBase { get; set; }
        public int defenseBase { get; set; }
        public int experienceBase { get; set; }
        public string spriteUrl { get; set; }
        public string? type1 { get; set; }
        public string? type2 { get; set; }

        public Monster()
        {
        }

        public Monster(int id, int pokemonId, string nom, int pointsVieBase, int forceBase, int defenseBase, int experienceBase, string spriteUrl, string type1, string type2)
        {
            this.Id = id;
            PokemonId = pokemonId;
            Nom = nom;
            this.pointsVieBase = pointsVieBase;
            this.forceBase = forceBase;
            this.defenseBase = defenseBase;
            this.experienceBase = experienceBase;
            this.spriteUrl = spriteUrl;
            this.type1 = type1;
            this.type2 = type2;
        }
    }
}
