using EvaluacionTecnica.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Products.Commands.CreateProduct;
using PruebaTecnica.Application.Features.Products.Commands.UpdateProduct;
using PruebaTecnica.Application.Features.Products.DTOs;
using PruebaTecnica.Application.Features.Products.Queries.GetAllProducts;
using PruebaTecnica.Application.Features.Products.Queries.GetProductById;
using PruebaTecnica.Application.Features.Products.Commands.DeleteProduct;
using PruebaTecnica.Application.Features.Products.Commands.ChangeProductStatus;
using Microsoft.AspNetCore.Authorization;

namespace PruebaTecnica.Api.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] string? nombre,
        [FromQuery] int? categoryId,
        [FromQuery] bool? estado,
        [FromQuery] int pagina = 1,
        [FromQuery] int cantidadPorPagina = 10,
        [FromQuery] string ordenarPor = "nombre",
        [FromQuery] string direccion = "asc",
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetAllProductsQuery(
                nombre,
                categoryId,
                estado,
                pagina,
                cantidadPorPagina,
                ordenarPor,
                direccion),
            cancellationToken);

        return Ok(result);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(
    int id,
    CancellationToken cancellationToken)
    {
        var product = await _sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        if (product is null)
        {
            return NotFound("Producto no encontrado.");
        }

        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> AddProduct(
    AddProductDto addProductDto,
    CancellationToken cancellationToken)
    {
        try
        {
            var product = await _sender.Send(
                new CreateProductCommand(
                    addProductDto.Codigo,
                    addProductDto.Nombre,
                    addProductDto.CategoryId,
                    addProductDto.Precio,
                    addProductDto.Estado),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product);
        }
        catch (KeyNotFoundException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
    int id,
    UpdateProductDto updateProductDto,
    CancellationToken cancellationToken)
    {
        try
        {
            var product = await _sender.Send(
                new UpdateProductCommand(
                    id,
                    updateProductDto.Codigo,
                    updateProductDto.Nombre,
                    updateProductDto.Precio,
                    updateProductDto.Estado),
                cancellationToken);

            if (product is null)
            {
                return NotFound("Producto no encontrado.");
            }

            return Ok(product);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }
    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> ChangeProductStatus(
    int id,
    ChangeProductStatusDto changeProductStatusDto,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ChangeProductStatusCommand(
                id,
                changeProductStatusDto.Estado),
            cancellationToken);

        if (result.ProductNotFound)
        {
            return NotFound("Producto no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            return BadRequest(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(new
        {
            mensaje = result.Message,
            producto = new
            {
                result.Id,
                result.Codigo,
                result.Nombre,
                result.Precio,
                result.Estado
            }
        });
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(
    int id,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        if (result.ProductNotFound)
        {
            return NotFound("Producto no encontrado.");
        }

        if (result.HasMovements)
        {
            return Conflict(new
            {
                message = "No se puede eliminar el producto porque tiene movimientos de inventario asociados."
            });
        }

        return Ok(new
        {
            message = "Producto eliminado correctamente."
        });
    }
}