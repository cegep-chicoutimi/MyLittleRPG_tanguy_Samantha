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

namespace MyLittleRPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly MonsterContext _context;
        private MonsterGeneration generator;

        public MonstersController(MonsterContext context)
        {
            _context = context;
            generator = new MonsterGeneration(context);
        }

        [HttpPut]
        [Route("monstre/generateall")]
        public async Task<IActionResult> generateall()
        {
            List<string> UsedXY = new List<string>();


            await _context.Database.ExecuteSqlRawAsync("DELETE FROM InstanceMonstres");
            for (int i = 0; i < 300; i++)
            {
                generator.addmonstre(UsedXY);
            }
            await _context.SaveChangesAsync();
            return Ok("300 monstre regenere");
        }
    }
}
