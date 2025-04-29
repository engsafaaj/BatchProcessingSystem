using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PreprocessingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreprocessingController : ControllerBase
    {
        private const string RabbitMqHost = "rabbitmq";
        private const string RawQueueName = "raw-data-queue";
        private const string CleanQueueName = "clean-data-queue";

        [HttpPost("start")]
        public async Task<IActionResult> StartPreprocessing()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = RabbitMqHost,
                    UserName = "guest",
                    Password = "guest"
                };

                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: RawQueueName, durable: true, exclusive: false, autoDelete: false);
                await channel.QueueDeclareAsync(queue: CleanQueueName, durable: true, exclusive: false, autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var rawData = JsonSerializer.Deserialize<JsonElement>(Encoding.UTF8.GetString(body));

                    
                    var cleanedData = rawData; 

                    var cleanedBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cleanedData));

                    var props = new BasicProperties
                    {
                        DeliveryMode = DeliveryModes.Persistent
                    };

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: CleanQueueName,
                        mandatory: false,
                        basicProperties: props,
                        body: cleanedBody
                    );

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                await channel.BasicConsumeAsync(
                    queue: RawQueueName,
                    autoAck: false,
                    consumer: consumer
                );

                return Ok("Preprocessing Service started and listening for raw batches.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while preprocessing data: {ex.Message}");
            }
        }
    }
}
