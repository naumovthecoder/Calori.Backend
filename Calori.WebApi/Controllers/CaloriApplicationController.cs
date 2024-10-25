using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Auth.Commands.RemoveAllUsers;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.CaloriApplications.Commands.UpdateApplication;
using Calori.Application.CaloriApplications.Queries;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Auth;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/application")]
    public class CaloriApplicationController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CaloriApplicationController(IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }
        
        [HttpPost]
        public async Task<ActionResult<CaloriApplication>> Create([FromBody] CreateCaloriApplicationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, "User with this email already exists.");
            }
            
            var request = HttpContext.Request;
            string acceptLanguageHeader = request.Headers["Accept-Language"];
            var cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(acceptLanguageHeader);


            var command = _mapper.Map<CreateCaloriApplicationCommand>(dto);
            command.CultureInfo = cultureInfo;
            var application = await Mediator.Send(command);
            return Ok(application);
        }
        
        [HttpPost("weight")]
        public async Task<ActionResult<RecommendedIdealWeightDto>> GetIdealWeight(
            [FromBody] CalculateMinMaxWeightDto dto)
        {
            var result = new RecommendedIdealWeightDto();
            
            if (dto.Weight.HasValue && dto.Height.HasValue && dto.Weight != 0 && dto.Height != 0)
            {
                decimal heightM = dto.Height.Value / 100.0m; 

                result.MaxWeight = (int)(24.9m * heightM * heightM);
                result.MinWeight = (int)(18.5m * heightM * heightM);
            }
            else
            {
                result.ErrorMessage = "Incorrect weight and height data.";
            }

            return result;
        }
        
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UpdateApplicationResult>> Update([FromBody] UpdateApplicationDto dto)
        {
            var request = HttpContext.Request;
            string acceptLanguageHeader = request.Headers["Accept-Language"];
            var cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(acceptLanguageHeader);

            string userEmail = User!.Identity!.Name;
            var command = _mapper.Map<UpdateApplicationCommand>(dto);
            // command.ApplicationUser = User;
            command.IdentityUserEmail = userEmail;
            command.CultureInfo = cultureInfo;
            var result = await Mediator.Send(command);
            return result;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApplicationDetailsVm>> Get()
        {
            
            if (User == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(User!.Identity!.Name);
            
            var query = new GetApplicationDetailsQuery
            {
                UserId = user.Id
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
        
        [HttpPost("users/removeall")]
        public async Task<ActionResult> RemoveAllUsers()
        {
            
            var command = _mapper.Map<RemoveAllUsersCommand>(new RemoveAllUsersCommand());
            var result = await Mediator.Send(command);

            return Ok(result.Message);
        }
    }
}