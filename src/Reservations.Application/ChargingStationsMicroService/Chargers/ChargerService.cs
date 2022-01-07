using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ChargerService> _logger;

        public ChargerService(ChargingStationsMicroServiceClient chargingStationsMicroServiceClient, ILogger<ChargerService> logger)
        {
            _chargingStationsMicroServiceClient = chargingStationsMicroServiceClient;
            _logger = logger;
        }

        public async Task<List<ChargerDto>> GetAsync(List<int> chargerIds)
        {
            var endpoint = $"endpoint Chargers?chargerIds={string.Join(",", chargerIds)}";

            try
            {
                _logger.LogInformation($"Entered Charging Stations MS {endpoint}");

                var responseMessage = _chargingStationsMicroServiceClient.Client.GetAsync($"Chargers?chargerIds={string.Join(",", chargerIds)}").Result;

                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"Exited Charging Stations MS {endpoint} with: 404 no Charger found");

                    return new List<ChargerDto>();
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }

                await using var responseStream = await responseMessage.Content.ReadAsStreamAsync();

                var chargerDtos = await JsonSerializer.DeserializeAsync<List<ChargerDto>>(responseStream);

                _logger.LogInformation($"Exited Charging Stations MS {endpoint} with: 200 OK");

                return chargerDtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited Comments MS {endpoint} with: Exception {e.Message}");
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
