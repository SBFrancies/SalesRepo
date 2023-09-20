using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using System.Net;

namespace SalesRepo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PostAsync([FromBody] CreateProductRequest request)
    {
        var response = await _productService.CreateProductAsync(request);
        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }

    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromQuery] string? search)
    {
        var response = await _productService.GetProductListAsync(search);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}/orders")]
    [Produces("application/json")]
    public async Task<IActionResult> GetOrdersAsync([FromRoute] int id)
    {
        var response = await _productService.GetProductOrderListAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
    {
        var response = await _productService.GetProductAsync(id);
        return Ok(response);
    }

    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PutAsync([FromBody] UpdateProductRequest request)
    {
        var response = await _productService.UpdateProductAsync(request);
        return StatusCode((int)HttpStatusCode.Created, response);
    }
}
