using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _peerLandingContext;
        private readonly IConfiguration _configuration;

        public UserServices(PeerlandingContext peerLandingContext, IConfiguration configuration)
        {
            _peerLandingContext = peerLandingContext;
            _configuration = configuration;
        }        

        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _peerLandingContext.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);

            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance,
            };

            await _peerLandingContext.MstUsers.AddAsync(newUser);
            await _peerLandingContext.SaveChangesAsync();

            return newUser.Name;
        }

        public async Task<List<ResUserDto>> GetAllUser()
        {
            return await _peerLandingContext.MstUsers
                .Where(user => user.Role != "admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                }).ToListAsync();
        }

        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _peerLandingContext.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if(user == null)
            {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new ResLoginDto
            {
                Token = token,
                UserId = user.Id,
            };

            return loginResponse;
        }

        private string GenerateJwtToken(MstUser user)
        {
            var jwtSetting = _configuration.GetSection("JWTSetting");
            var secretKey = jwtSetting["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSetting["ValidIssuer"],
                audience: jwtSetting["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        async Task<string> IUserServices.Update(string userId, ReqUpdateUserDto reqUpdate)
        {
            var existingUser = await _peerLandingContext.MstUsers.SingleOrDefaultAsync(user => user.Id == userId);

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            existingUser.Name = reqUpdate.Name ?? existingUser.Name;
            existingUser.Role = reqUpdate.Role ?? existingUser.Role;
            existingUser.Balance = reqUpdate.Balance ?? existingUser.Balance;

            _peerLandingContext.MstUsers.Update(existingUser);
            await _peerLandingContext.SaveChangesAsync();

            return reqUpdate.Name;
        }

        public async Task<string> Delete(string userId)
        {
            var user = await _peerLandingContext.MstUsers.SingleOrDefaultAsync(user => user.Id == userId);
            

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var userName = user.Name;

            _peerLandingContext.MstUsers.Remove(user);
            await _peerLandingContext.SaveChangesAsync();
            return userName;
        }

        public async Task<ResUserByIdDto> GetUserById(string userId)
        {
            var user = await _peerLandingContext.MstUsers
            .Where(user => user.Id == userId)
            .Select(user => new ResUserByIdDto
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role,
                Balance = user.Balance
            })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }
    }
}
