using System;
using System.Threading.Tasks;
using Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan;
using Calori.Domain.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PersonalPlanController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PersonalPlanController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AutoCalculatedPlanVm>> Get()
        {
            if (User == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(User!.Identity!.Name);
            
            var query = new GetAutoCalculatedPlanQuery()
            {
                //UserEmail = User!.Identity!.Name,
                UserId = user.Id,
                CurrentDate = DateTime.Today
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
    }
}
