namespace MyLittleRPG.Models
{
    public class QueteVaincreMonstres
    {
        public int Id { get; set; }
        public int NbMonstresAVaincre { get; set; }
        public string? TypeMonstre { get; set; }
        public int IdPersonnage { get; set; }
        public bool IsActive { get; set; }

        public QueteVaincreMonstres() { }

        public QueteVaincreMonstres(int nbMonstres, string? typeMonstre, int idPerso)
        {
            NbMonstresAVaincre = nbMonstres;
            TypeMonstre = typeMonstre;
            IdPersonnage = idPerso;
            IsActive = true;
        }
    }
}
