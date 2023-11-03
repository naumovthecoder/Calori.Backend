using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Calori.Application.Payment.AfterPayment;
using Stripe;
using Stripe.Checkout;

namespace Calori.WebApi.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : BaseController
    {
        private readonly IMapper _mapper;

        const string endpointSecret = "whsec_0fba8cc180177c2f6eb0ddfccfe32430a0b5165f3da65362f4b7615e33b37a70";

        public WebhookController(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        // [HttpPost]
        // public async Task<IActionResult> Index()
        // {
        //     var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //     try
        //     {
        //         var stripeEvent = EventUtility.ConstructEvent(json,
        //             Request.Headers["Stripe-Signature"], endpointSecret);
        //
        //         Console.WriteLine(json);
        //         
        //         //ApplicationUser applicationUser;
        //         // if (User != null)
        //         // {
        //         //     ApplicationUser applicationUser = await _userManager.FindByEmailAsync(User!.Identity!.Name);
        //         //     if (applicationUser != null)
        //         //     {
        //         //         Console.WriteLine(applicationUser.Email);
        //         //     }
        //         //     else
        //         //     {
        //         //         Console.WriteLine("User not found");
        //         //     }
        //         // }
        //         
        //         // Handle the event
        //         if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        //         {
        //         }
        //         // ... handle other event types
        //         else
        //         {
        //             Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
        //         }
        //
        //         return Ok();
        //     }
        //     catch (StripeException e)
        //     {
        //         return BadRequest();
        //     }
        // }
        
        [HttpPost]
        public async Task<IActionResult> Index()
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
                        
                        var command = _mapper.Map<AfterPaymentCommand>(afterPaymentCommand);
                        var result = await Mediator.Send(command);
                    }
                }
                // ... handle other event types
                else
                {
                    //Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}