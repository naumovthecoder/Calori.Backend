using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
using Calori.Application.PersonalPlan.Queries;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.PersonalPlan.Requests;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PersonalPlanController : BaseController
    {
        private readonly IMapper _mapper;

        public PersonalPlanController(IMapper mapper) => _mapper = mapper;
        
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PersonalPlanDetailsVm>> Get(int id)
        {
            //var command = _mapper.Map<CreateCaloriApplicationCommand>(dto);
            var query = new GetPersonalPlanDetailsQuery()
            {
                Id = id
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
        
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update([FromBody] UpdatePersonalPlanDto dto)
        {
            var command = _mapper.Map<UpdatePersonalSlimmingPlanCommand>(dto);
            //command.UserId = UserId;
            await Mediator.Send(command);
            return NoContent();
        }
    }
}