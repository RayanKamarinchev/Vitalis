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
        private readonly IProductPredictionService productPredictionService;

        public MoleculeController(IMoleculeService _moleculeService, IProductPredictionService _productPredictionService)
        {
            moleculeService = _moleculeService;
            productPredictionService = _productPredictionService;
        }

        [HttpGet("getReactions")]
        public IActionResult GetReactions(string reactant)
        {
            var res = productPredictionService.GetPossibleReactions(reactant);
            return Ok(res);
        }

        [HttpPost("predictProduct")]
        public IActionResult PredictProduct([FromBody] ProductReactionQueryModel model)
        {
            Reaction reaction = new Reaction(model.Reagent, model.Catalyst, model.Conditions, model.FollowUp);
            string product = productPredictionService.PredictProduct(model.Reactant, model.Smiles, reaction);
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
