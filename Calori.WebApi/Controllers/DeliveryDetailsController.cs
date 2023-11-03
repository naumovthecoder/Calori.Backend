using System;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan;
using Calori.Domain.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calori.WebApi.Controllers
{
    [Route("api/delivery-info")]
    public class DeliveryDetailsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICaloriDbContext _dbContext;

        public DeliveryDetailsController(UserManager<ApplicationUser> userManager,
            ICaloriDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> Get()
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

            var deliveryDetails = await _dbContext.CaloriShippingData
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            var details = new Details
            {
                Name = deliveryDetails.Name,
                Phone = deliveryDetails.UserPhone,
                Email = deliveryDetails.UserEmail,
                City = deliveryDetails.City,
                Line1 = deliveryDetails.Line1,
                Line2 = deliveryDetails.Line2,
                State = deliveryDetails.State,
                Post_code = deliveryDetails.PostalCode,
            };
            
            return Ok(details);
        }

        private class Details
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string City { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string State { get; set; }
            public string Post_code { get; set; }
        }
    }
}