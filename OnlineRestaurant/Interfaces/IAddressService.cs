using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAddressesAsync(string token);
        Task<Address> GetAddressByIdAsync (int id);
        Task<Address> CreateAddressAsync(string token,Address address);
        Address UpdateAddressAsync(Address address,UpdateAddressDto updateAddressDto);
        Address DeleteAddress(Address address);  
    }
}
