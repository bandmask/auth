using System;
using System.Security.Cryptography;
using Auth.DataAccess.Contexts;
using Auth.DataAccess.Stores;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<UserContext, UserContext>();
            services.AddTransient<UserStore, UserStore>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = _validIssuer;
                configureOptions.TokenValidationParameters = GetTokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();
        }

        private TokenValidationParameters GetTokenValidationParameters => new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _validIssuer,

            ValidateAudience = true,
            ValidAudience = _validAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(new RSACryptoServiceProvider(512)), //_signingKey

            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        private const string _validIssuer = "ropr.se";
        private const string _validAudience = "ropr-audience";
        private const string _issuerSigningKey = "asdasd123";
    }
}
