using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Compras;
using Pos.Application.UseCases.Compras;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/ordenes-compra")]
public sealed class OrdenesCompraController : ControllerBase
{
    private readonly OrdenCompraService _ordenCompraService;

    public OrdenesCompraController(OrdenCompraService ordenCompraService)
    {
        _ordenCompraService = ordenCompraService;
    }

    [HttpPost]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(OrdenCompraDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<OrdenCompraDto>> Create(
        [FromBody] OrdenCompraCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _ordenCompraService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(IReadOnlyList<OrdenCompraListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrdenCompraListItemDto>>> GetList(CancellationToken cancellationToken)
    {
        var result = await _ordenCompraService.GetListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(OrdenCompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrdenCompraDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var orden = await _ordenCompraService.GetByIdAsync(id, cancellationToken);
        return Ok(orden);
    }

    [HttpPost("{id:guid}/enviar")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(OrdenCompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrdenCompraDto>> Enviar(Guid id, CancellationToken cancellationToken)
    {
        var orden = await _ordenCompraService.EnviarAsync(id, cancellationToken);
        return Ok(orden);
    }

    [HttpPost("{id:guid}/cancelar")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(OrdenCompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrdenCompraDto>> Cancelar(Guid id, CancellationToken cancellationToken)
    {
        var orden = await _ordenCompraService.CancelarAsync(id, cancellationToken);
        return Ok(orden);
    }
}
