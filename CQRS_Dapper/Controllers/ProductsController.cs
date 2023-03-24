using CQRS_Dapper.Commands;
using CQRS_Dapper.Commands.Products;
using CQRS_Dapper.Model;
using CQRS_Dapper.Notifications;
using CQRS_Dapper.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRS_Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("GetProducts")]
        public async Task<ActionResult> GetProductsAsync()
        {
            var products = await _mediator.Send(new GetProductsQuery()).ConfigureAwait(true);
            return Ok(products);
        }

        [HttpPost("AddProduct")]
        public async Task<ActionResult> AddProductAsync([FromBody] Product product)
        {
            var productToReturn = await _mediator.Send(new AddProductCommand(product)).ConfigureAwait(true);
            await _mediator.Publish(new ProductAddedNotification(productToReturn)).ConfigureAwait(true);
            return CreatedAtRoute("GetProductById", new { id = productToReturn.Id }, productToReturn);
           
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public async Task<ActionResult> GetProductByIdAsync(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id)).ConfigureAwait(true);

            return Ok(product);
        }
    }
}
