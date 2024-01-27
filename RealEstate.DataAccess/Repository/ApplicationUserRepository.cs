using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.DataAccess.Data;
using RealEstate.Dto;
using RealEstate.Models.Identity;
using RealEstate.Utility;

namespace RealEstate.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {

        private readonly UserManager<ApplicationUser> _um;
        private readonly IUserStore<ApplicationUser> _us;
        private readonly IUserEmailStore<ApplicationUser> _es;

        private readonly string _jwtAccessSecret;

        public ApplicationUserRepository(
            ApplicationDbContext db,
            IConfiguration config,
            UserManager<ApplicationUser> um,
            IUserStore<ApplicationUser> us
        ) : base(db)
        {
            _um = um;
            _us = us;
            _es = GetEmailStore();
            _jwtAccessSecret = config.GetValue<string>("ApiSettings:JwtAccessSecret") ?? "";
        }

        public async Task<bool> IsUinqueUser(string UserName)
        {
            var user = await _um.FindByNameAsync(UserName);

            return user is null;
        }

        public async Task<ApplicationUserLoginResponseDto?> Login(ApplicationUserLoginRequestDto loginRequestDto)
        {
            // Note this is for demo only so no password hassing applied
            // in realworld scenario, please use somthing like bcrypt
            // to seed and hash the password. NEVER store plaintext
            // passwords in the database. Later we will use .net Identity
            // to take care of all the heavy lifting here.

            var user = await _um.FindByNameAsync(loginRequestDto.UserName);

            if (user is null || user.UserName is null) return null;

            bool isValidCredentials = await _um.CheckPasswordAsync(user, loginRequestDto.Password);

            if (!isValidCredentials) return null;

            var roles = await _um.GetRolesAsync(user);

            if (roles.Count < 1) return null;

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtAccessKey = Encoding.ASCII.GetBytes(_jwtAccessSecret);

            var jwtTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.First())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtAccessKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtAccessToken = jwtTokenHandler.WriteToken(jwtTokenHandler.CreateToken(jwtTokenDescriptor));

            return new ApplicationUserLoginResponseDto(
                jwtAccessToken
            );
        }

        public async Task<ApplicationUserLoginResponseDto?> Register(CreateApplicationUserDto userDto)
        {
            if (await IsUinqueUser(userDto.UserName))
            {
                var user = CreateUser();
                await _us.SetUserNameAsync(user, userDto.UserName, CancellationToken.None);
                await _es.SetEmailAsync(user, userDto.UserName, CancellationToken.None);
                user.Name = userDto.Name;
                var result = await _um.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    if (userDto.Role is not null && userDto.Role != "")
                    {
                        await _um.AddToRoleAsync(user, userDto.Role);
                    }
                    else
                    {
                        await _um.AddToRoleAsync(user, SD.Role_Customer);
                    }
                    return await Login(new ApplicationUserLoginRequestDto(
                        userDto.UserName,
                        userDto.Password
                    ));
                }
            }

            return null;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                // return Activator.CreateInstance<ApplicationUser>();
                return Activator.CreateInstance<ApplicationUser>(); // updated so we can customize info we collected on a user 
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_um.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_us;
        }
    }
}