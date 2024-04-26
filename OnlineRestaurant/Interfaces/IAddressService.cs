using OnlineRestaurant.Dtos;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAddressesAsync(string token, PaginateDto dto);
        Task<Address> CreateAddressAsync(string token,Address address);
        Task<Address> UpdateAddressAsync(int id,UpdateAddressDto updateAddressDto);
        Task<Address> DeleteAddress(int id);  
    }
}
