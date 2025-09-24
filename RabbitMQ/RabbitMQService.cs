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
            await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: "");
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

        public static async Task<string> ConsumeMessage(string queueName, int timeout = 5000)
        {
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            var tcs = new TaskCompletionSource<string>();
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(message);

                await channel.CloseAsync();
                await connection.CloseAsync();
            };

            await channel.BasicConsumeAsync(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
            if (completed == tcs.Task)
                return await tcs.Task;
            else
                return "";
        }
    }
}