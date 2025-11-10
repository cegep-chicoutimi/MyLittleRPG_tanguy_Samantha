namespace MyLittleRPG.Models
{
    public class QueteVisiterTuile
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int DistanceX { get; set; }
        public int DistanceY { get; set; }
        public TileType TileType { get; set; }
        public int IdPersonnage { get; set; }
        public bool IsActive { get; set; }

        public QueteVisiterTuile() { }

        public QueteVisiterTuile(int x, int y,int distanceX, int distanceY, TileType type, int idPerso)
        {
            X = x;
            Y = y;
            DistanceX = distanceX;
            DistanceY = distanceY;
            TileType = type;
            IdPersonnage = idPerso;
            IsActive = true;
        }
    }
}
