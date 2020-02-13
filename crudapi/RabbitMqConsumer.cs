using eShopWeb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crudapi
{
 

    public class RabbitMqConsumer 
    {

        public RabbitMqConsumer() {}

        public static async Task init(IApplicationBuilder applicationBuilder)
        {
            var test = new RabbitMqConsumer();

            Console.WriteLine("starting consumption");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "LdQueue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //var deserialized = JsonConvert.DeserializeObject<AddUser>(message);
                    Console.WriteLine(message);

                    try
                    {
                        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
                        {
                            var dataContext = serviceScope.ServiceProvider.GetService<PersonContext>();

                            //if table already exists the below attempt of migration will go wrong
                            dataContext.Persons.Add(new eShopWeb.Models.Person() { name = "NAME - " + message });
                            dataContext.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        var luca = ex.Message;
                    }




                };
                channel.BasicConsume(queue: "LdQueue",
                                                autoAck: true,
                                                consumer: consumer);

                Console.WriteLine("Done.");
                Console.ReadLine();
            }



        }
    }
}
