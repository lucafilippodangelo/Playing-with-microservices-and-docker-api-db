using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace crudapi
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

            services.AddDbContext<PersonContext>(c =>
            {
                try
                {
                    var connString = Configuration["ConnectionString"];
                    c.UseSqlServer(connString);
                    c.ConfigureWarnings(wb =>
                    {
                        //By default, in this application, we don't want to have client evaluations
                        wb.Log(RelationalEventId.QueryClientEvaluationWarning);
                    });
                }
                catch (System.Exception ex)
                {
                    var message = ex.Message;
                }
            });

            services.AddScoped<IPersonRepository, PersonRepository>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //Seed Data
            DatabaseAndDbContentSeed.SeedAsync(app)
            .Wait();

        }
    }
}
