using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;

namespace OnlineRestaurant.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AddressService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAuthService authService, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<Address> CreateAddressAsync(string token,Address address)
        {
            var userId = _authService.GetUserId(token);
            address.UserId = userId;
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
                return new Address { Message = "No User is Found!" };
            
            await _context.AddAsync(address);
            await _context.SaveChangesAsync();
            return address;

        }

        public async Task<Address> DeleteAddress(int id)
        {
            var address = await GetAddressByIdAsync(id);
            if (address == null)
                return new Address { Message = $"There is no Address with Id :{id}" };
            _context.Remove(address);
            await _context.SaveChangesAsync();
            return address;
        }

        private async Task<Address> GetAddressByIdAsync(int id)
        {
            return  await _context.Addresses.SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(string token,PaginateDto dto)
        {
            var userId = _authService.GetUserId(token);
            
            if (!await _userManager.Users.AnyAsync(c => c.Id == userId))
                return null;

            return _context.Addresses.Where(a=>a.UserId== userId).Paginate(dto.Page, dto.Size).ToList();
        }

        public async Task<Address> UpdateAddressAsync(int id,UpdateAddressDto updateAddressDto)
        {
            var address=await GetAddressByIdAsync(id);
            if (address == null)
                return new Address { Message = $"There is no Address with Id :{id}" };

            _mapper.Map(updateAddressDto, address);
            await _context.SaveChangesAsync();
            return address;
        }
    }
}
