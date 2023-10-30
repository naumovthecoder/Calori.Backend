using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Calori.Domain.Models.Auth
{
    public class LoginResponse
    {
        public JwtSecurityToken Token { get; set; }
        public List<string> Messages { get; set; }
    }
}