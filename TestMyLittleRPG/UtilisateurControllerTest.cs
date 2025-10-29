using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyLittleRPG.Models;
using MyLittleRPG;
using System.Net.Http.Json;
using NuGet.Protocol;

namespace TestMyLittleRPG
{
    public class UtilisateurControllerTest :IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private string _urlRegister, _urlCreatePerso, _urlLogin;

        public UtilisateurControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _urlRegister = "/api/Utilisateurs/auth/register";
            _urlCreatePerso = "/api/Personnages";
            _urlLogin = "/api/Utilisateurs/auth/Login";
        }

        [Fact]
        public async Task Registration_Works()
        {
            await Task.Delay(2000);

            var testEmail = $"TestPlayer_{Guid.NewGuid()}@mail.com"; ;
            var testMDP = "Password";
            var testPseudo = "JohnDoe";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    "/api/Utilisateurs/auth/register",
                    registerDto
                );

            Assert.True(registerResponse.IsSuccessStatusCode,
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task Registration_EmptyMDP_Fails()
        {
            await Task.Delay(2000);

            var testEmail = $"TestPlayer_{Guid.NewGuid()}@mail.com"; ;
            var testMDP = "";
            var testPseudo = "abc";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.ReasonPhrase == "Bad Request",
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }
        [Fact]
        public async Task Registration_EmptyPseudo_Fails()
        {
            await Task.Delay(2000);

            var testEmail = $"TestPlayer_{Guid.NewGuid()}@mail.com";
            var testMDP = "Password";
            var testPseudo = "";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.ReasonPhrase == "Bad Request",
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }
        [Fact]
        public async Task Registration_EmptyEmail_Fails()
        {
            await Task.Delay(2000);

            var testEmail = "";
            var testMDP = "Password";
            var testPseudo = "abc";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.ReasonPhrase == "Bad Request",
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }
        [Fact]
        public async Task Registration_InvalidEmail_Fails()
        {
            await Task.Delay(2000);

            var testEmail = "abc";
            var testMDP = "Password";
            var testPseudo = "abc";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.ReasonPhrase == "Bad Request",
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }
        [Fact]
        public async Task CreatePerson_WithValidData_Succes()
        {
            await Task.Delay(2000);

            //Création utilisateur valide
            var testEmail = $"TestPlayer_{Guid.NewGuid()}@mail.com";
            var testMDP = "Password";
            var testPseudo = "JohnDoe";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.IsSuccessStatusCode,
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            Assert.NotNull(userCreated);

            //Création Personnage valide
            var createPersoDto = new CreatePersonnageDto
            {
                IdUser = userCreated.Id,
                Nom = userCreated.Pseudo
            };

            var persoResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            Assert.True(persoResponse.IsSuccessStatusCode,
                $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }
        [Fact]
        public async Task CreatePerson_WithInvalidPseudo_Fails()
        {
            await Task.Delay(2000);

            //Création utilisateur valide
            var testEmail = $"TestPlayer_{Guid.NewGuid()}@mail.com";
            var testMDP = "Password";
            var testPseudo = "JohnDoe";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.IsSuccessStatusCode,
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            Assert.NotNull(userCreated);

            //Création Personnage valide
            var createPersoDto = new CreatePersonnageDto
            {
                IdUser = userCreated.Id,
                Nom = ""
            };

            var persoResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            Assert.True(persoResponse.ReasonPhrase == "Bad Request",
                $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }
        [Fact]
        public async Task CreatePerson_WithInvalidUser_Fails()
        {
            await Task.Delay(2000);

            Utilisateur user = new Utilisateur();
            //Création Personnage valide
            var createPersoDto = new CreatePersonnageDto
            {
                IdUser = user.Id,
                Nom = user.Pseudo
            };

            var persoResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            Assert.True(persoResponse.ReasonPhrase == "Bad Request",
                $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }
        [Fact]
        public async Task CreatePerson_AlreadyExists_Fails()
        {
            await Task.Delay(2000);

            //Création utilisateur valide
            var testEmail = $"TestPlayer_{Guid.NewGuid()}@mail.com";
            var testMDP = "Password";
            var testPseudo = "JohnDoe";


            var registerDto = new RegisterDTO
            {
                pseudo = testPseudo,
                email = testEmail,
                password = testMDP
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            Assert.True(registerResponse.IsSuccessStatusCode,
                $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            Assert.NotNull(userCreated);

            //Création Personnage valide
            var createPersoDto = new CreatePersonnageDto
            {
                IdUser = userCreated.Id,
                Nom = userCreated.Pseudo
            };

            var persoResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            Assert.True(persoResponse.IsSuccessStatusCode,
                $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");


            //Création perso dupliquer invalide
            var persoInvalidResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            Assert.True(persoInvalidResponse.ReasonPhrase == "Unauthorized",
                $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }
    }
}