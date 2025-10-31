using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeProdutos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController:ControllerBase
{
    private readonly IMongoRepository<Product> _repo;
    public ProductsController(IMongoRepository<Product> repo) { _repo = repo; }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> Get() => await _repo.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Product p)
    {
        await _repo.CreateAsync(p);
        return CreatedAtAction(nameof(Get), new { id = p.Id }, p);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, Product p)
    {
        await _repo.UpdateAsync(id, p);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}