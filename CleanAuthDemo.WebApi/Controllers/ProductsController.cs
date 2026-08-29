using CleanAuthDemo.Application.Authorization;
using CleanAuthDemo.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAuthDemo.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Products.Read)]
    public IActionResult Get()
    {
        return Ok(new[]
        {
            new
            {
                Id = 1,
                Name = "Product A"
            },
            new
            {
                Id = 2,
                Name = "Product B"
            }
        });
    }

    [HttpPost]
    [HasPermission(Permissions.Products.Create)]
    public IActionResult Create()
    {
        return Ok(new
        {
            Message = "Product created."
        });
    }

    [HttpPut("{id:int}")]
    [HasPermission(Permissions.Products.Update)]
    public IActionResult Update(int id)
    {
        return Ok(new
        {
            Message = $"Product {id} updated."
        });
    }

    [HttpDelete("{id:int}")]
    [HasPermission(Permissions.Products.Delete)]
    public IActionResult Delete(int id)
    {
        return Ok(new
        {
            Message = $"Product {id} deleted."
        });
    }
}