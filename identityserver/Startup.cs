// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        private const string corsPolicy = "_allowSpecificOrigins";

        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            //services.AddControllersWithViews();
            services.AddCors(options => {
                options.AddPolicy(name: corsPolicy,
                    builder => {
                        builder.WithOrigins("http://localhost:8082");
                    });
            });
            var builder = services.AddIdentityServer(x => {
                        x.IssuerUri = "http://identity";
                    })
                .AddDeveloperSigningCredential()
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(corsPolicy);
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }        
            app.UseIdentityServer();
        }
    }
}
