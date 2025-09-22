using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MyLittleRPG.Data.Context;
using System.ComponentModel;

namespace MyLittleRPG.Models
{
    public class TileGeneration
    {
        private readonly MonsterContext _context;

        private int probaHerbe = 20;
        private int probaEau = 10;
        private int probaMontagne = 15;
        private int probaForet = 15;
        private int probaVille = 5;
        private int probaRoute = 35;

        public TileGeneration(MonsterContext context)
        {
            _context = context;
        }

        public async Task<Tile?> GenererTile(int PositionX, int PositionY)
        {
            Tile? tile = await _context.Tiles.FindAsync(PositionX, PositionY);

            if (tile != null)
            {
                return tile;
            }
            reglerproba(PositionX, PositionY);
            Random random = new Random();
            int rand = random.Next(101);

            if (rand < probaHerbe)
            {
                tile = new Tile(PositionX, PositionY, TileType.HERBE, true, "Plains.png");
            }
            else if (rand < probaHerbe + probaEau)
            {
                tile = new Tile(PositionX, PositionY, TileType.EAU, false, "River.png");
            }
            else if (rand < probaHerbe + probaEau + probaMontagne)
            {
                tile = new Tile(PositionX, PositionY, TileType.MONTAGNE, false, "Mountain.png");
            }
            else if (rand < probaHerbe + probaEau + probaMontagne + probaForet)
            {
                tile = new Tile(PositionX, PositionY, TileType.FORET, true, "Forest.png");
            }
            else if (rand < probaHerbe + probaEau + probaMontagne + probaForet + probaVille)
            {
                tile = new Tile(PositionX, PositionY, TileType.VILLE, true, "Town.png");
            }
            else if (rand < probaHerbe + probaEau + probaMontagne + probaForet + probaVille + probaRoute)
            {
                tile = new Tile(PositionX, PositionY, TileType.ROUTE, true, "Road.png");

            }
            else { return null; }

            resetproba();

            _context.Tiles.Add(tile);
            await _context.SaveChangesAsync();

            return tile;
        }

        public async Task<IEnumerable<Tile>> GenererTilesAutour(int posX, int posY)
        {
            List<Tile> tiles = new List<Tile>();
            int[] offsets = { -1, 0, 1 };

            foreach (var dx in offsets)
            {
                foreach (var dy in offsets)
                {
                    int newX = posX + dx;
                    int newY = posY + dy;

                    var tile = await GenererTile(newX, newY);

                    if (tile != null)
                    {
                        tiles.Add(tile);
                    }
                }
            }
            return tiles;
        }
        private void resetproba()
        {
            probaHerbe = 20;
            probaEau = 10;
            probaMontagne = 15;
            probaForet = 15;
            probaVille = 05;
            probaRoute = 35;
        }

        private void reglerproba(int positionX, int positionY)
        {
            var tileW = _context.Tiles.Find(positionX - 1, positionY);
            checkTile(tileW);
            var tileE = _context.Tiles.Find(positionX + 1, positionY);
            checkTile(tileE);
            var tileN = _context.Tiles.Find(positionX, positionY + 1);
            checkTile(tileN);
            var tileS = _context.Tiles.Find(positionX, positionY - 1);
            checkTile(tileS);

        }


        private void checkTile(Tile? tile)
        {
            if (tile != null)
            {
                if (tile.Type == TileType.FORET)
                {
                    probaForet += 10;
                    probaHerbe -= 2;
                    probaEau -= 2;
                    probaMontagne -= 2;
                    probaVille -= 2;
                    probaRoute -= 2;
                }
                else if (tile.Type == TileType.EAU)
                {
                    probaEau += 10;
                    probaHerbe -= 2;
                    probaForet -= 2;
                    probaMontagne -= 2;
                    probaVille -= 2;
                    probaRoute -= 2;
                }
                else if (tile.Type == TileType.MONTAGNE)
                {
                    probaMontagne += 10;
                    probaHerbe -= 2;
                    probaForet -= 2;
                    probaEau -= 2;
                    probaVille -= 2;
                    probaRoute -= 2;
                }

            }
        }

    }
}
