﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using identityserver.Data;
using identityserver.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using identityserver.Services;
using Microsoft.AspNetCore.HttpOverrides;
using IdentityServer4.Extensions;
using IdentityServer4.Models;

namespace identityserver
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor; // | ForwardedHeaders.XForwardedProto;
            });


            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                // options.UseSqlite(connectionString));
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                //.AddInMemoryIdentityResources(Config.IdentityResources)
                //.AddInMemoryApiScopes(Config.ApiScopes)
                //.AddInMemoryClients(Config.Clients)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddOpenIdConnect("oidc", "ORCID", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://orcid.org";
                    options.ClientId = Configuration["ORCID:ClientId"];
                    options.ClientSecret = Configuration["ORCID:ClientSecret"];
                    options.ResponseType = OidcConstants.ResponseTypes.Code;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                        RequireExpirationTime = false,
                    };
                });

            services.AddScoped<IProfileService, ProfileService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            // This setting enables IdentityServer to work behind a reverse proxy.
            // The domain of the IdentityServer must be set in configuration.
            app.Use(async (ctx, next) =>
            {
                ctx.SetIdentityServerOrigin(Configuration["IdentityServer:Origin"]);
                await next();
            });

            // Initialize IdentityServer database
            InitializeDatabase(app);

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        // Initialize IdentityServer database
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                // Remove old Javascript client to ensure AllowedCorsOrigins, RedirectUris and PostLogoutRedirectUris are updated
                var oldJsClient = context.Clients.FirstOrDefault(c => c.ClientId == "js");
                if (oldJsClient != null)
                {
                    context.Clients.Remove(oldJsClient);
                    context.SaveChanges();
                }

                // New Javascript client
                var newJsClient = new Client()
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris = { Configuration["JavaScriptClient:Dev:RedirectUri"], Configuration["JavaScriptClient:Test:RedirectUri"] },
                    PostLogoutRedirectUris = { Configuration["JavaScriptClient:Dev:PostLogoutRedirectUri"], Configuration["JavaScriptClient:Test:PostLogoutRedirectUri"] },
                    AllowedCorsOrigins = { Configuration["JavaScriptClient:Dev:AllowedCorsOrigin"], Configuration["JavaScriptClient:Test:AllowedCorsOrigin"] },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AlwaysIncludeUserClaimsInIdToken = true
                };
                context.Clients.Add(newJsClient.ToEntity());
                context.SaveChanges();
                

                // Add identity resource - OpenId
                var newIdentityResourceOpenId = new IdentityResources.OpenId();
                var existingIdentityResourceOpenId = context.IdentityResources.FirstOrDefault(i => i.Name == newIdentityResourceOpenId.Name);
                if (existingIdentityResourceOpenId == null)
                {
                    context.IdentityResources.Add(newIdentityResourceOpenId.ToEntity());
                    context.SaveChanges();
                }

                // Add identity resource - Profile
                var newIdentityResourceProfile = new IdentityResources.Profile();
                var existingIdentityResourceProfile = context.IdentityResources.FirstOrDefault(i => i.Name == newIdentityResourceProfile.Name);
                if (existingIdentityResourceProfile == null)
                {
                    context.IdentityResources.Add(newIdentityResourceProfile.ToEntity());
                    context.SaveChanges();
                }

                // Add api scope
                var newApiScope = new ApiScope("api1", "Researcher profile API");
                var existingApiScope = context.ApiScopes.FirstOrDefault(a => a.Name == newApiScope.Name);
                if (existingApiScope == null)
                {
                    context.ApiScopes.Add(newApiScope.ToEntity());
                    context.SaveChanges();
                }
            }
        }
    }
}