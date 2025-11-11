using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyLittleRPG;
using MyLittleRPG.Models;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace TestMyLittleRPG
{
    public class TileControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public TileControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTuiles_WithAuthenticatedUser_Returns3x3Grid()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles?X=3&Y=3&UserId=37");

            GrilleJeuDto grille = await GetTilesResponse.Content.ReadFromJsonAsync<GrilleJeuDto>();

            Assert.NotNull(grille);
            Assert.Equal(3, grille.CentreX);
            Assert.Equal(3, grille.CentreY);
            Assert.Equal(9, grille.Tuiles.Count);

        }

        [Fact]
        public async Task GetTuiles_WithoutAuthentication_ReturnsUnauthorized()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles?X=3&Y=3&UserId=38");


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);

        }
        [Fact]
        public async Task GetTuiles_WithDisconnectedUser_ReturnsUnauthorized()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles?X=3&Y=3&UserId=39");


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);

        }



        [Fact]
        public async Task GetTuiles_WithAuthenticatedUser_IncludesMonsterData()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles?X=3&Y=3&UserId=37");
            var GetTilesResponse2 = await _client.GetAsync("/api/Tiles?X=6&Y=6&UserId=37");
            var GetTilesResponse3 = await _client.GetAsync("/api/Tiles?X=9&Y=9&UserId=37");

            List<TuileAvecInfosDto> listOfTiles;

            GrilleJeuDto grille = await GetTilesResponse.Content.ReadFromJsonAsync<GrilleJeuDto>();
            listOfTiles = grille.Tuiles;
            grille = await GetTilesResponse2.Content.ReadFromJsonAsync<GrilleJeuDto>();
            listOfTiles.AddRange(grille.Tuiles);
            grille = await GetTilesResponse3.Content.ReadFromJsonAsync<GrilleJeuDto>();
            listOfTiles.AddRange(grille.Tuiles);

            List<InstanceMonstreDto> listOfMonstres = new List<InstanceMonstreDto>();

            foreach (var tile in listOfTiles)
            {
                if (tile.Monstre != null)
                {
                    listOfMonstres.Add(tile.Monstre);
                }
            }


            Assert.NotEqual(0, listOfMonstres.Count);
            Assert.NotNull(listOfMonstres.First().Id);
            Assert.NotNull(listOfMonstres.First().Attaque);


        }
        [Fact]
        public async Task ExplorerTuile_WithinRange_ReturnsTuileData()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/11%2C11?X=11&Y=11&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();

            Assert.NotNull(Tile);
            Assert.Equal(11, Tile.X);
            Assert.Equal(11, Tile.Y);
            Assert.Equal("ROUTE", Tile.TypeTuile);
            Assert.True(Tile.EstAccessible);
        }
        [Fact]
        public async Task ExplorerTuile_WithinRange_ReturnsMonsterIfPresent()
        {
            //ce test peut echouer puisque generation de monstre aleatoire merci de le relance jusqua son bon fonctionement

            await _client.PutAsync("/api/Monsters/monstre/generateall", null);

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/11%2C11?X=10&Y=11&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();

            Assert.NotNull(Tile);
            Assert.NotNull(Tile.Monstre);
        }
        [Fact]
        public async Task ExplorerTuile_WithinRange_ReturnsNullMonsterIfEmpty()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/11%2C11?X=11&Y=11&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();

            Assert.NotNull(Tile);
            Assert.Null(Tile.Monstre);
        }
        [Fact]
        public async Task ExplorerTuile_TwoStepsAway_Succeeds()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/12%2C10?X=11&Y=11&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();

            Assert.NotNull(Tile);
        }
        [Fact]
        public async Task ExplorerTuile_FiveStepsAway_ReturnsForbidden()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/12%2C10?X=20&Y=20&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);
        }
        [Fact]
        public async Task ExplorerTuile_BeyondMapBoundaries_ReturnsForbidden()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/12%2C10?X=51&Y=51&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);
        }
        [Fact]
        public async Task ExplorerTuile_NegativeCoordinates_ReturnsForbidden()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/12%2C10?X=51&Y=51&UserId=37");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);
        }
        [Fact]
        public async Task ExplorerTuile_WithoutAuthentication_ReturnsForbidden()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/12%2C10?X=1&Y=1&UserId=2");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);
        }
        [Fact]
        public async Task ExplorerTuile_WithDisconnectedUser_ReturnsForbidden()
        {

            var GetTilesResponse = await _client.GetAsync("/api/Tiles/12%2C10?X=1&Y=1&UserId=39");


            TuileAvecInfosDto Tile = await GetTilesResponse.Content.ReadFromJsonAsync<TuileAvecInfosDto>();


            Assert.False(GetTilesResponse.IsSuccessStatusCode);
            Assert.Equal("Unauthorized", GetTilesResponse.ReasonPhrase);
        }

        [Fact]
        public async Task GetTuiles_OnedgeOfMap_returnonlyValidTile()
        {

            //ce test ne fonctionne pqs pour le moment cqr le progrqmme ne sadapte pas au coord pour savoir le taille du tableau a renvoyer
            var GetTilesResponse = await _client.GetAsync("/api/Tiles?X=1&Y=1&UserId=40");
            GrilleJeuDto grille = await GetTilesResponse.Content.ReadFromJsonAsync<GrilleJeuDto>();

            Assert.NotNull(grille);
            Assert.Equal(1, grille.CentreX);
            Assert.Equal(1, grille.CentreY);
            Assert.Equal(4, grille.Tuiles.Count);

        }
    }
}