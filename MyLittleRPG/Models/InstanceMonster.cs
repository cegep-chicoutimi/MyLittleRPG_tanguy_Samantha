using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG.Models
{
    [PrimaryKey(nameof(X), nameof(Y))]
    public class InstanceMonster
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int MonsterId { get; set; }
        public Monster Monster { get; set; }
        public int niveaux { get; set; }
        public int PVMax { get; set; }
        public int PVactuels { get; set; }



    }
}
