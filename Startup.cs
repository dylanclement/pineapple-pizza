﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amazon.S3;
using Amazon;
using PineapplePizza.Config;
using Amazon.DynamoDBv2;
using Amazon.Rekognition;

namespace PineapplePizza
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonRekognition>();

            services.AddTransient<S3Connector>();
            services.AddTransient<DynamoDBConnector>();
            services.AddTransient<RekognitionConnector>();
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

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
