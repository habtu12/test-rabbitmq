using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Perago.SharedKernel.BroadcastRabbitMQ;
using Perago.SharedKernel.BroadcastRabbitMQ.Bus;
using Perago.SharedKernel.RabbitMQ;
using Perago.SharedKernel.RabbitMQ.Bus;
using Perago.SharedKernel.RabbitMQ.Configuration;
using RabbitMQ.Client;
using System;
using Test.RabbitMQ.API.IntegrationEvents;
using Test.RabbitMQ.API.Models;
using Test.RabbitMQ.API.Services;

namespace Test.RabbitMQ.API
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
            services.AddEntityFrameworkNpgsql()
                   .AddDbContext<InventoryContext>(options => options.UseNpgsql(Configuration.GetConnectionString("InventoryDatabase")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test RabbitMQ", Version = "v1" });
            });
            services.AddTransient<IProductService, ProductService>();
            services.AddEventBus(Configuration);
            services.AddBroadcastEventBus(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test RabbitMQ v1"));
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.ConfigureEventBus();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            //Event Bus
            var rabbitMQConfiguration = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
            services.AddSingleton(rabbitMQConfiguration);

            //Domain Bus
            services.AddTransient<IEventBus, RabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMQConfiguration.HostName,
                    VirtualHost = rabbitMQConfiguration.VirtualHost,
                    UserName = rabbitMQConfiguration.UserName,
                    Password = rabbitMQConfiguration.Password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(10),
                    DispatchConsumersAsync = true
                };

                return new RabbitMQBus(scopeFactory, factory);
            });

            //Subscriptions
            services.AddTransient<ProductIntegrationEventHandler>();


            //Integration Events
            services.AddTransient<IEventHandler<ProductIntegrationEvent>, ProductIntegrationEventHandler>();

            return services;
        }

        public static IServiceCollection AddBroadcastEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            //Event Bus
            var rabbitMQConfiguration = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
            services.AddSingleton(rabbitMQConfiguration);

            //Domain Bus
            services.AddTransient<IBroadcastEventBus, BroadcastRabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMQConfiguration.HostName,
                    VirtualHost = rabbitMQConfiguration.VirtualHost,
                    UserName = rabbitMQConfiguration.UserName,
                    Password = rabbitMQConfiguration.Password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(10),
                    DispatchConsumersAsync = true
                };

                return new BroadcastRabbitMQBus(scopeFactory, factory);
            });

            return services;
        }

        public static void ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductIntegrationEvent, ProductIntegrationEventHandler>();
        }
    }
}
