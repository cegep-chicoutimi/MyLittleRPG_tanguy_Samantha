using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG.Models
{
    [PrimaryKey(nameof(Tile))]
    public class InstanceMonster
    {
        public Tile Tile { get; set; }
        public Monster Monster { get; set; }
        public int niveaux { get; set; }
        public int PVMax { get; set; }
        public int PVactuels { get; set; }

    }
}
