using Reservations.Application.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reservations.Application.ChargingStationsMicroService.Chargers
{
    public class ChargerService : IChargerService
    {
        private readonly ChargingStationsMicroServiceClient _chargingStationsMicroServiceClient;

        public ChargerService(ChargingStationsMicroServiceClient chargingStationsMicroServiceClient)
        {
            _chargingStationsMicroServiceClient = chargingStationsMicroServiceClient;
        }

        public async Task<List<ChargerDto>> GetAsync(List<int> chargerIds)
        {
            try
            {
                var responseMessage = _chargingStationsMicroServiceClient.Client.GetAsync($"Chargers?chargerIds={string.Join(",", chargerIds)}").Result;

                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<ChargerDto>();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }

                await using var responseStream = await responseMessage.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<List<ChargerDto>>(responseStream);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<(ChargerDto, HttpStatusCode?)> GetAsync(int chargerId)
        {
            try
            {
                var responseMessage = _chargingStationsMicroServiceClient.Client.GetAsync($"Chargers/{chargerId}").Result;

                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    return (null, responseMessage.StatusCode);
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }

                await using var responseStream = await responseMessage.Content.ReadAsStreamAsync();

                return (await JsonSerializer.DeserializeAsync<ChargerDto>(responseStream), responseMessage.StatusCode);
            }
            catch (Exception e)
            {
                return (null, null);
            }
        }
    }
}
