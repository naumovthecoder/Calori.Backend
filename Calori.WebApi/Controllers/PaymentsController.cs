using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Auth.Commands.Register;
using Calori.Application.Payment;
using Calori.Domain.Models.Auth;
using Calori.WebApi.Configuration;
using Calori.WebApi.Models.Stripe;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Calori.WebApi.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IOptions<StripeOptions> options;
        private readonly IStripeClient client;
        private readonly UserManager<ApplicationUser> _userManager;
        private IMediator _mediator;
        private readonly IMapper _mapper;
        

        public PaymentsController(IOptions<StripeOptions> options,
            UserManager<ApplicationUser> userManager, IMapper mapper, IMediator mediator)
        {
            this.options = options;
            _userManager = userManager;
            _mediator = mediator;
            _mapper = mapper;
            this.client = new StripeClient(this.options.Value.SecretKey);
        }

        [HttpGet("config")]
        public ConfigResponse Setup()
        {
            return new ConfigResponse
            {
                ProPrice = this.options.Value.ProPrice,
                BasicPrice = this.options.Value.BasicPrice,
                PublishableKey = this.options.Value.PublishableKey,
            };
        }

        private class CreateCheckoutSessionResponse
        {
            public string SessionUrl { get; set; }
            public string SessionId { get; set; } 
        }

        [Authorize]
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var referringUrl = Request.Headers["Host"].ToString();

            ApplicationUser appUser;
            if (User != null)
            {
                appUser = await _userManager.FindByEmailAsync(User!.Identity!.Name);
                if (appUser != null)
                {
                    #region SessionCreating

                    var options = new SessionCreateOptions
                    {
                        // SuccessUrl = $"{this.options.Value.Domain}/success.html?session_id={{CHECKOUT_SESSION_ID}}",
                        // CancelUrl = $"{this.options.Value.Domain}/canceled.html",
                        SuccessUrl = "https://calori-c69d2.web.app/profile?payment_result=success",
                        CancelUrl = "https://calori-c69d2.web.app/profile?payment_result=fail",
                        PhoneNumberCollection = new SessionPhoneNumberCollectionOptions { Enabled = true },
                        CustomerEmail = appUser.Email,
                        ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                        {
                            AllowedCountries = new List<string> { "FI" }
                        },
                        Mode = "subscription",
                        LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions
                            {
                                Price = Request.Form["priceId"],
                                Quantity = 1,
                            },
                        },
                        // AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                    };
                    var service = new SessionService(this.client);
                    try
                    {
                        var session = await service.CreateAsync(options); // async
                        Response.Headers.Add("Location", session.Url);
                        // return new StatusCodeResult(303);
                        var result = new CreateCheckoutSessionResponse
                        {
                            SessionUrl = session.Url
                        };
                        
                        ApplicationUser applicationUser;
                        if (User != null)
                        {
                            applicationUser = await _userManager.FindByEmailAsync(User!.Identity!.Name);
                            
                            if (applicationUser != null)
                            {
                                result.SessionId = session.Id;
                                var payCommand = new PaymentCommand
                                {
                                    SessionId = session.Id,
                                    UserId = applicationUser.Id
                                };
                                
                                var command = _mapper.Map<PaymentCommand>(payCommand);
                                var response = await _mediator.Send(command);

                                if (response == null || !string.IsNullOrEmpty(response.Message))
                                {
                                    return BadRequest();
                                }
                            }
                        }
                         
                        return Ok(result);
                    }
                    catch (StripeException e)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            ErrorMessage = new ErrorMessage
                            {
                                Message = e.StripeError.Message,
                            }
                        });
                    }

                    #endregion
                }
            }
            return Unauthorized();
        }

        [HttpGet("checkout-session")]
        public async Task<IActionResult> CheckoutSession(string sessionId)
        {
            var service = new SessionService(this.client);
            var session = await service.GetAsync(sessionId);
            return Ok(session);
        }

        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal(string sessionId)
        {
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutService = new SessionService(this.client);
            var checkoutSession = await checkoutService.GetAsync(sessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = this.options.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = returnUrl,
            };
            var service = new Stripe.BillingPortal.SessionService(this.client);
            var session = await service.CreateAsync(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
