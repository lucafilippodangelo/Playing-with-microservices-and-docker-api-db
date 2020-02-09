using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crudapi
{
    public class DatabaseAndDbContentSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            try
            {
                using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) {
                    var dataContext = serviceScope.ServiceProvider.GetService<PersonContext>();

                    //if table already exists the below attempt of migration will go wrong
                    dataContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var luca = ex.Message;
            }
        }

    }
}
