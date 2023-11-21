using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

using System.IdentityModel.Tokens.Jwt;

namespace OnlineRestaurant.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Address> CreateAddressAsync(string token,Address address)
        {
            var errormessages = ValidateHelper<Address>.Validate(address);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new Address { Message = errormessages };
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return new Address { Message = "No User is Found!" };
            }
            

            var Address = new Address
            {
                DepartmentNum=address.DepartmentNum,
                City=address.City,
                PhoneNumber=address.PhoneNumber,
                Street=address.Street,
                UserId=userId,
            };
            
            await _context.AddAsync(Address);
           await _context.SaveChangesAsync();
            return Address;

        }

        public async Task<Address> DeleteAddress(Address address)
        {
            _context.Remove(address);
           await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            var address = await _context.Addresses.SingleOrDefaultAsync(a => a.Id == id);
            if (address == null)
                return new Address { Message = $"There is no Address with Id :{id}" };
            return address;
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
            {
                return null;
            }
            var addresses =await _context.Addresses.Where(a=>a.UserId== userId).ToListAsync();
           
            return addresses;
        }

        public async Task<Address> UpdateAddressAsync(Address address,UpdateAddressDto updateAddressDto)
        {
            var errormessages = ValidateHelper<UpdateAddressDto>.Validate(updateAddressDto);
            if (!string.IsNullOrEmpty(errormessages))
            {
                return new Address { Message = errormessages };
            }
            
            address.Street=updateAddressDto.Street??address.Street;
            address.City=updateAddressDto.City??address.City;
            address.PhoneNumber = updateAddressDto.PhoneNumber ?? address.PhoneNumber;
            address.DepartmentNum=updateAddressDto.DepartmentNum??address.DepartmentNum;
            

            _context.Update(address);
           await _context.SaveChangesAsync();

            return address;
        }
    }
}
