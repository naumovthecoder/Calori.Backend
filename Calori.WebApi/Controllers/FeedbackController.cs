using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Feedback;
using Calori.Domain.Models.CaloriAccount;
using Calori.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calori.WebApi.Controllers
{
    [Route("api/form")]
    public class FeedbackController : BaseController
    {
        private readonly IMapper _mapper;
        
        public FeedbackController(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        [HttpPost]
        public async Task<ActionResult<CaloriFeedbackResponse>> Feedback(
            [FromBody] FeedbackDto dto)
        {
            var command = _mapper.Map<CreateFeedbackCommand>(dto);
            var result = await Mediator.Send(command);
            
            return Ok(result);
        }
    }
}