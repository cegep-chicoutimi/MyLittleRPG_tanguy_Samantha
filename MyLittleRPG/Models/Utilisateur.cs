namespace MyLittleRPG.Models
{
    public class Utilisateur
    {
        public int Id {  get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Pseudo {  get; set; }
        public DateTime DateInscription { get; set; }

        public Utilisateur(int id, string email, string motDePasse, string pseudo) 
        {
            Id = id;
            Email = email;
            MotDePasse = motDePasse;
            Pseudo = pseudo;
            DateInscription = DateTime.Now;
        }

    }
}
