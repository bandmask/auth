using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspNetCore.Identity.Mongo;
using Auth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(corsPolicy => corsPolicy.AddPolicy("allowAllPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var jwtIssuerOptions = GetJwtIssuerOptions(jwtAppSettingOptions);

            services.AddSingleton<JwtIssuerOptions>(jwtIssuerOptions);
            services.AddIdentityMongoDbProvider<AppUser>(dbIdentityOptions =>
            {
                dbIdentityOptions.ConnectionString = "mongodb://localhost/auth";
            });

            services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.RequireHttpsMetadata = false;

                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuerOptions.Issuer,
                        ValidAudience = jwtIssuerOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuerOptions.SigningKey)),
                        TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuerOptions.SigningDecryption)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("allowAllPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }

        private JwtIssuerOptions GetJwtIssuerOptions(IConfigurationSection jwtAppSettingOptions)
        {
            var jwtIssuerOptions = new JwtIssuerOptions
            {
                Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                Authority = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                SigningKey = jwtAppSettingOptions[nameof(JwtIssuerOptions.SigningKey)],
                SigningDecryption = jwtAppSettingOptions[nameof(JwtIssuerOptions.SigningDecryption)]
            };

            return jwtIssuerOptions;
        }
    }
}
