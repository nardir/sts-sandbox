using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Axerrio.DDD.EB.RabbitMQ.ConsoleReceive
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //https://www.rabbitmq.com/dotnet-api-guide.html
                //var factory = new ConnectionFactory() { HostName = "localhost", Port = 8081, UserName = "user", Password = "password" };

                var factory = new ConnectionFactory();
                factory.Uri = new Uri(@"amqp://user:password@localhost:8081//");

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout", durable: true, autoDelete: false);

                    //var queueName = channel.QueueDeclare().QueueName;

                    var queueName = "logs_queue";
                    channel.QueueDeclare(queue: queueName,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);


                    channel.QueueBind(queue: queueName,
                                      exchange: "logs",
                                      routingKey: "");

                    Console.WriteLine(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] {0}", message);
                    };

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
