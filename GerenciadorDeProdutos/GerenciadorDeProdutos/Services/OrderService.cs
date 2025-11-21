using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Repositories;

namespace GerenciadorDeProdutos.Services;

public class OrderService
{
    private readonly IMongoRepository<Order> _orderRepo;
    private readonly IMongoRepository<Product> _productRepo;

    public OrderService(
        IMongoRepository<Order> orderRepo,
        IMongoRepository<Product> productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public async Task<Order> CreateOrderAsync(string userId, List<OrderItem> items)
    {
        if (items == null || items.Count == 0)
            throw new Exception("O pedido deve conter pelo menos um item.");

        var orderItems = new List<OrderItem>();

        foreach (var item in items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception($"Produto {item.ProductId} não encontrado.");

            if (product.Stock < item.Quantity)
                throw new Exception($"Estoque insuficiente para o produto {product.Name}. Estoque atual: {product.Stock}.");

            // Reduz o estoque
            product.Stock -= item.Quantity;
            await _productRepo.UpdateAsync(product.Id!, product);

            // Adiciona item final ao pedido com valor atual do produto
            orderItems.Add(new OrderItem
            {
                ProductId = product.Id!,
                Quantity = item.Quantity,
                Price = product.Price
            });
        }

        var order = new Order
        {
            UserId = userId,
            Items = orderItems,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepo.CreateAsync(order);
        return order;
    }

    public async Task<List<Order>> GetOrdersAsync()
        => await _orderRepo.GetAllAsync();

    public async Task<Order?> GetOrderByIdAsync(string id)
        => await _orderRepo.GetByIdAsync(id);
}