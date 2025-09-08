using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Models;


namespace MyLittleRPG.Data.Context
{
    public class MonsterContext : DbContext
    {
        public DbSet<Monster> Monsters { get; set; }

        public MonsterContext(DbContextOptions<MonsterContext> options) : base(options) { }
    }
}
