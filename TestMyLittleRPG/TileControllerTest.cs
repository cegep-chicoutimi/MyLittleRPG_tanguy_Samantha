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

            List<InstanceMonstreDto> listOfMonstres= new List<InstanceMonstreDto>() ;

            foreach (var tile in listOfTiles) 
            { 
                if (tile.Monstre != null)
                {
                    listOfMonstres.Add(tile.Monstre);
                }
            }


            Assert.NotEqual(0, listOfMonstres.Count);
            Assert.NotNull( listOfMonstres.First().Id);
            Assert.NotNull( listOfMonstres.First().Attaque);


        }
    }
}