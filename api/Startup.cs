using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace api
{
    public class Startup
    {
        private const string corsPolicy = "_allowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

                    //-------   IDENTITY SERVER (OLD) VERSION   -------//
                    //            services.AddAuthentication("Bearer")
                    //                .AddJwtBearer("Bearer", options =>
                    //                {
                    //                    options.Authority = "http://identity";
                    //                    options.RequireHttpsMetadata = false;
                    //                    options.TokenValidationParameters = new TokenValidationParameters
                    //                    {
                    //                        ValidateAudience = false
                    //                    };
                    //                });

        //----- NEW VERSION AUTH0: -----//
            services.AddMvc();

            // Auth server is being added:
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://dev-fwkbpfvs14v5kh8o.us.auth0.com/";
                options.Audience = "http://localhost:7356";
            });

            // Allowed scope is being defined:
            services.AddAuthorization(options => {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "read:krc-genk");
                });
            });

            // Cors policy to allow access from web-layer:8082:
            services.AddControllers();
            services.AddCors(options => {
                options.AddPolicy(name: corsPolicy,
                    builder => {
                        builder.WithOrigins("http://localhost:8082")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(corsPolicy);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(corsPolicy);
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization("ApiScope");
            });

        }
    }
}
