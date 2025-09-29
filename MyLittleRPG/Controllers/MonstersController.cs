using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG.Data.Context;
using MyLittleRPG.Models;

namespace MyLittleRPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly MonsterContext _context;

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

        [HttpPost]
        [Route("monstre/fight/{x}/{y}")]
        public async Task<ActionResult<ResultDto>> fight(int x, int y)
        {
            return BadRequest("not imlemented");
        }

    }
}
