using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Domain.Models.ApplicationModels;
using Calori.WebApi.Models;
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
    }
}