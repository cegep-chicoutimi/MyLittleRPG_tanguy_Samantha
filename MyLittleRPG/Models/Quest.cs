namespace MyLittleRPG.Models
{
    public class Quest
    {
        public int Id { get; set; }
        public int IdPersonnage { get; set; }
        public int? IdObjectifVaincreMonstres { get; set; }
        public int? IdObjectifVisiterTuile { get; set; }
        public int? IdObjectifNiveauAtteint { get; set; }

        public bool IsActive { get; set; }

        Quest(int idPerso)
        {
            IdPersonnage = idPerso;
            IdObjectifNiveauAtteint = null;
            IdObjectifVaincreMonstres = null;
            IdObjectifVisiterTuile = null;
            IsActive = true;
        }
    }
}
