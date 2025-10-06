using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MyLittleRPG.Data.Context;
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

        public MonstersController(MonsterContext context)
        {
            _context = context;
        }

        [HttpPut]
        [Route("monstre/generateall")]
        public async Task<IActionResult> generateall()
        {
            return BadRequest("not imlemented");
        }

        [HttpPut]
        [Route("monstre/generate10")]
        public async Task<IActionResult> generate10()
        {
            return BadRequest("not imlemented");
        }
    }
}
