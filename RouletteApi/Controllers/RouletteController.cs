using Microsoft.AspNetCore.Mvc;
using RouletteApi.Interface;
using RouletteApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouletteApi.Controllers
{
    [Produces ("application/json")]
    public class RouletteController : ControllerBase
    {
        private readonly IRouletteRepository _rouletteRepo;

        // now we have used DI to inject the instance once only for the lifetime (transient)
        public RouletteController(IRouletteRepository rouletteRepo)
        {
            _rouletteRepo = rouletteRepo;
        }
        // now we can write out 4 endpoints PlaceBet, Spin, Payout and ShowPreviousSpins
        // all implementations of Roullte Interface will be accesibble via _rouletteRepo
        [HttpGet]
        [Route("api/[controller]/spin")]
        public Task<int> Spin()
        {
            return _rouletteRepo.SpinAsync();
        }

        [HttpGet]
        [Route("api/[controller]/payout")]
        public IActionResult Payout(decimal stake)
        {
            return Ok(_rouletteRepo.Payout(stake));
        }

        [HttpGet]
        [Route("api/[controller]/previousspins")]
        public async Task<List<int>> ShowPreviousSpins()
        {
            var result = _rouletteRepo.PreviousSpinsAsync();
            return await result;
        }

        [HttpPost]
        [Route("api/[controller]/placebet")]
        public Task<PlaceBetResponse> PlaceBet(int selection, decimal stake)
        {
            return _rouletteRepo.PlaceBetAsync(selection, stake);
        }
    }
}
