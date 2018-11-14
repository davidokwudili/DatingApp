using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Datas;
using DatingApp.API.Datas.IDatas;
using DatingApp.API.helpers;
using DatingApp.Datas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api
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
            // DONT BE CONCERNED ABOUT THE ORDERING HERE

            //add Db Context data and get Sqlite connection string from the Configuration File
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultSqliteCon")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    //So as to ignore the exxept (Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property )
                    .AddJsonOptions(opt =>
                    {
                        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });

            // configgure the CORS so as to allow connection based on policies
            services.AddCors();

            // Set data to the cloudinary Class from the Configuration File
            //it will convert the data in the config file to tha class
            services.Configure<CoudinarySettings>(Configuration.GetSection("CoudinarySettings"));

            // add the auto mapper service
            services.AddAutoMapper();

            // so as to run the seed class 1ce
            services.AddTransient<Seed>();

            // configgure the AuthRepo as a service to be available for injection
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();

            // configgure so as to allow authentication on our API
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                                .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });

            //Aslo add the LogUserActivity as a service to function
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            // ALWAYS BE CONCERNED ABOUT THE ORDERING HERE

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // to handle all API error messages
                app.UseExceptionHandler(handler =>
                {
                    handler.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            //Our Extention Method
                            context.Response.AddApplicationError(error.Error.Message);
                            // write erro asyc
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }


            // app.UseHttpsRedirection();
            // add the seed user fucn to to pserform opt on start, comment after inserting datas once
            // seeder.SeedUsers();

            //add allow policy, for the api to be accessible
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //so as to use auth where it's availavable on our API controler
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
