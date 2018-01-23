using RabbitMQ.Client;
using System;
using System.Text;

namespace Axerrio.DDD.EB.RabbitMQ.ConsoleSend
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost", Port = 8081};

                factory.UserName = "user";
                factory.Password = "password";



                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                        channel.ExchangeDeclare(exchange: "logs", type: "fanout", durable: true, autoDelete: false);

                        var message = GetMessage(args);
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: "logs",
                                             routingKey: "",
                                             basicProperties: properties,
                                             body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0)
                   ? string.Join(" ", args)
                   : "info: Hello World!");
        }
    }
}
