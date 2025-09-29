using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG.Models
{
    [PrimaryKey(nameof(PositionX), nameof(PositionY))]
    public class InstanceMonster
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public Monster Monster { get; set; }
        public int niveaux { get; set; }
        public int PVMax { get; set; }
        public int PVactuels { get; set; }

    }
}
