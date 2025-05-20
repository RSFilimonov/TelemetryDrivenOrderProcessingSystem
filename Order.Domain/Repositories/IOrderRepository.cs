using Order.Domain.Models;

namespace Order.Domain.Repositories;

public interface IOrderRepository
{
    Task<OrderModel?> GetOrderByIdAsync(Guid id);
    
    Task<IEnumerable<OrderModel>> GetAllOrdersAsync();
    
    Task AddOrderAsync(OrderModel orderModel);
    
    Task UpdateOrder(OrderModel orderModel);
    
    Task RemoveOrder(OrderModel orderModel);
}