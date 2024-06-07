using OrderManagmentSytemDAL.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.Services.Auth
{
    public interface IJWTService
    {
        string GenerateJwtToken(User user);

        bool Validate(string jwtToken, out JwtSecurityToken jwtSecurityToken);
    }
}
