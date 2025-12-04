using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Models;


namespace MyLittleRPG.Data.Context
{
    public class MonsterContext : DbContext
    {
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<PokedexEntry> Pokedex { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<InstanceMonster> InstanceMonstres { get; set; }

        public DbSet<Utilisateur> Utilisateurs { get; set; }

        public DbSet<Personnage> Personnages { get; set; }

        public DbSet<QueteNiveauAtteint> QuetesNiveauAtteint { get; set; }
        public DbSet<QueteVaincreMonstres> QuetesVaincreMonstres { get; set; }

        public DbSet<QueteVisiterTuile> QuetesVisiterTuile { get; set; }
        public MonsterContext(DbContextOptions<MonsterContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InstanceMonster>()
                .HasOne(im => im.Monster)
                .WithMany()
                .HasForeignKey(im => im.MonsterId);

            modelBuilder.Ignore<QuestDTO>();
        }
        public DbSet<MyLittleRPG.Models.QuestDTO> QuestDTO { get; set; } = default!;
    }
}
