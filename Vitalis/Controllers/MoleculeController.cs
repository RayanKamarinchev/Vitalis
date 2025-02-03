using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;
using Vitalis.Core.Models.GptResponses;

namespace Vitalis.Controllers
{
    [ApiController]
    [Route("mol")]
    public class MoleculeController : ControllerBase
    {
        private readonly IMoleculeService moleculeService;

        public MoleculeController(IMoleculeService _moleculeService)
        {
            moleculeService = _moleculeService;
        }

        [HttpGet("getReactions")]
        public IActionResult GetReactions(string reactant)
        {
            var res = moleculeService.GetPossibleReactions(reactant);
            return Ok(res);
        }

        [HttpGet("predictProduct")]
        public async Task<IActionResult> PredictProduct(string reactant, string reagent, string catalyst = "", string conditions = "",
            string followUp = "")
        {
            string product = await moleculeService.PredictProduct(reactant, reagent, catalyst, conditions, followUp);
            return Ok(product);
            //return "CC(Cl)C";
        }

        [HttpGet("info/{name}")]
        public async Task<IActionResult> Info(string name)
        {
            CompoundInfo info = await moleculeService.GetInfo(name);
            return Ok(info);
        }
    }
}
