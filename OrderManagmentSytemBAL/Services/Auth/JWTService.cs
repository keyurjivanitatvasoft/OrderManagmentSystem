using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagmentSytemDAL.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.Services.Auth
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration configuration;

        public JWTService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId",user.UserId.ToString()),
                new Claim("UserName",user.UserName),
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Convert.ToString(configuration["Jwt:Key"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires =
                DateTime.Now.AddMinutes(10);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool Validate(string jwtToken, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;
            if (jwtToken == null)
                return false;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Convert.ToString(configuration["Jwt:Key"]));
            try
            {
                tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                jwtSecurityToken = (JwtSecurityToken)validatedToken;
                return jwtSecurityToken.ValidTo >= DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

    }
}
