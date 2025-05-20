using System.ComponentModel.DataAnnotations;

namespace Order.Domain.Models;

public class OrderItemModel
{
    public Guid Id { get; init; }
    
    public Guid ProductId { get; set; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal TotalPrice => UnitPrice * Quantity;
    
    public Guid OrderId { get; init; }
    public OrderModel Order { get; init; } = null!;

    private OrderItemModel(Guid id, Guid productId, int quantity, decimal unitPrice, Guid orderId)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        OrderId = orderId;
    }

    public static OrderItemModel CreateOrder(Guid productId, string productName, int quantity, decimal unitPrice, Guid orderId)
    {
        if (productId == Guid.Empty)
            throw new ValidationException("Product ID is required.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new ValidationException("Product name is required.");

        if (quantity <= 0)
            throw new ValidationException("Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ValidationException("Unit price must be greater than zero.");
        
        if (orderId == Guid.Empty)
            throw new ValidationException("Product ID is required.");
        
        return new OrderItemModel(Guid.NewGuid(), productId, quantity, unitPrice, orderId);
    }
}
