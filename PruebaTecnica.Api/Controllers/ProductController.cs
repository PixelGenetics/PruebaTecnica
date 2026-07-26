using EvaluacionTecnica.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Application.Features.Products.Commands.ChangeProductStatus;
using PruebaTecnica.Application.Features.Products.Commands.CreateProduct;
using PruebaTecnica.Application.Features.Products.Commands.DeleteProduct;
using PruebaTecnica.Application.Features.Products.Commands.UpdateProduct;
using PruebaTecnica.Application.Features.Products.DTOs;
using PruebaTecnica.Application.Features.Products.Queries.GetAllProducts;
using PruebaTecnica.Application.Features.Products.Queries.GetProductById;

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
    /// <summary>
    /// Obtiene un listado paginado de productos.
    /// </summary>
    /// <remarks>
    /// Consulta los productos registrados en el sistema y permite aplicar filtros
    /// opcionales por nombre, categoría y estado.
    /// 
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    ///
    /// También permite controlar la paginación y ordenar los resultados por una
    /// propiedad determinada en dirección ascendente o descendente.
    ///
    /// ❌ Si no se proporcionan parámetros, devuelve la primera página con 10 productos,
    /// ordenados por nombre de forma ascendente.
    /// </remarks>
    /// <param name="nombre">
    /// Busca productos por nombre.
    /// </param>
    /// <param name="categoryId">
    /// Identificador de categoría por la que se desea filtrar.
    /// </param>
    /// <param name="estado">
    /// Estado del producto. Usar <c>true</c> para productos activos
    /// y <c>false</c> para productos inactivos.
    /// </param>
    /// <param name="pagina">
    /// Número de página que se desea consultar. El valor predeterminado es 1.
    /// </param>
    /// <param name="cantidadPorPagina">
    /// Cantidad de productos que se devolverán por página.
    /// El valor predeterminado es 10.
    /// </param>
    /// <param name="ordenarPor">
    /// Propiedad por la que se ordenarán los resultados.
    /// El valor predeterminado es <c>nombre</c>.
    /// </param>
    /// <param name="direccion">
    /// Dirección del ordenamiento. Los valores admitidos son <c>asc</c>
    /// y <c>desc</c>. El valor predeterminado es <c>asc</c>.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con los productos encontrados, la información
    /// de paginación y el total de registros; o HTTP 400 cuando alguno de los
    /// parámetros proporcionados no es válido.
    /// </returns>
    /// <response code="200">
    /// El listado de productos fue obtenido correctamente.
    /// </response>
    /// <response code="400">
    /// Alguno de los filtros, valores de paginación o parámetros de ordenamiento
    /// proporcionados no es válido.
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedProductsDto), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] string? nombre,
        [FromQuery] int? categoryId,
        [FromQuery] bool? estado,
        [FromQuery] int pagina = 1,
        [FromQuery] int cantidadPorPagina = 10,
        [FromQuery] string ordenarPor = "nombre",
        [FromQuery] string direccion = "asc")
    {
        var result = await _sender.Send(
            new GetAllProductsQuery(
                nombre,
                categoryId,
                estado,
                pagina,
                cantidadPorPagina,
                ordenarPor,
                direccion));

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un producto mediante su identificador.
    /// </summary>
    /// <remarks>
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// 
    /// ✅ Busca un producto registrado en el sistema utilizando su identificador único.
    ///
    /// ❌ Si no existe un producto con el identificador proporcionado, devuelve una
    /// respuesta HTTP 404.
    /// </remarks>
    /// <param name="id">
    /// Identificador único del producto que se desea consultar.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con los datos del producto encontrado;
    /// o HTTP 404 si el producto no existe.
    /// </returns>
    /// <response code="200">
    /// El producto fue encontrado correctamente.
    /// </response>
    /// <response code="404">
    /// No existe un producto con el identificador proporcionado.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 404)]
    public async Task<IActionResult> GetProductById(
    int id)
    {
        var product = await _sender.Send(
            new GetProductByIdQuery(id));

        if (product is null)
        {
            return NotFound("Producto no encontrado.");
        }

        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    /// <remarks>
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// ✅ Registra un producto utilizando el código, nombre, categoría, precio y estado
    /// proporcionados.
    ///
    /// ❌ Si la categoría indicada no existe, devuelve una respuesta HTTP 400.
    /// ❌ Si el código o el nombre del producto generan un conflicto con una regla
    /// de negocio, devuelve una respuesta HTTP 409.
    /// </remarks>
    /// <param name="addProductDto">
    /// Datos necesarios para crear el producto.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 201 con el producto creado; HTTP 400 si la categoría no
    /// existe o los datos enviados son inválidos; o HTTP 409 si ocurre un conflicto
    /// con una regla de negocio.
    /// </returns>
    /// <response code="201">
    /// El producto fue creado correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados son inválidos o la categoría proporcionada no existe.
    /// </response>
    /// <response code="409">
    /// Ya existe un producto con el código o nombre proporcionado, o se produjo
    /// otro conflicto con una regla de negocio.
    /// </response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto),201)]
    [ProducesResponseType(typeof(MessageCategoryDto),400)]
    [ProducesResponseType(typeof(MessageCategoryDto),404)]
    public async Task<IActionResult> AddProduct(
    AddProductDto addProductDto)
    {
        try
        {
            var product = await _sender.Send(
                new CreateProductCommand(
                    addProductDto.Codigo,
                    addProductDto.Nombre,
                    addProductDto.CategoryId,
                    addProductDto.Precio,
                    addProductDto.Estado));

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

    /// <summary>
    /// Actualiza parcialmente un producto existente.
    /// </summary>
    /// <remarks>
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// 
    /// ✅ Modifica los datos editables de un producto utilizando su identificador único.
    ///
    /// El endpoint permite actualizar el código, nombre, precio y estado del producto.
    /// Si el producto no existe, devuelve una respuesta HTTP 404.
    ///
    /// ❌ Si los datos enviados no cumplen las reglas de validación, devuelve HTTP 400.
    /// ❌ Si la actualización genera un conflicto de negocio, como utilizar un código
    /// o nombre que ya pertenece a otro producto, devuelve HTTP 409.
    /// </remarks>
    /// <param name="id">
    /// Identificador único del producto.
    /// </param>
    /// <param name="updateProductDto">
    /// Datos que se utilizarán para actualizar parcialmente el producto.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con el producto actualizado; HTTP 400 si los datos
    /// enviados son inválidos; HTTP 404 si el producto no existe; o HTTP 409 si
    /// ocurre un conflicto con una regla de negocio.
    /// </returns>
    /// <response code="200">
    /// El producto fue actualizado correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen las reglas de validación.
    /// </response>
    /// <response code="404">
    /// No existe un producto con el identificador proporcionado.
    /// </response>
    /// <response code="409">
    /// La actualización genera un conflicto con una regla de negocio.
    /// </response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 404)]
    [ProducesResponseType(typeof(MessageCategoryDto),409)]
    public async Task<IActionResult> UpdateProduct(
    int id,
    UpdateProductDto updateProductDto)
    {
        try
        {
            var product = await _sender.Send(
                new UpdateProductCommand(
                    id,
                    updateProductDto.Codigo,
                    updateProductDto.Nombre,
                    updateProductDto.Precio,
                    updateProductDto.Estado));

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

    /// <summary>
    /// Cambia el estado de un producto.
    /// </summary>
    /// <remarks>
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Activa o desactiva un producto mediante su identificador único.
    ///
    /// ✅ La operación aplica las reglas de negocio definidas para el cambio de estado.
    /// 
    /// ❌ Si el producto no existe, devuelve una respuesta HTTP 404.
    ///
    /// ❌ Si el cambio solicitado no puede realizarse por una regla de negocio,
    /// devuelve una respuesta HTTP 400 con el motivo correspondiente.
    /// </remarks>
    /// <param name="id">
    /// Identificador único del producto.
    /// </param>
    /// <param name="changeProductStatusDto">
    /// Nuevo estado que se asignará al producto.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con el mensaje de confirmación y los datos actualizados
    /// del producto; HTTP 400 si el cambio de estado no puede realizarse; o HTTP 404
    /// si el producto no existe.
    /// </returns>
    /// <response code="200">
    /// El estado del producto fue actualizado correctamente.
    /// </response>
    /// <response code="400">
    /// El cambio de estado no puede realizarse debido a una regla de negocio
    /// o porque los datos enviados no son válidos.
    /// </response>
    /// <response code="404">
    /// No existe un producto con el identificador proporcionado.
    /// </response>
    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(ChangeProductStatusResponseDto),200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 404)]
    public async Task<IActionResult> ChangeProductStatus(
    int id,
    ChangeProductStatusDto changeProductStatusDto)
    {
        var result = await _sender.Send(
            new ChangeProductStatusCommand(
                id,
                changeProductStatusDto.Estado));

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

        return Ok(new ChangeProductStatusResponseDto
        {
            Mensaje = result.Message,
            Producto = new ProductStatusDto
            {
                Id = result.Id,
                Codigo = result.Codigo,
                Nombre = result.Nombre,
                Precio = result.Precio,
                Estado = result.Estado
            }
        });
    }

    /// <summary>
    /// Elimina un producto existente.
    /// </summary>
    /// <remarks>
    /// 
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Elimina un producto utilizando su identificador único.
    ///
    /// ✅ El producto solo puede eliminarse cuando no tiene movimientos de inventario
    /// asociados.
    /// 
    /// ❌ Si existen movimientos vinculados, la operación devuelve una
    /// respuesta HTTP 409.
    ///
    /// ❌ Si el producto no existe, devuelve una respuesta HTTP 404.
    /// </remarks>
    /// <param name="id">
    /// Identificador único del producto a eliminar.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con un mensaje de confirmación; HTTP 404 si el
    /// producto no existe. HTTP 409 si tiene movimientos de inventario asociados.
    /// </returns>
    /// <response code="200">
    /// El producto fue eliminado correctamente.
    /// </response>
    /// <response code="404">
    /// No existe un producto con el identificador proporcionado.
    /// </response>
    /// <response code="409">
    /// El producto no puede eliminarse porque tiene movimientos de inventario asociados.
    /// </response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(MessageCategoryDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 404)]
    [ProducesResponseType(typeof(MessageCategoryDto), 409)]
    public async Task<IActionResult> DeleteProduct(
    int id)
    {
        var result = await _sender.Send(
            new DeleteProductCommand(id) );

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

        return Ok(new MessageCategoryDto
        {
            Message = "Producto eliminado correctamente."
        });
    }
}