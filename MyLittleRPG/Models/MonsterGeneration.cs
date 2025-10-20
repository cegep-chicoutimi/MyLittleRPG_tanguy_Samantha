using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Migrations;
using MyLittleRPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLittleRPG.Models
{
    public class MonsterGeneration
    {
        private readonly MonsterContext _context;

        public MonsterGeneration(MonsterContext _context) { this._context = _context; }
        public bool generate10()
        {
            List<string> UsedXY = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                addmonstre(UsedXY);
            }
            
            _context.SaveChanges();
            return true;
        }

        public void addmonstre(List<string> UsedXY)
        {
            Random random = new Random();
            int x;
            int y;
            string verif;

            do
            {

                x = random.Next(1, 50);
                y = random.Next(1, 50);
                verif = x + "," + y;
            } while (UsedXY.Contains(verif) || !isSpawnable(x, y));

            UsedXY.Add(x + "," + y);

            Monster monster = getRandomMonstre();
            int level = getDistanceVille(x, y) / 3;

            InstanceMonster monsterInstance = new InstanceMonster();

            monsterInstance.X = x;
            monsterInstance.Y = y;
            monsterInstance.MonsterId = monster.Id;
            monsterInstance.Monster = monster;
            monsterInstance.niveaux = level;
            monsterInstance.PVMax = monster.pointsVieBase;
            monsterInstance.PVactuels = monster.pointsVieBase;

            Console.WriteLine("ajout de " + monster.Nom + "de niveaux " + level + "a la bd a la case " + x + " ; " + y);

            _context.InstanceMonstres.Add(monsterInstance);
        }

        private bool isSpawnable(int x, int y)
        {

            var tile = _context.Tiles.Find(x, y);
            if (tile == null)
                return false;

            if (!tile.estTraversable)
                return false;

            if (tile.Type == TileType.VILLE || tile.Type == TileType.ROUTE)
                return false;

            return true;
        }

        private int getDistanceVille(int x, int y)
        {
            int distance = 1000;
            foreach (var tile in _context.Tiles)
            {
                if (tile.Type == TileType.VILLE)
                {
                    int newDistance = Math.Abs(x - tile.X) + Math.Abs(y - tile.Y);
                    if (newDistance < distance) { distance = newDistance; }
                }
            }

            return distance;
            throw new NotImplementedException();
        }

        private Monster getRandomMonstre()
        {
            Random random = new Random();

            int id = random.Next(1, 809);
            return _context.Monsters.Find(id);
        }
    }
}
