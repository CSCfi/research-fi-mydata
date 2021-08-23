using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Services;
using api.Models.Ttv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.HttpOverrides;

namespace api
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");


            services.AddDbContext<TtvContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddControllers();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["OAUTH:AUTHORITY"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1");
                });
            });

            // CORS policies
            services.AddCors(options =>
            {
                // Development and testing
                options.AddPolicy("development", builder =>
                {
                    builder.WithOrigins(
                        "https://*.csc.fi",
                        "https://*.rahtiapp.fi",
                        "https://localhost:5003"
                    )
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });

                // Production - TODO remove localhost 
                options.AddPolicy("production", builder =>
                {
                    builder.WithOrigins(
                        "https://*.csc.fi",
                        "https://*.rahtiapp.fi",
                        "https://localhost:5003"
                    )
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddResponseCompression();
            services.AddHttpClient<OrcidApiService>();
            services.AddScoped<OrcidJsonParserService>();
            services.AddScoped<UserProfileService>();
            services.AddScoped<DemoDataService>();

            /*
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
            });
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DemoDataService demoDataService)
        {
            // Use Forwarded Headers Middleware to enable client ip address detection behind load balancer.
            // Must run this before other middleware.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Add registered data sources, organizations etc. needed in demo.
            // Most of demo data is added to each user, who creates a profile.
            demoDataService.InitDemo();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
            }

            app.UseResponseCompression();
            app.UseRouting();

            // CORS policy depends on the environment
            if (env.IsDevelopment())
            {
                app.UseCors("development");
            }
            if (env.IsProduction())
            {
                app.UseCors("production");
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
