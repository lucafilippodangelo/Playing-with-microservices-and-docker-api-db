using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crudapi
{
    public class RabbitListener
    {
        ConnectionFactory factory { get; set; }
        IConnection connection { get; set; }
        IModel channel { get; set; }

        public void Register()
        {
            channel.QueueDeclare(queue: "LdQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                int m = 0;
                Console.WriteLine("------------------------- *** CHE CAZZO *** -----------------------");
            };
            channel.BasicConsume(queue: "LdQueue", autoAck: true, consumer: consumer);
        }

        public void Deregister()
        {
            this.connection.Close();
        }

        public RabbitListener()
        {
            this.factory = new ConnectionFactory() {
                HostName = "host.docker.internal", 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();


        }
    }
}
