using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservations.API.Extensions;
using Reservations.Application.ChargingStationsMicroService.Chargers;
using Reservations.Application.Reservations;
using Reservations.Application.ReservationSlots;
using Reservations.Application.Shared;
using Reservations.Application.Statuses;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using Reservations.Domain.StatusAggregate;
using Reservations.Infrastructure;
using Reservations.Infrastructure.Repositories;
using System;
using System.Net.Http;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

namespace Reservations.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>((_, options) =>
            {
                options.UseNpgsql(GetConnectionString());
            });

            var mapperConfig = CreateMapperConfiguration();
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IReservationSlotService, ReservationSlotService>();
            
            services.AddScoped<IChargerService, ChargerService>();

            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();

            Configuration["Consul:Host"] = Environment.GetEnvironmentVariable("HOST_IP");
            services.AddServiceDiscovery(options => options.UseConsul());

            // ChargingStations MS Client

            services.AddTransient<ChargingStationsMicroServiceClient>();

            services.AddHttpClient("chargers-dev", (_, client) =>
                {
                    SetHttpClientBaseAddress(client, new Uri(FormatConfigString(Configuration["ChargingStationsService:DevAddress"])));
                    SetHttpClientRequestHeader(client, "ReservationsMS");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                );

            services.AddHttpClient("chargers", (_, client) =>
                {
                    SetHttpClientBaseAddress(client, new Uri(FormatConfigString(Configuration["ChargingStationsService:Address"])));
                    SetHttpClientRequestHeader(client, "ReservationsMS");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                ).AddServiceDiscovery();

            services.AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });

            services.AddSwagger();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }

        private static string FormatConfigString(string connectionString)
        {
            return connectionString.Replace("\"", "");
        }

        private static void SetHttpClientRequestHeader(HttpClient client, string userAgent)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            client.DefaultRequestVersion = new Version(1, 0);
        }

        private static void SetHttpClientBaseAddress(HttpClient client, Uri baseAddress)
        {
            client.BaseAddress = baseAddress;
        }

        private static string GetConnectionString()
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var database = Environment.GetEnvironmentVariable("DB_NAME");
            var username = Environment.GetEnvironmentVariable("DB_USERNAME");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            return $"Host={host};Database={database};Username={username};Password={password}";
        }

        private static MapperConfiguration CreateMapperConfiguration()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ReservationMapperProfile());
                mc.AddProfile(new StatusMapperProfile());
            });

            return mapperConfig;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChargingStations.API v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
