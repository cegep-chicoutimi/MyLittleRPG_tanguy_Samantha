namespace MyLittleRPG.Models
{
    public class QueteNiveauAtteint
    {
        public int Id { get; set; }
        public int NiveauAAtteindre { get; set; }
        public int IdPersonnage { get; set; }
        public int NiveauPerso { get; set; }
        public bool IsActive { get; set; }

        public QueteNiveauAtteint() { }

        public QueteNiveauAtteint(int niveau, int idperson, int niveauPerso)
        {
            NiveauAAtteindre = niveau;
            IdPersonnage = idperson;
            NiveauPerso = niveauPerso;
            IsActive = true;
        }
    }
}
