using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Common.Exceptions;
using Calori.Application.Services.UserService;
using Calori.Domain.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Calori.Application.Auth.Commands.Register
{
    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, RegisterResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly IConfiguration _configuration;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<RegisterResponse> Handle(RegisterUserCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new RegisterResponse();
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            
            if (userExists != null)
            {
                throw new UserAlreadyExistsException(request.Email);
            }
        
            ApplicationUser user = new ApplicationUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            
            var password = new PasswordGenerator().GeneratePassword();
            var identityResult = await _userManager.CreateAsync(user, password);
            
            var token = await GetAuthToken(user);

            var errorMessages = identityResult.Errors.Select(error => error.Description).ToList();
            errorMessages.Add("User creation failed! Please check user details and try again.");

            result.Token = new JwtSecurityTokenHandler().WriteToken(token);
            result.User = user;
            
            // if (!result.Succeeded)
            //     return StatusCode(StatusCodes.Status500InternalServerError, new Response
            //     {
            //         Status = "Error", 
            //         Messages = errorMessages.ToArray()
            //     });
        
            // var emailService = new EmailService.EmailService(
            //     smtpServer: _configuration["EmailSettings:SmtpServer"], 
            //     smtpPort: int.Parse(_configuration["EmailSettings:SmtpPort"]),
            //     smtpUsername: _configuration["EmailSettings:SmtpUsername"],
            //     smtpPassword: _configuration["EmailSettings:SmtpPassword"]
            //     );
            //
            // string toEmail = "naumovthecoder@icloud.com";
            // string subject = "Test Email from Calori.App";
            // string body = $"Welcome to Calori.App!\nYour Login - {model.Email}\nPassword - {password}";
            //
            // try
            // {
            //     await emailService.SendEmailAsync(toEmail, subject, body);
            // }
            // catch (Exception e)
            // {
            //     return StatusCode(StatusCodes.Status500InternalServerError, new Response
            //     {
            //         Status = "Error", 
            //         Messages = new []{$"Error when sending an e-mail. Message - {e.Message}"}
            //     });
            // }
        
            // return Ok(new Response { Status = "Success", Messages = new []
            // {
            //     "User created successfully!"
            // } });

            return result;
        }

        private async Task<JwtSecurityToken> GetAuthToken(ApplicationUser user)
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

            return token;
        }
    }
}