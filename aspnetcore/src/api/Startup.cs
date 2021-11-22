using api.Services;
using api.Models.Ttv;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // SQL Server options.
            // Enable retry on failure.
            // Split LINQ queriers into multiple SQL queries.
            services.AddDbContext<TtvContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure();
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                )
            );

            services.AddControllers();

            // Swagger documentation
            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Mydata API",
                        Description = "An API for Mydata frontend.",
                        Version = "v1"
                    });
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                    });

                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }

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

            services.AddHttpClient<OrcidApiService>();
            services.AddScoped<OrcidJsonParserService>();
            services.AddScoped<UserProfileService>();
            services.AddSingleton<ElasticsearchService>();
            services.AddSingleton<UtilityService>();
            services.AddSingleton<LanguageService>();
            services.AddScoped<DemoDataService>();
            services.AddScoped<TtvSqlService>();
            services.AddMemoryCache();

            services.AddHostedService<BackgroundElasticsearchUpdateService>();
            services.AddSingleton<BackgroundElasticsearchPersonUpdateQueue>();
            services.AddTransient<BackgroundProfiledata>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DemoDataService demoDataService)
        {
            // Response compression.
            app.UseResponseCompression();

            // Use Forwarded Headers Middleware to enable client ip address detection behind load balancer.
            // Must run this before other middleware.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Add registered data sources, organizations etc. needed in demo.
            // Most of demo data is added to each user, who creates a profile.
            demoDataService.InitDemo();

            // Development environment settings
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            // CORS policy depends on the environment
            if (Environment.IsDevelopment())
            {
                app.UseCors("development");
            }
            if (Environment.IsProduction())
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
