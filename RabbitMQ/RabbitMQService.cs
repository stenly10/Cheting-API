using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cheting.RabbitMQ
{
    public class RabbitMQService
    {
        private static ConnectionFactory _factory = new ConnectionFactory { HostName = "localhost" };

        public static async Task CreateExchange(string exchangeName, string type = ExchangeType.Fanout)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: type);
        }

        public static async Task CreateQueue(string queueName, string exchangeName, string routingKey = "")
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
            await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }

        public static async Task PublishMessage(string exchangeName, string message, string routingKey = "")
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: exchangeName,
                                     routingKey: routingKey,
                                     body: body);
        }

        public static async Task<List<String>> ConsumeAllMessage(string queueName, int timeout = 5000)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var messages = new List<string>();

            while (true)
            {
                var result = await channel.BasicGetAsync(queueName, autoAck: true);
                if (result == null)
                    break;

                var message = Encoding.UTF8.GetString(result.Body.ToArray());
                messages.Add(message);
            }

            return messages;
        }
    }
}