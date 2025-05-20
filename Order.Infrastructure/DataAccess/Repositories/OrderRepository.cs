using Order.Domain.Models;
using Order.Domain.Repositories;

namespace Order.Infrastructure.DataAccess.Repositories;

public interface OrderRepository : IOrderRepository
{
    async Task<OrderModel?> GetOrderByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    
    async Task<IEnumerable<OrderModel>> GetAllOrdersAsync()
    {
        throw new NotImplementedException();
    }
    
    async Task AddOrderAsync(OrderModel orderModel)
    {
        throw new NotImplementedException();
    }
    
    async Task UpdateOrder(OrderModel orderModel)
    {
        throw new NotImplementedException();
    }
    
    async Task RemoveOrder(OrderModel orderModel)
    {
        throw new NotImplementedException();
    }
}