using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(" [x] ------------------------------ RECEIVER BACKGROUND PROCESS EXECUTION --------------------------- {0}");

            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal",
                Port = 5672,
                DispatchConsumersAsync = false,
                UserName = "guest",
                Password = "guest"

            };

            //try
            //{
            //    var connectionX = factory.CreateConnection();
            //}
            //catch (Exception ex)
            //{
            //    var luca = ex.Message;
            //}


            using (var connection = factory.CreateConnection())


            using (var channel = connection.CreateModel())
            {
                //Console.WriteLine(" [x] ------------------------------ RECEIVER --------------------------- {0}");
                channel.QueueDeclare(queue: "LdQueue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null
                                        );

                var consumer = new EventingBasicConsumer(channel);


                try
                {
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] ------------------------------ RECEIVER THIS IS NOT HAPPENING --------------------------- {0}", message);

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

        }
    }
}
