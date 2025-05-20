using System.ComponentModel.DataAnnotations;
using Order.Domain.Enums;
using Order.Domain.Exceptions;

namespace Order.Domain.Models;

public class OrderModel
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public DateTime CreatedAt { get; init; }

    private OrderStatus Status { get; set; }

    public decimal TotalAmount { get; init; }

    public Guid ShippingAddressId { get; set; }

    public AddressModel ShippingAddress { get; init; } = null!;
    public ICollection<OrderItemModel> Items { get; init; } = null!;

    private OrderModel() { }
    private OrderModel(Guid id, Guid customerId, DateTime createdAt, OrderStatus status, Guid shippingAddressId)
    {
        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        Status = status;
        TotalAmount = Items.Sum(i => i.TotalPrice);
        ShippingAddressId = shippingAddressId;
    }

    public static OrderModel CreateOrder(Guid customerId, Guid shippingAddressId)
    {
        if (customerId == Guid.Empty)
            throw new ValidationException("Customer ID is required.");

        if (shippingAddressId == Guid.Empty)
            throw new ValidationException("Shipping address id is required.");
        
        return new OrderModel(Guid.NewGuid(), customerId, DateTime.Now, OrderStatus.Created, shippingAddressId);
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        switch (newStatus)
        {
            case OrderStatus.InProgress:
                if (Status != OrderStatus.Created)
                    throw new OrderStatusException($"Only {nameof(OrderStatus.Created)} orders can be {nameof(OrderStatus.InProgress)}.");
                Status = newStatus;
                break;

            case OrderStatus.Shipped:
                if (Status != OrderStatus.InProgress)
                    throw new OrderStatusException($"Only {nameof(OrderStatus.InProgress)} orders can be {nameof(OrderStatus.Shipped)}.");
                Status = newStatus;
                break;

            case OrderStatus.Completed:
                if (Status != OrderStatus.Shipped)
                    throw new OrderStatusException($"Only {nameof(OrderStatus.Shipped)} orders can be {nameof(OrderStatus.Completed)}.");
                Status = newStatus;
                break;

            case OrderStatus.Cancelled:
                if (Status == OrderStatus.Completed)
                    throw new OrderStatusException($"{nameof(OrderStatus.Completed)} orders cannot be {nameof(OrderStatus.Cancelled)}.");
                Status = newStatus;
                break;

            default:
                throw new OrderStatusException($"Order status {newStatus} is invalid.");
        }
    }
}
