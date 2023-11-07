using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
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
        public async Task<ActionResult> Get()
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

            if (deliveryDetails == null)
            {
                return NoContent();
            }

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
        
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(UpdateDetails request, CancellationToken cancellationToken)
        {
            var user = await GetUserFromIdentityAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            var personalShippingData = await _dbContext.CaloriShippingData
                .FirstOrDefaultAsync(s => 
                    s.UserId.ToLower() == user.Id.ToLower(), cancellationToken);

            if (personalShippingData == null)
            {
                return NotFound();
            }
            
            if (!string.IsNullOrEmpty(request.Name))
            {
                personalShippingData.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Phone))
            {
                personalShippingData.UserPhone = request.Phone;
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                personalShippingData.UserEmail = request.Email;
            }
            if (!string.IsNullOrEmpty(request.City))
            {
                personalShippingData.City = request.City;
            }
            if (!string.IsNullOrEmpty(request.Line1))
            {
                personalShippingData.Line1 = request.Line1;
            }
            if (!string.IsNullOrEmpty(request.Line2))
            {
                personalShippingData.Line2 = request.Line2;
            }
            if (!string.IsNullOrEmpty(request.State))
            {
                personalShippingData.State = request.State;
            }
            if (!string.IsNullOrEmpty(request.Post_code))
            {
                personalShippingData.PostalCode = request.Post_code;
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(request);
        }

        private async Task<ApplicationUser> GetUserFromIdentityAsync()
        {
            var userEmail = User?.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }

            return await _userManager.FindByEmailAsync(userEmail);
        }
        
        public record UpdateDetails
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

        public class Details
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