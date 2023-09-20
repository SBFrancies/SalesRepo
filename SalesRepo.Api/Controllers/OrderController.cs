using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using System.Net;

namespace SalesRepo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpPost]
    [Route("{customerId}/{productId}")]
    [Produces("application/json")]
    public async Task<IActionResult> PostAsync([FromRoute] int customerId, [FromRoute] int productId)
    {
        var response = await _orderService.CreateOrderAsync(customerId, productId);
        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PutAsync([FromBody] UpdateOrderRequest request)
    {
        var response = await _orderService.UpdateOrderAsync(request);
        return Ok(response);
    }

    [HttpGet]
    [Route("{customerId}/{productId}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromRoute] int customerId, [FromRoute] int productId)
    {
        var response = await _orderService.GetOrderAsync(customerId, productId);
        return Ok(response);
    }

    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _orderService.GetOrderListAsync();
        return Ok(response);
    }

    [HttpDelete]
    [Route("{customerId}/{productId}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int customerId, [FromRoute] int productId)
    {
        await _orderService.DeleteOrderAsync(customerId, productId);
        return NoContent();
    }
}
