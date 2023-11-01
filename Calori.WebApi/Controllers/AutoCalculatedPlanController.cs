using System;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
using Calori.Application.PersonalPlan.Queries;
using Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan;
using Calori.Domain.Models.Auth;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AutoCalculatedPlanController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AutoCalculatedPlanController(IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AutoCalculatedPlanVm>> Get()
        {
            string userEmail = User!.Identity!.Name;
            
            var query = new GetAutoCalculatedPlanQuery()
            {
                UserEmail = userEmail,
                CurrentDate = DateTime.Today
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
        
        // [HttpPut]
        // [Authorize]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // public async Task<IActionResult> Update([FromBody] UpdatePersonalPlanDto dto)
        // {
        //     var command = _mapper.Map<UpdatePersonalSlimmingPlanCommand>(dto);
        //     await Mediator.Send(command);
        //     return NoContent();
        // }
    }
}
