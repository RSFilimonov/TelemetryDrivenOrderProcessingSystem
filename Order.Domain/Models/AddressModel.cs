using System.ComponentModel.DataAnnotations;

namespace Order.Domain.Models;

public class AddressModel
{
    public Guid Id { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
    public string Country { get; init; }

    public ICollection<OrderModel> Orders { get; init; } = null!;

    private AddressModel(Guid id, string street, string city, string zipCode, string country)
    {
        Id = id;
        Street = street;
        City = city;
        ZipCode = zipCode;
        Country = country;
    }

    public static AddressModel CreateAddress(string street, string city, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ValidationException("Street is required.");

        if (string.IsNullOrWhiteSpace(city))
            throw new ValidationException("City is required.");

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ValidationException("Zip code is required.");

        if (string.IsNullOrWhiteSpace(country))
            throw new ValidationException("Country is required.");
        
        return new AddressModel(Guid.NewGuid(), street, city, zipCode, country);
    }
}
