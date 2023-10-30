using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Calori.Domain.Models.Auth
{
    public class RegisterResponse
    {
        public ApplicationUser User { get; set; }
        public string Token { get; set; }
        public List<string> Messages { get; set; }
    }
}