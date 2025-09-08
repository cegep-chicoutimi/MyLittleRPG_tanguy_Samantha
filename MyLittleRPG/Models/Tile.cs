using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG.Models
{
    [PrimaryKey(nameof(PositionX), nameof(PositionY))]
    public class Tile
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public TileType Type { get; set; }
        public bool estTraversable { get; set; }
        public string imageURL { get; set; }

        public Tile( int PositionX,int PositionY, TileType Type,bool estTraversable,string imageURL)
        {
            this.PositionX = PositionX;
            this.PositionY = PositionY;
            this.Type = Type;
            this.estTraversable = estTraversable;
            this.imageURL = imageURL;
       
        }
    }
}
