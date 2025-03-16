using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CryptoWallet.Services
{
    public class CryptoService
    {
        private readonly HttpClient _httpClient;

        public CryptoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetCoinPriceAsync(string coinName)
        {
            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinName.ToLower()}&vs_currencies=usd";
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            return json[coinName.ToLower()]?["usd"]?.Value<decimal>() ?? 0;
        }
    }
}
