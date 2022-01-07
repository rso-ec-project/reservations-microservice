using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Winton.Extensions.Configuration.Consul;
using Winton.Extensions.Configuration.Consul.Parsers;

namespace Reservations.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (_, builder) =>
                    {
                        var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false)
                            .Build();

                        config["Consul:Host"] = Environment.GetEnvironmentVariable("HOST_IP");
                        var consulHost = config["Consul:Host"];
                        var consulPort = config["Consul:Port"];

                        builder.AddConsul(
                            "ReservationsMicroservice",
                            options =>
                            {
                                options.ConsulConfigurationOptions =
                                    cco => { cco.Address = new Uri($"http://{consulHost}:{consulPort}"); };
                                options.Optional = false;
                                options.ReloadOnChange = true;
                                options.Parser = new SimpleConfigurationParser();
                            });
                    }
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((ctx, config) => { config.ReadFrom.Configuration(ctx.Configuration); });
    }
}
