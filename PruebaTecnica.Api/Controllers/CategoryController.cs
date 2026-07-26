using MediatR;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Application.Features.Categories.Commands.CreateCategory;
using PruebaTecnica.Application.Features.Categories.Queries.GetAllCategories;
using PruebaTecnica.Application.Features.Categories.Queries.GetCategoryById;
using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Application.Features.Categories.Commands.UpdateCategory;
using PruebaTecnica.Application.Features.Categories.Commands.DeleteCategory;
using Microsoft.AspNetCore.Authorization;

namespace PruebaTecnica.Api.Controllers;

[Authorize]
[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }
    /// <summary>
    /// Obtiene todas las categorías registradas.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Recupera la lista completa de categorías disponibles en el sistema.
    /// Si no existen categorías registradas, devuelve una lista vacía.
    /// </remarks>
    /// <returns>
    /// Una respuesta HTTP 200 con la lista de categorías registradas.
    /// </returns>
    /// <response code="200">
    /// Las categorías fueron obtenidas correctamente.
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), 200)]
    public async Task<IActionResult> GetAllCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _sender.Send(
            new GetAllCategoriesQuery(),
            cancellationToken);

        return Ok(categories);
    }
    /// <summary>
    /// Obtiene una categoría mediante su identificador.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Busca una categoría registrada en el sistema utilizando su identificador único.
    /// Si no existe una categoría con el identificador proporcionado, devuelve una
    /// respuesta HTTP 404.
    /// </remarks>
    /// <param name="id">Identificador único de la categoría.</param>
    /// <returns>
    /// Una respuesta HTTP 200 con los datos de la categoría encontrada,
    /// o una respuesta HTTP 404 cuando la categoría no existe.
    /// </returns>
    /// <response code="200">La categoría fue encontrada correctamente.</response>
    /// <response code="404">No existe una categoría con el identificador proporcionado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<IActionResult> GetCategoryById(
    int id)
    {
        var category = await _sender.Send(
            new GetCategoryByIdQuery(id));

        if (category is null)
        {
            return NotFound("Categoría no encontrada.");
        }

        return Ok(category);
    }
    /// <summary>
    /// Crea una nueva categoría.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// ✅ Registra una nueva categoría utilizando el nombre y el estado proporcionados.
    ///
    /// ✅ Cuando la categoría se crea exitosamente, devuelve una respuesta HTTP 201,
    /// junto con los datos de la categoría y la ubicación del endpoint para consultarla.
    ///
    /// ❌ Si los datos enviados no cumplen las validaciones definidas, devuelve una
    /// respuesta HTTP 400.
    /// 
    /// ❌ Si ocurre un conflicto de negocio, como intentar registrar
    /// una categoría duplicada, devuelve una respuesta HTTP 409.
    /// </remarks>
    /// <param name="addCategoryDto">
    /// Datos necesarios para crear la categoría.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 201 con la categoría creada; HTTP 400 si los datos son
    /// inválidos; o HTTP 409 si ocurre un conflicto durante la creación.
    /// </returns>
    /// <response code="201">
    /// La categoría fue creada correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen las reglas de validación.
    /// </response>
    /// <response code="409">
    /// Ya existe una categoría con los datos proporcionados o se produjo un conflicto
    /// con una regla de negocio.
    /// </response>
    [HttpPost]
    [ProducesResponseType(typeof(AddCategoryDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 409)]
    public async Task<IActionResult> AddCategory(
    AddCategoryDto addCategoryDto)
    {
        try
        {
            var category = await _sender.Send(
                new CreateCategoryCommand(
                    addCategoryDto.Nombre,
                    addCategoryDto.Estado));

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = category.Id },
                category);
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
    /// Actualiza una categoría existente.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// ✅ Modifica el nombre y el estado de una categoría utilizando su identificador.
    ///
    /// ❌ Si la categoría no existe, devuelve una respuesta HTTP 404.
    /// 
    /// ❌ Si los datos proporcionados no cumplen las reglas de validación, devuelve HTTP 400.
    /// 
    /// ❌ Si la actualización genera un conflicto de negocio, como utilizar un nombre
    /// que ya pertenece a otra categoría, devuelve HTTP 409.
    /// </remarks>
    /// <param name="id">
    /// Identificador único de la categoría.
    /// </param>
    /// <param name="updateCategoryDto">
    /// Datos que se utilizarán para actualizar la categoría.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con la categoría actualizada; HTTP 400 si los datos
    /// son inválidos; HTTP 404 si la categoría no existe; o HTTP 409 si ocurre un
    /// conflicto con una regla de negocio.
    /// </returns>
    /// <response code="200">
    /// La categoría fue actualizada correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen las reglas de validación.
    /// </response>
    /// <response code="404">
    /// No existe una categoría con el identificador proporcionado.
    /// </response>
    /// <response code="409">
    /// La actualización genera un conflicto con una regla de negocio.
    /// </response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UpdateCategoryDto), 200)]
    [ProducesResponseType(typeof(string), 404)]
    [ProducesResponseType(typeof(MessageCategoryDto), 409)]
    public async Task<IActionResult> UpdateCategory(
    int id,
    UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            var category = await _sender.Send(
                new UpdateCategoryCommand(
                    id,
                    updateCategoryDto.Nombre,
                    updateCategoryDto.Estado));

            if (category is null)
            {
                return NotFound("Categoría no encontrada.");
            }

            return Ok(category);
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
    /// Elimina una categoría existente.
    /// </summary>
    /// <remarks>
    /// 🔐 Se necesita autenticación válida para ejecutar endpoint
    /// 
    /// Elimina una categoría utilizando su identificador único.
    ///
    /// ✅La categoría solo puede eliminarse cuando no tiene productos asociados.
    /// 
    /// ❌ Si existen productos vinculados, la operación devuelve una respuesta HTTP 409.
    /// ❌ Si la categoría no existe, devuelve una respuesta HTTP 404.
    /// </remarks>
    /// <param name="id">
    /// Identificador único de la categoría.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP 200 con un mensaje de confirmación; HTTP 404 si la
    /// categoría no existe; o HTTP 409 si tiene productos asociados.
    /// </returns>
    /// <response code="200">
    /// La categoría fue eliminada correctamente.
    /// </response>
    /// <response code="404">
    /// No existe una categoría con el identificador proporcionado.
    /// </response>
    /// <response code="409">
    /// La categoría no puede eliminarse porque tiene productos asociados.
    /// </response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(MessageCategoryDto), 200)]
    [ProducesResponseType(typeof(MessageCategoryDto), 400)]
    [ProducesResponseType(typeof(MessageCategoryDto), 409)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<IActionResult> DeleteCategory(
    int id)
    {
        var result = await _sender.Send(
            new DeleteCategoryCommand(id));

        if (result.HasProducts)
        {
            return Conflict(new
            {
                message = "No se puede eliminar la categoría porque tiene productos asociados."
            });
        }

        if (!result.Success)
        {
            return NotFound("Categoría no encontrada.");
        }

        return Ok(new MessageCategoryDto
        {
            Message = "Categoría eliminada."
        });
    }
}