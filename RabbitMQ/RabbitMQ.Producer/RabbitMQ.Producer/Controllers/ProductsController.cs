using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMessageProducer _messageProducer;

        public ProductsController(IMessageProducer messageProducer)
        {
            _messageProducer =messageProducer;
        }

        [HttpPost]
        public async Task<IActionResult> Send(Product product)
        {
            try
            {
                _messageProducer.SendMessage(product);
                 return Ok();
            }
            catch (Exception ex) { 
                 return BadRequest(ex.Message);
            }
        }
    }
}
