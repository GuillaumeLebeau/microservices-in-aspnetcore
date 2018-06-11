using System;
using System.Reflection;

using DbUp;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Npgsql;

using Polly;
using ShoppingCart.Clients;
using ShoppingCart.Stores;

namespace ShoppingCart
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IEventStore>(
                _ => new Stores.EventStore(Configuration["EventStoreConnectionString"])
            );

            services.AddSingleton<IShoppingCartStore>(
                _ => new ShoppingCartStore(Configuration["ConnectionString"])
            );

            services.AddSingleton<ICache, Cache>();
            services.AddSingleton<IProductCatalogClient>(
                prov => new ProductCatalogClient(Configuration["ProductCatalogUrl"], prov.GetService<ICache>())
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();

            WaitForSqlAvailabilityAsync(Configuration["ConnectionString"], loggerFactory, 2);
        }

        private void WaitForSqlAvailabilityAsync(string connectionString, ILoggerFactory loggerFactory, int retries = 0)
        {
            var logger = loggerFactory.CreateLogger(nameof(Startup));
            var policy = CreatePolicy(retries, logger, nameof(WaitForSqlAvailabilityAsync));
            policy.Execute(
                () =>
                {
                    EnsureDatabase.For.PostgresqlDatabase(connectionString);

                    var upgrader = DeployChanges.To.PostgresqlDatabase(connectionString, "shopcart")
                        .WithScriptsEmbeddedInAssembly(Assembly.GetEntryAssembly())
                        .LogToConsole()
                        .Build();

                    var result = upgrader.PerformUpgrade();

                    if (!result.Successful)
                        throw result.Error;

                }
            );
        }
        
        private Policy CreatePolicy(int retries, ILogger logger, string prefix)
        {
            return Policy.Handle<PostgresException>().
                WaitAndRetry(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}")
                );
        }
    }
}
