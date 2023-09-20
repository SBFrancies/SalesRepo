using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using System.Net;

namespace SalesRepo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PostAsync([FromBody]CreateCustomerRequest request)
    {
        var response = await _customerService.CreateCustomerAsync(request);
        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return NoContent();
    }

    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromQuery]string? search)
    {
        var response = await _customerService.GetCustomerListAsync(search);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}/orders")]
    [Produces("application/json")]
    public async Task<IActionResult> GetOrdersAsync([FromRoute]int id)
    {
        var response = await _customerService.GetCustomerOrderListAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromRoute]int id)
    {
        var response = await _customerService.GetCustomerAsync(id);
        return Ok(response);
    }

    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PutAsync([FromBody]UpdateCustomerRequest request)
    {
        var response = await _customerService.UpdateCustomerAsync(request);
        return StatusCode((int)HttpStatusCode.Created, response);
    }
}
