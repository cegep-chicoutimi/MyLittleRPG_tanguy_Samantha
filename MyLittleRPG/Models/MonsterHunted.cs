using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MyLittleRPG.Models
{
    public class MonsterHunted
    {
        public int Id { get; set; }

        public int IdMonstre {  get; set; }

        public int IdPerso { get; set; }
        public MonsterHunted() { }

        public MonsterHunted(int id, int idMonstre, int idperso) 
        { 
            Id = id; 
            IdMonstre = idMonstre;
            IdPerso = idperso;
        }
    }
}
