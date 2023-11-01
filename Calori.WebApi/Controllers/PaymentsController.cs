﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Calori.Domain.Models.Auth;
using Calori.WebApi.Configuration;
using Calori.WebApi.Models.Stripe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Calori.WebApi.Controllers
{
    public class PaymentsController : Controller
    {
        public readonly IOptions<StripeOptions> options;
        private readonly IStripeClient client;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(IOptions<StripeOptions> options,
        UserManager<ApplicationUser> userManager)
        {
            this.options = options;
            _userManager = userManager;
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
            public ApplicationUser User { get; set; }
        }

        [Authorize]
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var b = Request.Form["priceId"];
            
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{this.options.Value.Domain}/success.html?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{this.options.Value.Domain}/canceled.html",
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
                var session = await service.CreateAsync(options);
                Response.Headers.Add("Location", session.Url);
                
                var result = new CreateCheckoutSessionResponse
                {
                    SessionUrl = session.Url
                };

                var user =  await _userManager.FindByEmailAsync(User!.Identity!.Name);

                if (user != null)
                {
                    result.User = user;
                }

                return Ok(result);
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest(new ErrorResponse
                {
                    ErrorMessage = new ErrorMessage
                    {
                        Message = e.StripeError.Message,
                    }
                });
            }
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

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    this.options.Value.WebhookSecret
                );
                Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something failed {e}");
                return BadRequest();
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                Console.WriteLine($"Session ID: {session.Id}");
                // Take some action based on session.
            }

            return Ok();
        }
    }
}
