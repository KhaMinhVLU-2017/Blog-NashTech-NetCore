using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Routing.Constraints;
using WebAPI.Helpers;
using WebAPI.Services;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.Text;
using WebAPI.Hubs;
using Entities.Models;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowAll";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<AppMeoContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:SocialBlog"]));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);// Can need

            // Config for deploy
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();// Can need
            services.AddScoped<IValidService, ValidService>();// Can need
            services.AddScoped<IAuthorService, AuthorService>();

            services.AddCors(action =>
                action.AddPolicy(MyAllowSpecificOrigins, builder =>
                 builder
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowAnyOrigin()
                 .DisallowCredentials()
                 .SetIsOriginAllowed(_ => true)// Remember the last life
                 .SetIsOriginAllowedToAllowWildcardSubdomains()
                 ));

            services.AddSignalR();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<AppSettings> appSet)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.Use(async (context, next) =>
            {
                var serviceCheck = context.RequestServices.GetService<IValidService>();
                var appsetting = appSet;
                string token = context.Request.Headers["Authorization"];
                if (token != null)
                {
                    var keySecret = appsetting.Value.Secret;

                    Console.WriteLine("KeySecert " + keySecret);
                    Console.WriteLine("Token " + token);

                    var sentence = serviceCheck.CheckToken(token, keySecret);



                    switch (sentence)
                    {
                        case "Token has expired":
                            string jsonString = JsonConvert.SerializeObject(new
                            {
                                status = 403,
                                message = "Token has expired"
                            });

                            await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                            return;
                        case "Token has invalid signature":
                            string tokeninvalid = JsonConvert.SerializeObject(new
                            {
                                status = 403,
                                message = "Token has invalid signature"
                            });

                            await context.Response.WriteAsync(tokeninvalid, Encoding.UTF8);
                            return;
                        default:
                            UserClient decodeUser = JsonConvert.DeserializeObject<UserClient>(sentence);
                            context.Items["UserID"] = decodeUser.UserID;
                            context.Items["RoleID"] = decodeUser.RoleID;
                            break;
                    }
                }
                await next.Invoke();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            string myRoot = Path.Combine(Directory.GetCurrentDirectory(), "Assert"); //Combine static root with folder Private
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(myRoot),
                RequestPath = "/Assert"
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<HubCentral>("/hubCentral");
            });


            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
