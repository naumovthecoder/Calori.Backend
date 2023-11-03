using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Auth.Commands.Login;
using Calori.Application.Auth.Commands.ResetPassword;
using Calori.Domain.Models.Auth;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AuthenticateController(UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (User == null)
            {
                return Unauthorized();
            }
            
            var user = await _userManager.FindByEmailAsync(User!.Identity!.Name);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }
        
        [HttpPost]
        [Route("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassModel model)
        {
            var command = _mapper.Map<ResetPasswordCommand>(model);
            var response = await Mediator.Send(command);

            return Ok(response.Message);
        }
        
        [HttpPost]
        [Route("check")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            return Ok();
        }
    }
}