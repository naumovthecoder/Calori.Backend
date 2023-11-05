using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calori.WebApi.Controllers
{
    [Route("api/user-payments")]
    public class UserPaymentsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICaloriDbContext _dbContext;

        public UserPaymentsController(IMapper mapper, UserManager<ApplicationUser> userManager,
            ICaloriDbContext dbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<GetUserPaymentsDto>>> Get()
        {
            var user = await GetUserFromIdentityAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            var userPayments = await GetUserPaymentsAsync(user.Id);

            if (userPayments == null || userPayments.Count == 0)
            {
                return NotFound();
            }

            var result = userPayments.Select(p => new GetUserPaymentsDto
            {
                Date = p.CreatedAt,
                Cost = p.Cost
            }).ToList();

            return Ok(result);
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

        private async Task<List<UserPayment>> GetUserPaymentsAsync(string userId)
        {
            return await _dbContext.UserPayments
                .Where(p => p.UserId.ToLower() == userId.ToLower())
                .ToListAsync();
        }

        public record GetUserPaymentsDto
        {
            public DateTime? Date { get; set; }
            public double Cost { get; set; }
            public string Message { get; set; }
        }

    }
}