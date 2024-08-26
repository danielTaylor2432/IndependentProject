using Microsoft.AspNetCore.Mvc;
using MingweiSamuel.Camille.Enums;
using YourProjectNamespace.Services; // Ensure this matches your namespace

public class SummonerController : Controller
{
    private readonly RiotApiService _riotApiService;

    // Use Dependency Injection to pass the RiotApiService to the controller
    public SummonerController(RiotApiService riotApiService)
    {
        _riotApiService = riotApiService;
    }

    public IActionResult Details(string summonerName)
    {
        var region = Region.NA; // Example: Set your region here
        var summoner = _riotApiService.GetSummonerByName(summonerName, region);

        if (summoner == null)
        {
            return NotFound("Summoner not found.");
        }

        return View(summoner);
    }
}
