using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Infrastructure.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interfaces;
using Shared.Common.Constants;
using Shared.DTOs.Product;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("Bearer")]
public class ProductsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    #region Additional Resources

    [HttpGet("get-product-by-no/{productNo}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProductByNo([Required] string productNo)
    {
        var product = await _repository.GetProductByNoAsync(productNo);
        if (product == null) return NotFound();

        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    #endregion

    #region CRUD

    [HttpGet]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _repository.GetProductsAsync();
        var result = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProduct([Required] long id)
    {
        var product = await _repository.GetProductAsync(id);
        if (product == null) return NotFound();

        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.CREATE)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        var productEntity = await _repository.GetProductByNoAsync(productDto.No);
        if (productEntity != null) return BadRequest($"Product No: {productDto.No} is existed.");

        var product = _mapper.Map<CatalogProduct>(productDto);
        await _repository.CreateProductAsync(product);
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.UPDATE)]
    public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
    {
        var product = await _repository.GetProductAsync(id);
        if (product == null) return NotFound();

        var updateProduct = _mapper.Map(productDto, product);
        await _repository.UpdateProductAsync(updateProduct);
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteProduct([Required] long id)
    {
        var product = await _repository.GetProductAsync(id);
        if (product == null) return NotFound();

        await _repository.DeleteProductAsync(id);
        return NoContent();
    }

    #endregion
}