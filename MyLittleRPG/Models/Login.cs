namespace MyLittleRPG.Models
{
    public class Login
    {
        public string? Email { get; set; }
        public string? MotDePasse { get; set; }

        public Login(string? email, string? motDePasse)
        {
            Email = email;
            MotDePasse = motDePasse;
        }
    }
}
