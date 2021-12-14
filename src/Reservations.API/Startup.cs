using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Reservations.Application.Reservations;
using Reservations.Application.Statuses;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using Reservations.Domain.StatusAggregate;
using Reservations.Infrastructure;
using Reservations.Infrastructure.Repositories;
using System;

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
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(GetConnectionString());
            });

            var mapperConfig = CreateMapperConfiguration();
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IStatusService, StatusService>();

            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reservations.API", Version = "v1" });
            });
        }

        private string GetConnectionString()
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reservations.API v1"));
            }

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
