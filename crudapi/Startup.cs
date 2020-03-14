using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
            //});
            //services.AddHostedService<RabbitMqBackgroundService>();

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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //RabbitMqConsumer.init(app);

            app.UseMvc();

            //Seed Data
            DatabaseAndDbContentSeed.SeedAsync(app)
            .Wait();

        }

    }

}
