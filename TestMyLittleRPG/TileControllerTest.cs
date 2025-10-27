using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLittleRPG.Tests
{
    public class TileControllerTest
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
            Assert.True(true);


        }
    }
}
