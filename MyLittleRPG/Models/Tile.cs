using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG.Models
{
    [PrimaryKey(nameof(X), nameof(Y))]
    public class Tile
    {
        public int X { get; set; }
        public int Y{ get; set; }
        public TileType Type { get; set; }
        public bool estTraversable { get; set; }
        public string imageURL { get; set; }

        public Tile( int X,int Y, TileType Type,bool estTraversable,string imageURL)
        {
            this.X = X;
            this.Y = Y;
            this.Type = Type;
            this.estTraversable = estTraversable;
            this.imageURL = imageURL;
       
        }
    }
}
