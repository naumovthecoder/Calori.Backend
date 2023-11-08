using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Interfaces;
using Calori.Application.Payment.AfterPayment;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Calori.WebApi.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICaloriDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        const string endpointSecret = "whsec_dei5OBJXfzAMCod9gO0QCpVlR6hpGWDy";

        public WebhookController(IMapper mapper, ICaloriDbContext context, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _dbContext = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                
                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    Console.WriteLine(stripeEvent.Type);
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        var customerDetails = session.CustomerDetails;
                        
                        var userAddress = customerDetails.Address;

                        var afterPaymentCommand = new AfterPaymentCommand();
                        
                        afterPaymentCommand.SessionId = session.Id;
                        afterPaymentCommand.UserEmail = customerDetails.Email;
                        afterPaymentCommand.Name = customerDetails.Name;
                        afterPaymentCommand.UserPhone = customerDetails.Phone;
                        afterPaymentCommand.City = userAddress.City;
                        afterPaymentCommand.Line1 = userAddress.Line1;
                        afterPaymentCommand.Line2 = userAddress.Line2;
                        afterPaymentCommand.PostalCode = userAddress.PostalCode;
                        afterPaymentCommand.Country = userAddress.Country;
                        afterPaymentCommand.State = userAddress.State;
                        afterPaymentCommand.Status = Events.CheckoutSessionCompleted;
                        afterPaymentCommand.AmountTotal = session.AmountTotal;
                        
                        var command = _mapper.Map<AfterPaymentCommand>(afterPaymentCommand);
                        var result = await Mediator.Send(command);
                    }
                }

                if (stripeEvent.Type == Events.CustomerSubscriptionPaused)
                {
                    var session = stripeEvent.Data.Object as Session;
                    try
                    {
                        if (session != null)
                        {
                            var customerDetails = session.CustomerDetails;
                            var subscriptionId = session.Subscription.Id;
                            var service = new SubscriptionService();
                            var response = await service.CancelAsync(subscriptionId);
                        
                            var user = await _userManager.FindByEmailAsync(customerDetails.Email);

                            await UpdatePersonalPlanStatus(cancellationToken, user, SubscriptionStatus.Paused);
                        }

                        return Ok("Subscription canceled successfully.");
                    }
                    catch (StripeException ex)
                    {
                        // Handle the error, e.g., return an error message
                        return BadRequest($"Subscription cancellation failed: {ex.Message}");
                    }
                }
                
                if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var customerDetails = session.CustomerDetails;
                    var subscriptionId = session.Subscription.Id;
                    var service = new SubscriptionService();
                    var response = await service.CancelAsync(subscriptionId);
                        
                    var user = await _userManager.FindByEmailAsync(customerDetails.Email);

                    await UpdatePersonalPlanStatus(cancellationToken, user, SubscriptionStatus.Canceled);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
        
        private async Task UpdatePersonalPlanStatus(CancellationToken cancellationToken, 
            ApplicationUser user, SubscriptionStatus status)
        {
            var application = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(a =>
                    a.UserId.ToLower() == user.Id.ToLower(), cancellationToken);

            var personalPlan = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(p =>
                    p.Id == application.PersonalSlimmingPlanId, cancellationToken);

            if (status == SubscriptionStatus.Paused)
            {
                personalPlan.SubscriptionStatus = SubscriptionStatus.Paused;
                personalPlan.PausedAt = DateTime.UtcNow;
            }

            if (status == SubscriptionStatus.Canceled)
            {
                personalPlan.SubscriptionStatus = SubscriptionStatus.Canceled;
            }
        }
    }
}