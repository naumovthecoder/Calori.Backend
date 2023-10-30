using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calori.Domain.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Calori.Application.Auth.Commands.Login
{
    public class LoginUserCommandHandler
        : IRequestHandler<LoginUserCommand, LoginResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public LoginUserCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var response = new LoginResponse();
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        authSigningKey, SecurityAlgorithms.HmacSha256)
                );
                
                response.Token = token;

                return response;
            }
            
            return response;
        }
    }
}


// [HttpPost]
        // [Route("login")]
        // public async Task<IActionResult> Login([FromBody] LoginModel model)
        // {
        //     var user = await _userManager.FindByEmailAsync(model.Email);
        //     if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //     {
        //         var userRoles = await _userManager.GetRolesAsync(user);
        //
        //         var authClaims = new List<Claim>
        //         {
        //             new(ClaimTypes.Name, user.UserName),
        //             new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //         };
        //
        //         foreach (var userRole in userRoles)
        //         {
        //             authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //         }
        //
        //         var authSigningKey = new SymmetricSecurityKey(
        //             Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
        //
        //         var token = new JwtSecurityToken(
        //             issuer: _configuration["JWT:ValidIssuer"],
        //             audience: _configuration["JWT:ValidAudience"],
        //             expires: DateTime.Now.AddHours(3),
        //             claims: authClaims,
        //             signingCredentials: new SigningCredentials(
        //                 authSigningKey, SecurityAlgorithms.HmacSha256)
        //         );
        //
        //         return Ok(new
        //         {
        //             token = new JwtSecurityTokenHandler().WriteToken(token),
        //             expiration = token.ValidTo
        //         });
        //     }
        //
        //     return Unauthorized();
        // }