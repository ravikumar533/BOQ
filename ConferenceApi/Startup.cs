using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceApi.Handlers;
using ConferenceApi.Models.Config;
using ConferenceApi.Services;
using ConferenceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConferenceApi
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
            var apiSettingsSection = Configuration.GetSection(nameof(ApiSettings));
            services.Configure<ApiSettings>(apiSettingsSection);
           
            var authSection = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(authSection);

            var keySettingsSection = Configuration.GetSection(nameof(ConferenceKeySettings));
            services.Configure<ConferenceKeySettings>(keySettingsSection);

            var routeSettingsSection = Configuration.GetSection(nameof(ConferenceRouteSettings));
            services.Configure<ConferenceRouteSettings>(routeSettingsSection);

            var cacheSettingsSection = Configuration.GetSection(nameof(CacheSettings));
            services.Configure<CacheSettings>(cacheSettingsSection);

            var configKeySettings = keySettingsSection.Get<ConferenceKeySettings>();

            services.AddHttpClient<IHttpClientService, HttpClientService>(client =>
            {
                client.DefaultRequestHeaders.Add(configKeySettings.SubscriptionKeyName, configKeySettings.SubscriptionKeyValue);
            });
            services.AddSingleton<IConferenceService, ConferenceService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Demo Conference Api", Version = "v1" });
                c.AddSecurityDefinition("Basic", new ApiKeyScheme()
                {
                    Description = "Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = "header",
                    Type= "basic"
                });
                c.DocumentFilter<SwaggerSecurityRequirementsDocumentFilter>();
            });

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Conference Api V1");
            });

            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        public class SwaggerSecurityRequirementsDocumentFilter : IDocumentFilter
        {
            public void Apply(SwaggerDocument document, DocumentFilterContext context)
            {
                document.Security = new List<IDictionary<string, IEnumerable<string>>>()
            {
                new Dictionary<string, IEnumerable<string>>()
                {
                    { "Bearer", new string[]{ } },
                    { "Basic", new string[]{ } },
                }
            };
            }
        }
    }
}
