using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Reservations.Application.Shared
{
    public class ChargingStationsMicroServiceClient
    {
        public HttpClient Client { get; set; }

        public ChargingStationsMicroServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            var chargersServiceEnv = configuration["ChargingStationsService:Environment"].Replace("\"", "");
            Client = httpClientFactory.CreateClient(chargersServiceEnv.Equals("dev") ? "chargers-dev" : "chargers");
        }
    }
}
