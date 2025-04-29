using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AggregationService.Data;
using AggregationService.Models;

namespace AggregationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private const string RabbitMqHost = "rabbitmq";
        private const string CleanQueueName = "clean-data-queue";
        private readonly AggregationDbContext _context;

        public AggregationController(AggregationDbContext context)
        {
            _context = context;
        }

        [HttpPost("aggregate")]
        public async Task<IActionResult> StartAggregation()
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

                await channel.QueueDeclareAsync(queue: CleanQueueName, durable: true, exclusive: false, autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var cleanData = JsonSerializer.Deserialize<JsonElement>(Encoding.UTF8.GetString(body));

                    // Read Fileds
                    var aggregatedResult = new AggregatedResult
                    {
                        PULocationID = cleanData.GetProperty("PULocationID").GetInt32(),
                        Quarter = GetQuarter(cleanData.GetProperty("tpep_pickup_datetime").GetDateTime()),
                        Year = cleanData.GetProperty("tpep_pickup_datetime").GetDateTime().Year,
                        TripCount = 1,
                        TotalFareAmount = cleanData.GetProperty("fare_amount").GetDecimal(),
                        AverageTripDistance = cleanData.GetProperty("trip_distance").GetDouble(),
                        AggregationTime = DateTime.UtcNow
                    };

                    // Save to DataBase
                    _context.AggregatedResults.Add(aggregatedResult);
                    await _context.SaveChangesAsync();

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                await channel.BasicConsumeAsync(
                    queue: CleanQueueName,
                    autoAck: false,
                    consumer: consumer
                );

                return Ok("Aggregation Service started and listening for clean batches.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while aggregating data: {ex.Message}");
            }
        }

       
        private static string GetQuarter(DateTime date)
        {
            if (date.Month >= 1 && date.Month <= 3) return "Q1";
            if (date.Month >= 4 && date.Month <= 6) return "Q2";
            if (date.Month >= 7 && date.Month <= 9) return "Q3";
            return "Q4";
        }
    }
}
