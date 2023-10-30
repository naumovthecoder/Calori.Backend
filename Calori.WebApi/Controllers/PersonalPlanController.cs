// using System.Threading.Tasks;
// using AutoMapper;
// using Calori.Application.CaloriApplications.Commands.CreateApplication;
// using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
// using Calori.Application.PersonalPlan.Queries;
// using Calori.Domain.Models.ApplicationModels;
// using Calori.Domain.Models.PersonalPlan.Requests;
// using Calori.WebApi.Models;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Calori.WebApi.Controllers
// {
//     [Route("api/[controller]")]
//     public class PersonalPlanController : BaseController
//     {
//         private readonly IMapper _mapper;
//
//         public PersonalPlanController(IMapper mapper) => _mapper = mapper;
//         
//         [HttpGet]
//         [Authorize]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//         public async Task<ActionResult<PersonalPlanDetailsVm>> Get()
//         {
//             var query = new GetPersonalPlanDetailsQuery
//             {
//                 UserEmail = User!.Identity!.Name
//             };
//             var vm = await Mediator.Send(query);
//             return Ok(vm);
//         }
//         
//         [HttpPut]
//         [Authorize]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//         public async Task<IActionResult> Update([FromBody] UpdatePersonalPlanDto dto)
//         {
//             var command = _mapper.Map<UpdatePersonalSlimmingPlanCommand>(dto);
//             await Mediator.Send(command);
//             return NoContent();
//         }
//     }
// }