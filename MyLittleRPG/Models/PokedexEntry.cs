using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG.Models
{
    [PrimaryKey(nameof(PersonnageId), nameof(MonsterId))]
    public class PokedexEntry
    {
        public int PersonnageId { get; set; }
        public int MonsterId { get; set; }
        [Required]
        public Personnage Personnage { get; set; }

        [Required]
        public Monster Monster { get; set; }

        public PokedexEntry() { }

        public PokedexEntry(Personnage personnage, Monster monster)
        {
            Personnage = personnage;
            Monster = monster;
            PersonnageId = personnage.Id;
            MonsterId = monster.Id;
        }
    }
}
