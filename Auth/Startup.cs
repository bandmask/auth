using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using AspNetCore.Identity.Mongo;
using Auth.Filters;
using Auth.Models;
using Auth.Settings;
using Auth.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"Config/appSettings.json", optional : false)
                .AddJsonFile($"Config/jwtSignerSettings.json", optional : false)
                .AddJsonFile($"Config/mongoDbSettings.json", optional : false)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(corsPolicy => corsPolicy.AddPolicy("allowAllPolicy",
                builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithOrigins("https://*.ropr.se ")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .Build()
            ));

            var jwtIssuerOptions = new JwtIssuerOptions(Configuration.GetSection(nameof(JwtIssuerOptions)));
            services.AddTransient<JwtIssuerOptions>(provider => jwtIssuerOptions);

            var connectionsOptions = new ConnectionsOptions(Configuration.GetSection(nameof(ConnectionsOptions)));
            services.AddIdentityMongoDbProvider<AppUser>(dbIdentityOptions =>
            {
                dbIdentityOptions.ConnectionString = connectionsOptions.MongoDbConnection;
            });

            services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.RequireHttpsMetadata = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuerOptions.Issuer,
                        ValidAudiences = jwtIssuerOptions.Audiences,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuerOptions.SigningKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddMvc(config =>
            {
                config.Filters.Add(new ClientFilterAttribute(allowedClients: jwtIssuerOptions.Audiences));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("allowAllPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
