using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyLittleRPG.Models;
using MyLittleRPG;

namespace TestMyLittleRPG
{
    public class UtilisateurControllerTest :IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UtilisateurControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Registration_Works()
        {
            await Task.Delay(2000);

            var testEmail = "JDoe@mail.com";
            var testMDP = "Password";
            var testPseudo = "JohnDoe";


            var registerDto = new RegisterDTO
            {
                email = testEmail,
                password = testMDP,
                pseudo = testPseudo

            };

           // var registerResponse = await _client

        }
    }
}