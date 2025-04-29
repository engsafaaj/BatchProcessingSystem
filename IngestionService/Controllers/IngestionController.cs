using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper;
using System.Formats.Asn1;

namespace IngestionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngestionController : ControllerBase
    {
        private const string RabbitMqHost = "rabbitmq";
        private const string QueueName = "raw-data-queue";

        [HttpPost("upload")]
        public async Task<IActionResult> UploadRawBatch([FromBody] JsonElement rawData)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = RabbitMqHost,
                    UserName = "guest",
                    Password = "guest"
                };

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rawData));
                var props = new BasicProperties
                {
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: QueueName,
                    mandatory: false,
                    basicProperties: props,
                    body: body);

                return Ok("Raw batch uploaded and queued successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while sending to RabbitMQ: {ex.Message}");
            }
        }

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("CSV file is required.");

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = RabbitMqHost,
                    UserName = "guest",
                    Password = "guest"
                };

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<dynamic>();

                foreach (var record in records)
                {
                    var json = JsonSerializer.Serialize(record);
                    var body = Encoding.UTF8.GetBytes(json);

                    var props = new BasicProperties
                    {
                        DeliveryMode = DeliveryModes.Persistent
                    };

                    await channel.BasicPublishAsync(
                        exchange: string.Empty,
                        routingKey: QueueName,
                        mandatory: false,
                        basicProperties: props,
                        body: body
                    );
                }

                return Ok("CSV uploaded and records published to RabbitMQ.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while sending CSV to RabbitMQ: {ex.Message}");
            }
        }
    }
}
