namespace MyLittleRPG.Models
{
    public class QueteNiveauAtteint
    {
        public int Id { get; set; }
        public int Niveau { get; set; }
        public int IdPersonnage { get; set; }
        public bool IsActive { get; set; }

        public QueteNiveauAtteint() { }

        public QueteNiveauAtteint(int niveau, int idperson)
        {
            Niveau = niveau;
            IdPersonnage = idperson;
            IsActive = true;
        }
    }
}
