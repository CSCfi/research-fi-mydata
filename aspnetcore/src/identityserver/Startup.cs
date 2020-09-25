// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
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
                    options.ClientId = "APP-DYHMDDN3DT72WR5Y";
                    options.ClientSecret = "95c0a794-cb06-487f-b197-5573da01e3cf";
                    options.ResponseType = OidcConstants.ResponseTypes.Code;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                        RequireExpirationTime = false,
                    };

                    // Use custom JSON Web Token validator for ORCID JWTs. See reason in OrcidJwtValidator comments.
                    options.ProtocolValidator = new OrcidJwtValidator
                    {
                        RequireStateValidation = options.ProtocolValidator.RequireStateValidation,
                        NonceLifetime = options.ProtocolValidator.NonceLifetime
                    };
                });
        }

        public void Configure(IApplicationBuilder app)
        {
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
    }

    public class OrcidJwtValidator : OpenIdConnectProtocolValidator
    {
        // ORCID tokens have long expiry time (20 years).
        // OpenIdConnectProtocolValidator.ValidateIdToken can only handle 32bit long expiry values,
        // which reach only up to Jan 2038.
        // ORCID login attempt will throw an error because Microsoft platform cannot handle ORCID's exp value.
        //
        // Workaround: If expiry value is higher than 2147483647, then add a new exp claim using value 2147483647.
        // The workaround should be removed if ORCID starts using short lived tokens, or Microsoft platform is
        // fixed to handle values higher than 2147483647.
        protected override void ValidateIdToken(OpenIdConnectProtocolValidationContext validationContext)
        {
            var expValueStrOriginal = validationContext.ValidatedIdToken.Claims.First(claim => claim.Type == "exp").Value;
            long expValueLong = Convert.ToInt64(expValueStrOriginal);

            if (expValueLong > 2147483647)
                validationContext.ValidatedIdToken.Payload.AddClaim(new Claim("exp", "2147483647"));

            base.ValidateIdToken(validationContext);
        }
    }
}