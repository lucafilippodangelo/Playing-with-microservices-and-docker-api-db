using eShopWeb.Models;
using Microsoft.EntityFrameworkCore.Metadata;
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
    public interface IRabbitMqConsumer
    {
        void init();
    }

    public class RabbitMqConsumer : IRabbitMqConsumer
    {
        private PersonRepository _personRepsitory;

        public RabbitMqConsumer(PersonRepository personRepsitory)
        { 
            init();
            _personRepsitory = personRepsitory;
        }

        public void init()
        {
            

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

                    try {
                        //LD WRITING A RECORD TO BE REFACTORED
                        _personRepsitory.AddPerson(new eShopWeb.Models.Person() { name = "NAME - " + message });
                        _personRepsitory.Save();
                    }
                    catch (Exception er) { Console.WriteLine(er.Message); }


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
