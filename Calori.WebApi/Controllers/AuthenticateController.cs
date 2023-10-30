using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Auth.Commands.Login;
using Calori.Application.Auth.Commands.ResetPassword;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : BaseController
    {
        private readonly IMapper _mapper;

        public AuthenticateController(IMapper mapper) => _mapper = mapper;
        
        // private readonly UserManager<ApplicationUser> _userManager;
        // private readonly RoleManager<IdentityRole> _roleManager;
        // private readonly IConfiguration _configuration;
        //
        // public AuthenticateController(UserManager<ApplicationUser> userManager, 
        //     RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        // {
        //     _userManager = userManager;
        //     _roleManager = roleManager;
        //     _configuration = configuration;
        // }
        
        // [HttpPost]
        // [Route("register")]
        // public async Task<IActionResult> Register([FromBody] RegisterModel model)
        // {
        //     var userExists = await _userManager.FindByEmailAsync(model.Email);
        //     if (userExists != null)
        //         return StatusCode(StatusCodes.Status500InternalServerError, 
        //             new Response { Status = "Error", Messages = new []
        //             {
        //                 "User already exists!"
        //             } });
        //     
        //
        //     ApplicationUser user = new ApplicationUser()
        //     {
        //         UserName = model.UserName,
        //         Email = model.Email,
        //         SecurityStamp = Guid.NewGuid().ToString(),
        //     };
        //     // var password = new PasswordGenerator().GeneratePassword();
        //     var result = await _userManager.CreateAsync(user, model.Password);
        //     var errorMessages = result.Errors.Select(error => error.Description).ToList();
        //     errorMessages.Add("User creation failed! Please check user details and try again.");
        //     
        //     if (!result.Succeeded)
        //         return StatusCode(StatusCodes.Status500InternalServerError, new Response
        //         {
        //             Status = "Error", 
        //             Messages = errorMessages.ToArray()
        //         });
        //
        //     // var emailService = new EmailService.EmailService(
        //     //     smtpServer: _configuration["EmailSettings:SmtpServer"], 
        //     //     smtpPort: int.Parse(_configuration["EmailSettings:SmtpPort"]),
        //     //     smtpUsername: _configuration["EmailSettings:SmtpUsername"],
        //     //     smtpPassword: _configuration["EmailSettings:SmtpPassword"]
        //     //     );
        //     //
        //     // string toEmail = "naumovthecoder@icloud.com";
        //     // string subject = "Test Email from Calori.App";
        //     // string body = $"Welcome to Calori.App!\nYour Login - {model.Email}\nPassword - {password}";
        //     //
        //     // try
        //     // {
        //     //     await emailService.SendEmailAsync(toEmail, subject, body);
        //     // }
        //     // catch (Exception e)
        //     // {
        //     //     return StatusCode(StatusCodes.Status500InternalServerError, new Response
        //     //     {
        //     //         Status = "Error", 
        //     //         Messages = new []{$"Error when sending an e-mail. Message - {e.Message}"}
        //     //     });
        //     // }
        //
        //     return Ok(new Response { Status = "Success", Messages = new []
        //     {
        //         "User created successfully!"
        //     } });
        // }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var command = _mapper.Map<LoginUserCommand>(model);
            var response = await Mediator.Send(command);
            
            if (response.Token != null)
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(response.Token),
                    expiration = response.Token.ValidTo
                });
            }

            return Unauthorized();
        }
        
        [HttpPost]
        [Route("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassModel model)
        {
            var command = _mapper.Map<ResetPasswordCommand>(model);
            var response = await Mediator.Send(command);

            return Ok(response.Message);
        }
    }
}