using AutoMapper;
using SecretSanta.Business.Services;
using SecretSanta.Business;
using SecretSanta.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Rewrite;

namespace SecretSanta.Api
{
    // Justification: Disable until ConfigureServices is added back.
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class Startup
    #pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void ConfigureServices(IServiceCollection services)
        {
            var sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
               options.EnableSensitiveDataLogging()
                      .UseSqlite(sqliteConnection));

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGiftService, GiftService>();
            services.AddScoped<IUserService, UserService>();

            System.Type profileType = typeof(AutomapperConfigurationProfile);
            System.Reflection.Assembly assembly = profileType.Assembly;
            services.AddAutoMapper(new[] { assembly });

            services.AddMvc(opts => opts.EnableEndpointRouting = false);

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "vMoney";
                    document.Info.Title = "Krusty Krab";
                    document.Info.Description = "Assignment 4";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Mr.Krabs",
                        Email = "10ReasonsToSaveADime@gmail.com",
                        Url = "https://spongebob.fandom.com/wiki/Eugene_H._Krabs"
                    };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            var option = new RewriteOptions();//code to redirect root to swagger taken from https://stackoverflow.com/questions/49290683/how-to-redirect-root-to-swagger-in-asp-net-core-2-x
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseMvc();
        }
    }
}
