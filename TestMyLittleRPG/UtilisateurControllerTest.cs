using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyLittleRPG;
using MyLittleRPG.Models;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;
using System.Net.Http.Json;
using FluentAssertions;

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

        #region Fonctions
        private async Task<HttpResponseMessage> Registration(string email, string mdp, string pseudo)
        {
            var registerDto = new RegisterDTO
            {
                pseudo = pseudo,
                email = email,
                password = mdp
            };

            var registerResponse = await _client.PostAsJsonAsync(
                    _urlRegister,
                    registerDto
                );

            return registerResponse;
        }

        private async Task<HttpResponseMessage> Login(string email, string mdp)
        {
            var login = new LoginUser(email, mdp);

            var loginResponse = await _client.PostAsJsonAsync(
                    _urlLogin,
                    login
                );

            return loginResponse;
        }
        #endregion

        #region Registration
        [Fact]
        public async Task Registration_Works()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();
        }

        [Fact]
        public async Task Registration_MDPisEncrypted_Works()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

            userCreated.MotDePasse.Should().NotBe("Password");
        }

        [Fact]
        public async Task Registration_EmptyMDP_Fails()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "", "JohnDoe");

            registerResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task Registration_EmptyPseudo_Fails()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "");

            registerResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task Registration_EmptyEmail_Fails()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"", "Password", "JohnDoe");

            registerResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task Registration_InvalidEmail_Fails()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer", "Password", "JohnDoe");

            registerResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task CreatePerson_WithValidData_Succes()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            registerResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

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

            persoResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task CreatePerson_WithInvalidPseudo_Fails()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            registerResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

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

            persoResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task CreatePerson_WithInvalidUser_Fails()
        {
            await Task.Delay(2000);

            Utilisateur user = new Utilisateur();

            //Création Personnage invalide
            var createPersoDto = new CreatePersonnageDto
            {
                IdUser = user.Id,
                Nom = user.Pseudo
            };

            var persoResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            persoResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");
        }

        [Fact]
        public async Task CreatePerson_AlreadyExists_Fails()
        {
            await Task.Delay(2000);

            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            registerResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

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

            persoResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");


            //Création perso dupliquer invalide
            var persoInvalidResponse = await _client.PostAsJsonAsync(
                    _urlCreatePerso,
                    createPersoDto
                );

            persoInvalidResponse.ReasonPhrase.Should().Be("Unauthorized", $"Character Creation failed: {await persoInvalidResponse.Content.ReadAsStringAsync()}");
        }
        #endregion

        #region Login
        [Fact]
        public async Task Login_Works()
        {
            await Task.Delay(2000);

            //Creer utilisateur pour test
            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            registerResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

            //Login utilisateur cree
            var loginResponse = await Login(userCreated.Email, "Password");

            loginResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await loginResponse.Content.ReadAsStringAsync()}");

            userCreated = await loginResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

            //Verif utilisateur is connected
            userCreated.isConnected.Should().BeTrue();

        }

        [Fact]
        public async Task GetPerso_ValidUserId_Works()
        {
            await Task.Delay(2000);

            //Creer utilisateur pour test
            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            registerResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

            //Login utilisateur cree
            var loginResponse = await Login(userCreated.Email, "Password");

            loginResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await loginResponse.Content.ReadAsStringAsync()}");

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

            persoResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await persoResponse.Content.ReadAsStringAsync()}");

            //Get personnage
            var getPersoResponse = await _client.GetAsync($"{_urlCreatePerso}/User/{userCreated.Id}");

            getPersoResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await getPersoResponse.Content.ReadAsStringAsync()}");

            var perso = await getPersoResponse.Content.ReadFromJsonAsync<Personnage>();

            perso.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPerso_NonValidUserId_Works()
        {
            var getPersoResponse = await _client.GetAsync($"{_urlCreatePerso}/User/0");

            getPersoResponse.ReasonPhrase.Should().Be("Not Found", $"Registration failed: {await getPersoResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task Login_InvalidEmail_Fails()
        {
            await Task.Delay(2000);

            //Login utilisateur cree
            var loginResponse = await Login("abc", "Password");

            loginResponse.ReasonPhrase.Should().Be("Unauthorized", $"Registration failed: {await loginResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task Login_InvalidMDP_Fails()
        {
            await Task.Delay(2000);

            //Creer utilisateur pour test
            var registerResponse = await Registration($"TestPlayer_{Guid.NewGuid()}@mail.com", "Password", "JohnDoe");

            registerResponse.IsSuccessStatusCode.Should().BeTrue($"Registration failed: {await registerResponse.Content.ReadAsStringAsync()}");

            var userCreated = await registerResponse.Content.ReadFromJsonAsync<Utilisateur>();

            userCreated.Should().NotBeNull();

            //Login utilisateur cree
            var loginResponse = await Login(userCreated.Email, "abc");

            loginResponse.ReasonPhrase.Should().Be("Unauthorized", $"Registration failed: {await loginResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task Login_InvalidUSer_Fails()
        {
            await Task.Delay(2000);

            //Creer utilisateur pour test
            var userCreated = new Utilisateur();

            userCreated.Should().NotBeNull();

            //Login utilisateur cree
            var loginResponse = await Login(userCreated.Email, userCreated.MotDePasse);

            loginResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await loginResponse.Content.ReadAsStringAsync()}");

        }

        [Fact]
        public async Task Login_EmptyCredential_Fails()
        {
            await Task.Delay(2000);

            //Creer utilisateur pour test
            var userCreated = new Utilisateur();

            userCreated.Should().NotBeNull();

            //Login utilisateur cree
            var loginResponse = await Login("", "");

            loginResponse.ReasonPhrase.Should().Be("Bad Request", $"Registration failed: {await loginResponse.Content.ReadAsStringAsync()}");

        }

        #endregion
    }
}