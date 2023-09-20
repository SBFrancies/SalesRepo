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

    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _orderService.GetOrderListAsync();
        return Ok(response);
    }
}
