namespace MyLittleRPG.Models
{
    public class LoginUser
    {
        public string? Email { get; set; }
        public string? MotDePasse { get; set; }

        public LoginUser(string? email, string? motDePasse)
        {
            Email = email;
            MotDePasse = motDePasse;
        }
    }
}
