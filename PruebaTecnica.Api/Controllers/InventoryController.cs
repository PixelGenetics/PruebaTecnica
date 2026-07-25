using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Inventory.Commands.RegisterMovement;
using PruebaTecnica.Application.Features.Inventory.DTOs;
using PruebaTecnica.Application.Features.Inventory.Queries.GetProductInventory;
using PruebaTecnica.Application.Features.Inventory.Queries.GetProductKardex;
using PruebaTecnica.Application.Features.Inventory.Queries.GetStockReport;
using Microsoft.AspNetCore.Authorization;

namespace PruebaTecnica.Api.Controllers;

[Authorize]
[Route("api/inventory")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly ISender _sender;

    public InventoryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("products/{productId:int}")]
    public async Task<IActionResult> GetProductInventory(
    int productId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductInventoryQuery(productId),
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"No se encontró el producto con Id {productId}."
            });
        }

        return Ok(result);
    }

    [HttpGet("products/{productId:int}/kardex")]
    public async Task<IActionResult> GetProductKardex(
    int productId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductKardexQuery(productId),
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"No se encontró el producto con Id {productId}."
            });
        }

        return Ok(result);
    }
    [HttpGet("stock")]
    public async Task<IActionResult> GetStockReport(
    [FromQuery] int? categoryId,
    [FromQuery] int? threshold,
    CancellationToken cancellationToken)
    {
        if (categoryId.HasValue && categoryId.Value <= 0)
        {
            return BadRequest(new
            {
                message = "El identificador de categoría debe ser mayor que cero."
            });
        }

        if (threshold.HasValue && threshold.Value < 0)
        {
            return BadRequest(new
            {
                message = "El umbral no puede ser negativo."
            });
        }

        var result = await _sender.Send(
            new GetStockReportQuery(categoryId, threshold),
            cancellationToken);

        return Ok(new
        {
            categoryId,
            threshold,
            totalRegistros = result.Count,
            productos = result
        });
    }

    [HttpPost("movements")]
    public async Task<IActionResult> RegisterMovement(
        RegisterInventoryMovementDto dto,
        CancellationToken cancellationToken)
    {
        var command = new RegisterMovementCommand(
            dto.ProductId,
            dto.Tipo,
            dto.Cantidad,
            dto.Referencia);

        var result = await _sender.Send(
            command,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message,
                stockActual = result.StockActual
            });
        }

        return Created(string.Empty, new
        {
            message = result.Message,
            stockActual = result.StockActual,
            movimiento = new
            {
                id = result.Movimiento!.Id,
                productId = result.Movimiento.ProductId,
                tipo = result.Movimiento.Tipo,
                cantidad = result.Movimiento.Cantidad,
                fecha = result.Movimiento.Fecha,
                referencia = result.Movimiento.Referencia
            }
        });
    }
}