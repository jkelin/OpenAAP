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
using OpenAAP.Services.Session;

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
            services.Configure<HashingOptions>(Configuration);
            services.Configure<Options.SessionOptions>(Configuration);

            services.AddSingleton<ISessionStorageService, InMemorySessionStorageService>();
            services.AddSingleton<PasswordHashingService, PasswordHashingService>();
            services.AddSingleton<SHA1PasswordHashingService, SHA1PasswordHashingService>();

            services.AddDbContext<OpenAAPContext>(opt => opt.UseInMemoryDatabase("OpenAPP"));
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
                context.Database.EnsureCreated();
            }
        }
    }
}
