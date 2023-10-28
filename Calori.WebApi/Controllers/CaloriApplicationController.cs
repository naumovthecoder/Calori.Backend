using System;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.CaloriApplications.Commands.UpdateApplication;
using Calori.Application.CaloriApplications.Queries;
using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api")]
    public class CaloriApplicationController : BaseController
    {
        private readonly IMapper _mapper;

        public CaloriApplicationController(IMapper mapper) => _mapper = mapper;
        
        [HttpPost("application")]
        public async Task<ActionResult<CaloriApplication>> Create([FromBody] CreateCaloriApplicationDto dto)
        {
            var command = _mapper.Map<CreateCaloriApplicationCommand>(dto);
            // command.UserId = UserId;
            var application = await Mediator.Send(command);
            return Ok(application);
        }
        
        [HttpPost("application/weight")]
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
        
        [HttpPut("application")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update([FromBody] UpdateApplicationDto dto)
        {
            var command = _mapper.Map<UpdateApplicationCommand>(dto);
            await Mediator.Send(command);
            return NoContent();
        }
        
        [HttpGet("application/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationDetailsVm>> Get(int id)
        {
            //var command = _mapper.Map<CreateCaloriApplicationCommand>(dto);
            var query = new GetApplicationDetailsQuery()
            {
                Id = id
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
    }
}