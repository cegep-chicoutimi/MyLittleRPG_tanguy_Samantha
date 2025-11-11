namespace MyLittleRPG.Models
{
    public class Utilisateur
    {
        public int Id {  get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Pseudo {  get; set; }
        public DateTime DateInscription { get; set; }

        public DateTime? TempsConexion { get; set; }

        public bool isConnected { get; set; } = false;

        public Utilisateur() { }

        public Utilisateur(RegisterDTO register) 
        {
            Email = register.email;
            MotDePasse = register.password;
            Pseudo = register.pseudo;
            DateInscription = DateTime.Now;
            TempsConexion = null;
            isConnected = false;
        }

    }
}
