using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GerenciadorDeProdutos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // exige token JWT em todas as ações
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // GET api/orders
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        var list = await _orderService.GetOrdersAsync();
        return Ok(list);
    }

    // GET api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    // POST api/orders
    [HttpPost]
    public async Task<ActionResult> Create(List<OrderItem> items)
    {
        // Extrai ID do usuário do token JWT
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        if (userId == null)
            return Unauthorized("Usuário não identificado.");

        try
        {
            var order = await _orderService.CreateOrderAsync(userId, items);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}