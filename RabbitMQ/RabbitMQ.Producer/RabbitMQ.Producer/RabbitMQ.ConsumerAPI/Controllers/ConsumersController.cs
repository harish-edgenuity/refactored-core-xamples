using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.ConsumerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumersController : ControllerBase
    {
         

        [HttpGet]
        public async Task<IActionResult> ConsumeProductMessage()
        {
            // Use dependency injection for connection factory
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("product", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);
            object res = new object();
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                res = Encoding.UTF8.GetString(body);

                // Do something with the message
                // Return message as API response

               
               
                
            };

            channel.BasicConsume(queue: "product", autoAck: true, consumer: consumer);

            // Wait for message to be received
           // await Task.Delay(-1); // Infinite wait (replace with appropriate timeout if needed)

            return Ok(res);
                
                
                
                
                // Return 404 if no message received within timeout
        }

    }
}
