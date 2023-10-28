using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Calori.WebApi.IdentityAuth;
using Calori.WebApi.Models;
using Calori.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Calori.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new Response { Status = "Error", Messages = new []
                    {
                        "User already exists!"
                    } });
            
        
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            // var password = new PasswordGenerator().GeneratePassword();
            var result = await _userManager.CreateAsync(user, model.Password);
            var errorMessages = result.Errors.Select(error => error.Description).ToList();
            errorMessages.Add("User creation failed! Please check user details and try again.");
            
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error", 
                    Messages = errorMessages.ToArray()
                });
        
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
        
            return Ok(new Response { Status = "Success", Messages = new []
            {
                "User created successfully!"
            } });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
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

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }
    }
}