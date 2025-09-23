using RabbitMQ.Client;

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
            await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: "");
        }

        public static async Task PublishMessage(string exchangeName, string message, string routingKey = "")
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            var body = System.Text.Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: exchangeName,
                                     routingKey: routingKey,
                                     body: body);
        }
    }
}