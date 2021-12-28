using System.Net.Http;

namespace Reservations.Application.Shared
{
    public class ChargingStationsMicroServiceClient
    {
        public HttpClient Client { get; set; }

        public ChargingStationsMicroServiceClient(HttpClient client)
        {
            Client = client;
        }
    }
}
