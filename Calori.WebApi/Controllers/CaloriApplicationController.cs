using System;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.CaloriApplications.Commands.UpdateApplication;
using Calori.Application.CaloriApplications.Queries;
using Calori.Domain.Models.ApplicationModels;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/application")]
    public class CaloriApplicationController : BaseController
    {
        private readonly IMapper _mapper;

        public CaloriApplicationController(IMapper mapper) => _mapper = mapper;
        
        [HttpPost]
        public async Task<ActionResult<CaloriApplication>> Create([FromBody] CreateCaloriApplicationDto dto)
        {
            var command = _mapper.Map<CreateCaloriApplicationCommand>(dto);
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
            string userEmail = User!.Identity!.Name;
            var command = _mapper.Map<UpdateApplicationCommand>(dto);
            Console.WriteLine(userEmail);
            // command.ApplicationUser = User;
            command.IdentityUserEmail = userEmail;
            var result = await Mediator.Send(command);
            return result;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApplicationDetailsVm>> Get()
        {
            string userEmail = User!.Identity!.Name;
            
            var query = new GetApplicationDetailsQuery
            {
                Email = userEmail
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }
    }
}