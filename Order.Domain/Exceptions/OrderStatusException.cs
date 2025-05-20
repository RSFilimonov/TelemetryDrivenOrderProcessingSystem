namespace Order.Domain.Exceptions;

public class OrderStatusException(string message) : Exception(message);