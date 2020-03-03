using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace crudapitwo
{
    public class RabbitMqBackgroundService : BackgroundService
    {


        public RabbitMqBackgroundService()
        {

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(" [x] ------------------------------ BACKGROUND PROCESS EXECUTION --------------------------- {0}");

            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal", //IT'S important to match the connection IP I see in the rabbitMQ web page -> http://localhost:15672/#/connections
                Port = 5672,
                UserName = "guest",
                Password = "guest"
                 
            };

            try
            {
                var connectionX = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                var luca = ex.Message;
            }

            Console.WriteLine(" [x] ------------------------------ !! IN 5 !! --------------------------- {0}");

            using (var connection = factory.CreateConnection())


            using (var channel = connection.CreateModel())
            {
                Console.WriteLine(" [x] ------------------------------ !! IN !! --------------------------- {0}");
                channel.QueueDeclare(queue: "LdQueue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null
                                        );

                var consumer = new EventingBasicConsumer(channel);
               

                try {
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] ------------------------------ THIS IS NOT HAPPENING --------------------------- {0}", message);

                        //try
                        //{
                        //    using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
                        //    {
                        //        var dataContext = serviceScope.ServiceProvider.GetService<PersonContext>();

                        //        dataContext.Persons.Add(new eShopWeb.Models.Person() { name = "NAME - " + message });
                        //        dataContext.SaveChanges();
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    var luca = ex.Message;
                        //}

                    };
                }
                catch (Exception ex)
                {
                    var luca = ex.Message;
                }


                channel.BasicConsume(queue: "LdQueue",
                                                autoAck: true,
                                                consumer: consumer);
            }

            Console.WriteLine(" [x] ------------------------------ OUT --------------------------- {0}");
        }
    }
}
