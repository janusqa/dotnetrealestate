using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.DataAccess.Data;
using RealEstate.Dto;
using RealEstate.Models.Domain;

namespace RealEstate.DataAccess.Repository
{
    public class LocalUserRepository : Repository<LocalUser>, ILocalUserRepository
    {
        private readonly string _jwtAccessSecret;

        public LocalUserRepository(ApplicationDbContext db, IConfiguration config) : base(db)
        {
            _jwtAccessSecret = config["ApiSettings:JwtAccessSecret"] ?? "";
        }

        public async Task<bool> IsUinqueUser(string UserName)
        {
            var User = (await FromSqlAsync($@"
                SELECT * FROM dbo.LocalUsers WHERE UserName = @UserName
            ", [new SqlParameter("UserName", UserName)])).FirstOrDefault();

            return User is null;
        }

        public async Task<LocalUserLoginResponseDto?> Login(LocalUserLoginRequestDto loginRequestDto)
        {
            // Note this is for demo only so no password hassing applied
            // in realworld scenario, please use somthing like bcrypt
            // to seed and hash the password. NEVER store plaintext
            // passwords in the database. Later we will use .net Identity
            // to take care of all the heavy lifting here.
            var user = (await FromSqlAsync($@"
                SELECT * FROM dbo.LocalUsers 
                WHERE 
                    LOWER(UserName)= LOWER(@UserName) AND
                    Password = @Password
            ", [
                new SqlParameter("UserName",loginRequestDto.UserName),
                new SqlParameter("Password",loginRequestDto.Password)
            ])).FirstOrDefault();

            if (user is null) return null;

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtAccessKey = Encoding.ASCII.GetBytes(_jwtAccessSecret);
            var jwtTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtAccessKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtAccessToken = jwtTokenHandler.WriteToken(jwtTokenHandler.CreateToken(jwtTokenDescriptor));

            return new LocalUserLoginResponseDto(
                new LocalUserDto(
                    user.Id,
                    user.UserName,
                    user.Role,
                    user.Name
                ),
                jwtAccessToken
            );
        }

        public async Task<LocalUserLoginResponseDto?> Register(CreateLocalUserDto userDto)
        {
            if (await IsUinqueUser(userDto.UserName))
            {
                await ExecuteSqlAsync($@"
                    INSERT INTO dbo.LocalUsers
                    (UserName, Password, Role, Name)
                    OUTPUT inserted.Id 
                    VALUES
                    (@UserName, @Password, @Role, @Name)
                ", [
                    new SqlParameter("Username",userDto.UserName),
                    new SqlParameter("Password",userDto.Password),
                    new SqlParameter("Role",userDto.Role),
                    new SqlParameter("Name", userDto.Name ?? (object)DBNull.Value)
                ]);

                return await Login(new LocalUserLoginRequestDto(
                    userDto.UserName,
                    userDto.Password
                ));
            }

            return null;
        }
    }
}