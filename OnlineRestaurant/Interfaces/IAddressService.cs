using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAddressesAsync(string token, PaginateDto dto);
        Task<Address> GetAddressByIdAsync (int id);
        Task<Address> CreateAddressAsync(string token,Address address);
        Task<Address> UpdateAddressAsync(Address address,UpdateAddressDto updateAddressDto);
        Task<Address> DeleteAddress(Address address);  
    }
}
