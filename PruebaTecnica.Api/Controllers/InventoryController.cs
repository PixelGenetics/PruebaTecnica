using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Application.Features.Inventory.Commands.RegisterMovement;
using PruebaTecnica.Application.Features.Inventory.DTOs;
using PruebaTecnica.Application.Features.Inventory.Queries.GetProductInventory;
using PruebaTecnica.Application.Features.Inventory.Queries.GetProductKardex;
using PruebaTecnica.Application.Features.Inventory.Queries.GetStockReport;

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
    /// <summary>
    /// Obtiene el inventario actual de un producto.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Consulta la información de inventario asociada a un producto mediante su
    /// identificador único.
    ///
    /// ❌ Si el producto no existe, devuelve una respuesta HTTP 404.
    /// </remarks>
    /// <param name="productId">
    /// Identificador único del producto.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con la información de inventario del producto;
    /// HTTP 400 si el identificador proporcionado no es válido; o HTTP 404
    /// si el producto no existe.
    /// </returns>
    /// <response code="200">
    /// La información de inventario fue obtenida correctamente.
    /// </response>
    /// <response code="400">
    /// El identificador proporcionado no es válido.
    /// </response>
    /// <response code="404">
    /// No existe un producto con el identificador proporcionado.
    /// </response>
    [HttpGet("products/{productId:int}")]
    [ProducesResponseType(typeof(ProductInventoryDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 404)]
    public async Task<IActionResult> GetProductInventory(
    int productId)
    {
        var result = await _sender.Send(
            new GetProductInventoryQuery(productId));

        if (result is null)
        {
            return NotFound(new
            {
                message = $"No se encontró el producto con Id {productId}."
            });
        }

        return Ok(result);
    }
    /// <summary>
    /// Obtiene el historial de movimientos de inventario de un producto.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Consulta el kardex de un producto mediante su identificador único.
    ///
    /// ✅ La respuesta incluye los movimientos de entrada y salida registrados,
    /// el saldo acumulado después de cada movimiento y el resumen del inventario,
    /// como el total de entradas, el total de salidas y el stock final.
    ///
    /// ❌ Si el producto no existe, devuelve una respuesta HTTP 404.
    /// </remarks>
    /// <param name="productId">
    /// Identificador único del producto para consultar kardex.
    /// </param>
    /// <returns>
    /// ✅ Una respuesta HTTP 200 con el historial y resumen de inventario del producto.
    /// 
    /// ❌ HTTP 400 si el identificador proporcionado no es válido;
    /// 
    /// ❌ HTTP 404 si el producto no existe.
    /// </returns>
    /// <response code="200">
    /// El kardex del producto fue obtenido correctamente.
    /// </response>
    /// <response code="400">
    /// El identificador proporcionado no es válido.
    /// </response>
    /// <response code="404">
    /// No existe un producto con el identificador proporcionado.
    /// </response>
    [HttpGet("products/{productId:int}/kardex")]
    [ProducesResponseType(typeof(ProductKardexDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 404)]
    public async Task<IActionResult> GetProductKardex(
    int productId)
    {
        var result = await _sender.Send(
            new GetProductKardexQuery(productId));

        if (result is null)
        {
            return NotFound(new
            {
                message = $"No se encontró el producto con Id {productId}."
            });
        }

        return Ok(result);
    }
    /// <summary>
    /// Obtiene un reporte del stock actual de los productos.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Consulta el stock disponible de todos los productos registrados.
    ///
    /// ✅ La consulta puede filtrarse opcionalmente por categoría mediante
    /// <c>categoryId</c>. También puede utilizarse el parámetro <c>threshold</c>
    /// para mostrar únicamente los productos cuyo stock sea menor o igual
    /// al umbral proporcionado.
    ///
    /// ❌ Si no se envían filtros, devuelve el stock actual de todos los productos.
    ///
    /// El identificador de categoría debe ser mayor que cero y el umbral
    /// no puede ser un número negativo.
    /// </remarks>
    /// <param name="categoryId">
    /// Identificador de la categoría.
    /// Debe ser mayor que cero.
    /// </param>
    /// <param name="threshold">
    /// Umbral opcional de stock. Cuando se proporciona, devuelve únicamente
    /// los productos cuyo stock sea menor o igual al valor indicado.
    /// No puede ser negativo.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con los filtros aplicados, el total de registros
    /// encontrados y la lista de productos; o HTTP 400 si alguno de los
    /// parámetros proporcionados no es válido.
    /// </returns>
    /// <response code="200">
    /// El reporte de stock fue obtenido correctamente.
    /// </response>
    /// <response code="400">
    /// El identificador de categoría o el umbral proporcionado no es válido.
    /// </response>
    [HttpGet("stock")]
    [ProducesResponseType(typeof(StockReportResponseDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 500)]
    public async Task<IActionResult> GetStockReport(
    [FromQuery] int? categoryId,
    [FromQuery] int? threshold)
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
            new GetStockReportQuery(categoryId, threshold));

        return Ok(new StockReportResponseDto
        {
            CategoryId = categoryId,
            Threshold = threshold,
            TotalRegistros  = result.Count,
            Productos = result
        });
    }

    /// <summary>
    /// Registra un movimiento de inventario.
    /// </summary>
    /// <remarks>
    /// Registra una entrada o salida de inventario para un producto.
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    ///
    /// ✅ Para los movimientos de salida, la cantidad solicitada no puede superar
    /// el stock disponible del producto. La operación no permite que el stock
    /// final quede en un valor negativo.
    ///
    /// ✅ Cuando el movimiento se registra correctamente, devuelve el stock actualizado
    /// y los datos del movimiento creado.
    /// </remarks>
    /// <param name="dto">
    /// Datos necesarios para registrar el movimiento, incluyendo el producto,
    /// el tipo de movimiento, la cantidad y una referencia opcional.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 201 con el movimiento registrado y el stock actualizado;
    /// o HTTP 400 cuando los datos son inválidos o el movimiento no puede realizarse.
    /// </returns>
    /// <response code="201">
    /// El movimiento de inventario fue registrado correctamente.
    /// </response>
    /// <response code="400">
    /// El movimiento no pudo registrarse porque los datos son inválidos,
    /// el producto no existe o la salida dejaría el stock en negativo.
    /// </response>
    [HttpPost("movements")]
    [ProducesResponseType(typeof(RegisterMovementResponseDto),201)]
    [ProducesResponseType(typeof(RegisterMovementErrorDto),400)]
    public async Task<IActionResult> RegisterMovement(
        RegisterInventoryMovementDto dto)
    {
        var command = new RegisterMovementCommand(
            dto.ProductId,
            dto.Tipo,
            dto.Cantidad,
            dto.Referencia);

        var result = await _sender.Send(
            command);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message,
                stockActual = result.StockActual
            });
        }

        return Created(string.Empty, new RegisterMovementResponseDto
        {
            Message = result.Message,
            StockActual = result.StockActual,
            Movimiento = new InventoryMovementDto
            {
                Id = result.Movimiento!.Id,
                ProductId = result.Movimiento.ProductId,
                Tipo = result.Movimiento.Tipo,
                Cantidad = result.Movimiento.Cantidad,
                Fecha = result.Movimiento.Fecha,
                Referencia = result.Movimiento.Referencia
            }
        });
    }
}