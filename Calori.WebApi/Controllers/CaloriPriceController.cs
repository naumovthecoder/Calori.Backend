using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.CaloriApplications.Queries;
using Calori.Application.Common.Exceptions;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public CaloriPriceController(ICaloriDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            if (User == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByEmailAsync(User!.Identity!.Name);

            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var application = await _dbContext.CaloriApplications
                    .FirstOrDefaultAsync(a => a.UserId.ToLower() == user.Id.ToLower());

                if (application == null)
                {
                    return NoContent();
                }
                
                var personalPlan = await _dbContext.PersonalSlimmingPlan
                    .FirstOrDefaultAsync(p => p.Id == application.PersonalSlimmingPlanId);
            
                var prices = await _dbContext.CaloriPrices
                    .Where(p => p.CaloriSlimmingPlanId == personalPlan.CaloriSlimmingPlanId)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                var response = prices.Select(item => new GetPricesResponse
                {
                    PriceId = item.PriceId,
                    Name = item.Name,
                    Price = item.Price
                }).ToList();
            
                return Ok(response);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public record GetPricesResponse
        {
            public string PriceId { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
        }
    }
}