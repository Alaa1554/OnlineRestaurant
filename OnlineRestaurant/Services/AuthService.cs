using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineRestaurant.Data;
using OnlineRestaurant.Dtos;
using OnlineRestaurant.Helpers;
using OnlineRestaurant.Interfaces;
using OnlineRestaurant.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;




namespace OnlineRestaurant.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImageService _imgService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memorycashe;
        private readonly TimeSpan _codeExpiration=TimeSpan.FromMinutes(15);

        public AuthService(UserManager<ApplicationUser> userManager, IEmailSender emailsender, IMemoryCache memoryCache, IMapper mapper, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager, IImageService imgService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _imgService = imgService;
            _context = context;
            _emailSender = emailsender;
            _memorycashe = memoryCache;
        }

        public async Task<string> RegisterAsync(RegisterModelDto registermodel)
        {
            string encryptedEmail = Helpers.Encryption.Encrypt(registermodel.Email);

            // Encrypt the Firstname
            registermodel.FirstName = Helpers.Encryption.Encrypt(registermodel.FirstName);

            // Encrypt the Lastname
            registermodel.LastName = Helpers.Encryption.Encrypt(registermodel.LastName);

            // Encrypt the Username
            registermodel.UserName = Helpers.Encryption.Encrypt(registermodel.UserName);
            if (registermodel.Password.Length < 6)
            {
                return  "الباسورد يجب ان يحتوي علي 6 حروف او ارقام علي الاقل" ;
            }
            if (await _userManager.FindByEmailAsync(encryptedEmail) is not null)
            {
                return "البريد الالكتروني موجود بالفعل" ;
            }
            if (await _userManager.FindByNameAsync(registermodel.UserName) is not null)
            {
                return  "اسم المستخدم موجود بالفعل" ;
            }
            var user = _mapper.Map<ApplicationUser>(registermodel);
            user.Email= encryptedEmail;
            user.UserImgUrl =registermodel.UserImg==null?null: _imgService.Upload(registermodel.UserImg);
            var result = await _userManager.CreateAsync(user, registermodel.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}\t";
                }
                return errors;
            }
            
            await _userManager.AddToRoleAsync(user, "User");
            var verificationCode = GenerateRandomCode();
            _emailSender.SendEmail(registermodel.Email, "Verification Code", $"Your verification code is {verificationCode}");
            _memorycashe.Set($"{user.Id} verification", verificationCode, _codeExpiration);
            return string.Empty;
        }
        public async Task<AuthModelDto> VerifyAccountAsync(VerifyAccountDto verifyaccount)
        {
            string encryptedEmail = Helpers.Encryption.Encrypt(verifyaccount.Email);
            var user=await _userManager.FindByEmailAsync(encryptedEmail);
            if(user==null)
                return new AuthModelDto { Message= "لم يتم العثور علي اي مستخدم" };
            if (!_memorycashe.TryGetValue($"{user.Id} verification", out string cashedcode))
                return new AuthModelDto { Message = "لم يتم العثور علي رمز التحقق او تم انتهاء صلاحيته " };
            if (verifyaccount.VerificationCode != cashedcode)
                return new AuthModelDto { Message = "رمز التحقق الذي ادخلته غير صحيح" };
            user.EmailConfirmed = true;
            var userWishList = new WishList
            {
                UserId = user.Id
            };
            await _context.wishLists.AddAsync(userWishList);
            await _context.SaveChangesAsync();
            return await GetAuthModelDto(user);
        }
        public async Task<string> ForgetPassword(EmailDto emailDto)
        {
            string encryptedEmail = Helpers.Encryption.Encrypt(emailDto.Email);
            var user = await _userManager.FindByEmailAsync(encryptedEmail);
            if (user == null)
                return "لم يتم العثور علي اي مستخدم";
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var callBackUrl = $"http://localhost:3000/reset-password/{token}";
             _emailSender.SendEmail(
                emailDto.Email,
                "Reset Password",
                $"Please reset your password by <a href={HtmlEncoder.Default.Encode(callBackUrl)}>clicking here</a>.");
            return string.Empty;
        }
        public async Task<string> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            string encryptedEmail = Helpers.Encryption.Encrypt(resetPasswordDto.Email);
            var user = await _userManager.FindByEmailAsync(encryptedEmail);
            if (user == null)
                return "لم يتم العثور علي اي مستخدم";
            var bytes = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
            var token = Encoding.UTF8.GetString(bytes);
            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider,"ResetPassword",token);
            if (!result)
                return "حدثت مشكله اثناء التحقق يرجي المحاوله مره اخري";
            var resetPasswordResult=await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);
            if (!resetPasswordResult.Succeeded)
                return "حدثت مشكله اثناء تغيير الباسورد يرجي المحاوله مره اخري";
            return string.Empty;
        }
        public async Task<string> ResendVerificationCode(string email)
        {
            string encryptedEmail = Helpers.Encryption.Encrypt(email);
            var user = await _userManager.FindByEmailAsync(encryptedEmail);
            if (user == null)
                return "لم يتم العثور علي اي مستخدم";
            var verificationCode = GenerateRandomCode();
            _emailSender.SendEmail(email, "Verification Code", $"Your verification code is {verificationCode}");
            _memorycashe.Set($"{user.Id} verification", verificationCode, _codeExpiration);
            return string.Empty;
        } 
        public async Task<AuthModelDto> GetTokenAsync(TokenRequestDto tokenrequest)
        {
            if (tokenrequest.Password.Length < 6)
            {
                return new AuthModelDto { Message = "الباسورد يجب ان يحتوي علي 6 حروف او ارقام علي الاقل" };
            }
            string encryptedEmail = Helpers.Encryption.Encrypt(tokenrequest.Email);
            var user = await _userManager.FindByEmailAsync(encryptedEmail);
            if (user == null || !await _userManager.CheckPasswordAsync(user, tokenrequest.Password)||!user.EmailConfirmed)
            {
                return new AuthModelDto { Message = "البريد الالكتروني او كلمه السر غير صحيحه" };
            }
            return await GetAuthModelDto(user);
        }
        public async Task<string> AddRoleAsync(AddRoleDto role)
        {
            var user = await _userManager.FindByIdAsync(role.UserId);
            if (user == null || await _roleManager.FindByNameAsync(role.Role) == null|| role.Role == "SuperAdmin")
            {
                return "Role or UserId is Invaild";
            }
            if (await _userManager.IsInRoleAsync(user, role.Role))
            {
                return "User Already Assigned to this Role";
            }
            var result = await _userManager.AddToRoleAsync(user, role.Role);
            return result.Succeeded ? string.Empty : "Something Went Wrong";
        }
        public async Task<string> RemoveRoleAsync(string userid)
        {
            var user =await _userManager.FindByIdAsync(userid);
            if (user == null)
                return "No User is Found";
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("Admin"))
                return "Role Already Removed";
            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            return result.Succeeded ? string.Empty : "Something Went Wrong";
        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
       
       
        public async Task<AuthModelDto> GmailRegisterAsync(GmailRegisterDto registermodel)
        {
            string encryptedEmail = Helpers.Encryption.Encrypt(registermodel.Email);
            var user = await _userManager.FindByEmailAsync(encryptedEmail);
            registermodel.Email = encryptedEmail;

            // Encrypt the Firstname
            registermodel.FirstName = Helpers.Encryption.Encrypt(registermodel.FirstName);

            // Encrypt the Lastname
            registermodel.LastName = Helpers.Encryption.Encrypt(registermodel.LastName);

            // Encrypt the Username
            registermodel.UserName = Helpers.Encryption.Encrypt(registermodel.UserName);
            if (user is not null)
            {
                UpdateGmailAccountData(user, registermodel);  
                var result= await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = "";
                    foreach (var error in result.Errors)
                    {
                        errors += $"{error.Description}\t";
                    }
                    return new AuthModelDto { Message = errors };
                }
                return await GetAuthModelDto(user);
            }
            else 
            {
                if (await _userManager.FindByNameAsync(registermodel.UserName) is not null)
                {
                    return new AuthModelDto { Message = "اسم المستخدم موجود بالفعل" };
                }
                user = _mapper.Map<ApplicationUser>(registermodel);
                user.EmailConfirmed = true;
                var result= await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = "";
                    foreach (var error in result.Errors)
                    {
                        errors += $"{error.Description}\t";
                    }
                    return new AuthModelDto { Message = errors };
                }
                await _userManager.AddToRoleAsync(user, "User");

                var userWishList = new WishList
                {
                    UserId = user.Id
                };

                await _context.wishLists.AddAsync(userWishList);
                await _context.SaveChangesAsync();
                return await GetAuthModelDto(user);
            }
        }
        public string GetUserId(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token) as JwtSecurityToken;
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            return userId;
        }
        public async Task<AuthModelDto> GetAuthModelDto(ApplicationUser user)
        {
            var jwtSecurityToken = await CreateJwtToken(user);
            var roleList = await _userManager.GetRolesAsync(user);
            return new AuthModelDto(user.Email, jwtSecurityToken.ValidTo, true, roleList.ToList(), new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), user.UserName, user.UserImgUrl, user.FirstName, user.LastName);
        }
        private string GenerateRandomCode()
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string numbers = "0123456789";
            const int codeLength = 6;

            Random random = new Random();
            char[] code = new char[codeLength];
            code[0] = characters[random.Next(characters.Length)];
            code[1] = numbers[random.Next(numbers.Length)];
           
            for (int i = 2; i < codeLength; i++)
            {
                code[i] = characters[random.Next(characters.Length)];
            }
            
            for (int i = 0; i < codeLength; i++)
            {
                int j = random.Next(i, codeLength);
                char temp = code[i];
                code[i] = code[j];
                code[j] = temp;
            }

            string randomCode = new string(code);

            return randomCode;
        }
        private void UpdateGmailAccountData(ApplicationUser user, GmailRegisterDto dto)
        {
            if (dto.FirstName != user.FirstName)
                user.FirstName = dto.FirstName;
            if (dto.LastName != user.LastName)
                user.LastName = dto.LastName;
            if (dto.UserImgUrl != user.UserImgUrl)
                user.UserImgUrl = dto.UserImgUrl;
            if (dto.UserName != user.UserName)
                user.UserName = dto.UserName;
        }
    }
}

