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

    [HttpGet]
    public async Task<IActionResult> GetAllCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _sender.Send(
            new GetAllCategoriesQuery(),
            cancellationToken);

        return Ok(categories);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(
    int id,
    CancellationToken cancellationToken)
    {
        var category = await _sender.Send(
            new GetCategoryByIdQuery(id),
            cancellationToken);

        if (category is null)
        {
            return NotFound("Categoría no encontrada.");
        }

        return Ok(category);
    }
    [HttpPost]
    public async Task<IActionResult> AddCategory(
    AddCategoryDto addCategoryDto,
    CancellationToken cancellationToken)
    {
        try
        {
            var category = await _sender.Send(
                new CreateCategoryCommand(
                    addCategoryDto.Nombre,
                    addCategoryDto.Estado),
                cancellationToken);

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
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
    int id,
    UpdateCategoryDto updateCategoryDto,
    CancellationToken cancellationToken)
    {
        try
        {
            var category = await _sender.Send(
                new UpdateCategoryCommand(
                    id,
                    updateCategoryDto.Nombre,
                    updateCategoryDto.Estado),
                cancellationToken);

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
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(
    int id,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new DeleteCategoryCommand(id),
            cancellationToken);

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

        return Ok(new
        {
            message = "Categoría eliminada."
        });
    }
}