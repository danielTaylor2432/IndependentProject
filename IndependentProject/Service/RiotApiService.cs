using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.SummonerV4;
using Microsoft.Extensions.Configuration;

namespace YourProjectNamespace.Services
{
    public class RiotApiService
    {
        private readonly RiotApi _riotApi;

        public RiotApiService(IConfiguration configuration)
        {
            string apiKey = configuration["RiotApi:ApiKey"];
            _riotApi = RiotApi.NewInstance(apiKey);
        }
        //Rat
        public Summoner GetSummonerByName(string summonerName, Region region)
        {
            return _riotApi.SummonerV4.GetBySummonerName(region, summonerName);
        }
    }
}
