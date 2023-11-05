using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.Queries;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calori.WebApi.Controllers
{
    [Route("api/prices")]
    public class CaloriPriceController : BaseController
    {
        private readonly ICaloriDbContext _dbContext;

        public CaloriPriceController(ICaloriDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<OkObjectResult> Get()
        {
            var prices = await _dbContext.CaloriPrices.ToListAsync();
            
            return Ok(prices);
        }
    }
}