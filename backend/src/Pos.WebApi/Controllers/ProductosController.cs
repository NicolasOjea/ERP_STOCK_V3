using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Stock;
using Pos.Application.UseCases.Products;
using Pos.Application.UseCases.Stock;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/productos")]
public sealed class ProductosController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly StockService _stockService;

    public ProductosController(ProductService productService, StockService stockService)
    {
        _productService = productService;
        _stockService = stockService;
    }

    [HttpGet]
    [Authorize(Policy = "PERM_PRODUCTO_VER")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] Guid? categoriaId,
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        var results = await _productService.SearchAsync(search, categoriaId, activo, cancellationToken);
        return Ok(results);
    }

    [HttpPost]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductDetailDto>> Create(
        [FromBody] ProductCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_VER")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDetailDto>> Update(
        Guid id,
        [FromBody] ProductUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _productService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/codigos")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductCodeDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductCodeDto>> AddCode(
        Guid id,
        [FromBody] ProductCodeCreateDto request,
        CancellationToken cancellationToken)
    {
        var code = await _productService.AddCodeAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, code);
    }

    [HttpDelete("{id:guid}/codigos/{codigoId:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveCode(Guid id, Guid codigoId, CancellationToken cancellationToken)
    {
        await _productService.RemoveCodeAsync(id, codigoId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/proveedores")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductProveedorDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductProveedorDto>> AddProveedor(
        Guid id,
        [FromBody] ProductProveedorCreateDto request,
        CancellationToken cancellationToken)
    {
        var relation = await _productService.AddProveedorAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, relation);
    }

    [HttpPatch("{id:guid}/proveedores/{relId:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductProveedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductProveedorDto>> SetProveedorPrincipal(
        Guid id,
        Guid relId,
        [FromBody] ProductProveedorUpdateDto request,
        CancellationToken cancellationToken)
    {
        var relation = await _productService.SetProveedorPrincipalAsync(id, relId, request, cancellationToken);
        return Ok(relation);
    }

    [HttpPatch("{id:guid}/stock-config")]
    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [ProducesResponseType(typeof(StockConfigDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StockConfigDto>> UpdateStockConfig(
        Guid id,
        [FromBody] StockConfigUpdateDto request,
        CancellationToken cancellationToken)
    {
        var config = await _stockService.UpdateStockConfigAsync(id, request, cancellationToken);
        return Ok(config);
    }

    [HttpGet("{id:guid}/stock-config")]
    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [ProducesResponseType(typeof(StockConfigDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StockConfigDto>> GetStockConfig(Guid id, CancellationToken cancellationToken)
    {
        var config = await _stockService.GetStockConfigAsync(id, cancellationToken);
        return Ok(config);
    }
}
