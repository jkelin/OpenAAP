using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAAP.Context;
using OpenAAP.Helpers;
using OpenAAP.Options;
using OpenAAP.Services.PasswordHashing;
using OpenAAP.Services.SessionDataStorage;
using OpenAAP.Services.SessionStorage;
using StackExchange.Redis;

namespace OpenAAP
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
            services.Configure<HashingOptions>(Configuration.GetSection(HashingOptions.Section));
            services.Configure<Options.SessionOptions>(Configuration.GetSection(Options.SessionOptions.Section));
            services.Configure<DatabaseOptions>(Configuration.GetSection(DatabaseOptions.Section));

            
            services.AddTransient<PasswordHashingService, PasswordHashingService>();
            services.AddTransient<SHA1PasswordHashingService, SHA1PasswordHashingService>();
            services.AddTransient<PBKDF2PasswordHashingService, PBKDF2PasswordHashingService>();
            services.AddTransient<SessionService, SessionService>();

            ConfigureDatabase(services);
            ConfigureSessionStore(services);

            services.AddMvc(options =>
            {
                options.Filters.Add(new ModelStateValidationFilter());
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(Dictionary<string, string[]>), 400));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                // options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
        }

        void ConfigureDatabase(IServiceCollection services)
        {
            var opts = Configuration.GetSection(DatabaseOptions.Section).Get<DatabaseOptions>();

            switch (opts.Type)
            {
                case DatabaseType.InMemory:
                    services.AddDbContext<OpenAAPContext>(opt => opt.UseInMemoryDatabase("OpenAPP"));
                    break;
                case DatabaseType.Sqlite:
                    services.AddDbContext<OpenAAPContext>(opt => opt.UseSqlite(opts.ConnectionStringSqlite));
                    break;
                case DatabaseType.SqlServer:
                    services.AddDbContext<OpenAAPContext>(opt => opt.UseSqlServer(opts.ConnectionStringSqlServer));
                    break;
                case DatabaseType.Postgres:
                    services.AddDbContext<OpenAAPContext>(opt => opt.UseNpgsql(opts.ConnectionStringPostgres));
                    break;
            }
        }

        void ConfigureSessionStore(IServiceCollection services)
        {
            var opts = Configuration.GetSection(Options.SessionOptions.Section).Get<Options.SessionOptions>();

            switch (opts.SessionStoreType)
            {
                case SessionStoreType.InMemory:
                    services.AddSingleton<ISessionDataStorage, InMemorySessionDataStorageService>();
                    break;
                case SessionStoreType.Redis:
                    var redis = ConnectionMultiplexer.Connect(opts.ConnectionStringRedis);
                    services.AddSingleton(_ => redis);
                    services.AddTransient(_ => redis.GetDatabase());
                    services.AddTransient<ISessionDataStorage, RedisSessionDataStorageService>();
                    break;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<OpenAAPContext>();
                var dbOptions = Configuration.GetSection(DatabaseOptions.Section).Get<DatabaseOptions>();

                if (dbOptions.Type == DatabaseType.InMemory)
                {
                    context.Database.EnsureCreated();
                }
                else
                {
                    context.Database.Migrate();
                }

                if (dbOptions.SeedWithTestData ?? false)
                {
                    context.Seed().Wait();
                }
            }
        }
    }
}
