using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Service;
using System.Net;

namespace SalesRepo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;

    public CustomerController(ICustomerService customerService, IOrderService orderService)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PostAsync([FromBody] CreateCustomerRequest request)
    {
        var response = await _customerService.CreateCustomerAsync(request);
        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return NoContent();
    }

    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromQuery] string? search)
    {
        var response = await _customerService.GetCustomerListAsync(search);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}/orders")]
    [Produces("application/json")]
    public async Task<IActionResult> GetOrdersAsync([FromRoute] int id)
    {
        var response = await _customerService.GetCustomerOrderListAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
    {
        var response = await _customerService.GetCustomerAsync(id);
        return Ok(response);
    }

    [HttpPut]
    [Route("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PutAsync([FromRoute]int id, [FromBody]UpdateCustomerRequest request)
    {
        if (id != request.Id)
        {
            throw new ValidationException("Route ID does not match request body ID");
        }

        var response = await _customerService.UpdateCustomerAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("{id}/order/{productId}")]
    [Produces("application/json")]
    public async Task<IActionResult> PostOrderAsync([FromRoute] int id, [FromRoute] int productId)
    {
        var response = await _orderService.CreateOrderAsync(id, productId);
        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpPut]
    [Route("{id}/order")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PutOrderAsync([FromRoute] int id, [FromBody] UpdateOrderRequest request)
    {
        if (id != request.CustomerId)
        {
            throw new ValidationException("Route ID does not match request body ID");
        }

        var response = await _orderService.UpdateOrderAsync(request);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}/order/{productId}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetOrderAsync([FromRoute] int id, [FromRoute] int productId)
    {
        var response = await _orderService.GetOrderAsync(id, productId);
        return Ok(response);
    }


    [HttpDelete]
    [Route("{id}/order/{productId}")]
    public async Task<IActionResult> DeleteOrderAsync([FromRoute] int id, [FromRoute] int productId)
    {
        await _orderService.DeleteOrderAsync(id, productId);
        return NoContent();
    }
}
