using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Calori.Application.Interfaces;
using Calori.Domain.Models;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
using Calori.Domain.Models.Enums;
using Calori.Domain.Models.Payment;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.Payment.AfterPayment
{
    public class AfterPaymentCommandHandler :
        IRequestHandler<AfterPaymentCommand, AfterPaymentResponse>
    {
        private readonly ICaloriDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AfterPaymentCommandHandler(ICaloriDbContext context, 
            UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _dbContext = context;
            _emailService = emailService;
        }

        public async Task<AfterPaymentResponse> Handle(AfterPaymentCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new Exception(nameof(AfterPaymentCommand));
            }

            try
            {
                var userPayment = await _dbContext.UserPayments
                    .FirstOrDefaultAsync(p =>
                        p.SessionId.ToLower() == request.SessionId.ToLower(), cancellationToken);

                userPayment.IsPaid = true;
                userPayment.Status = PaymentStatus.Successful;
                userPayment.UpdatedAt = DateTime.UtcNow;
                if (request.AmountTotal != null) userPayment.Cost = (double)request.AmountTotal;

                // var caloriStripeUser = new CaloriStripeCustomer();
                // caloriStripeUser.CaloriUserId = userPayment.UserId;
                //
                // await _dbContext.CaloriStripeCustomers.AddAsync(caloriStripeUser);

                var user = await _userManager.FindByIdAsync(userPayment.UserId);
                
                var shipData = await _dbContext.CaloriShippingData
                    .FirstOrDefaultAsync(d => 
                        d.UserId == user.Id, cancellationToken);

                await UpdatePersonalPlanStatus(cancellationToken, user);

                if (shipData != null)
                {
                    return new AfterPaymentResponse
                    {
                        ShippingData = shipData,
                        Message = ""
                    };
                }

                var shippingData = new CaloriShippingData();
                shippingData.UserId = user.Id;
                shippingData.UserEmail = request.UserEmail;
                shippingData.Name = request.Name;
                shippingData.UserPhone = request.UserPhone;
                shippingData.City = request.City;
                shippingData.Line1 = request.Line1;
                shippingData.Line2 = request.Line2;
                shippingData.PostalCode = request.PostalCode;
                shippingData.Country = request.Country;
                shippingData.State = request.State;

                await _dbContext.CaloriShippingData.AddAsync(shippingData, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                var message = $"Hi, {request.Name} \ud83d\udc4b\n\n" +
                              $"Thank you for making your subscription with Calori. " +
                              $"We’re thrilled to support you on your journey towards a healthier life." +
                              $"\n\nHere are our next steps together:\n\n1\ufe0f\u20e3 " +
                              $"You’ll get a call from us on the next work day. " +
                              $"We’d love to get to know you better and fully understand your needs and goals.\n\n2\ufe0f\u20e3 " +
                              $"You’ll start receiving your meals from 8.1.2024 - just in time to rock your New Year’s resolutions. \n\n" +
                              $"Kind regards,\nCalori\n";

                await _emailService.SendEmailAsync(request.UserEmail,
                    $"Hi, from Calori \ud83d\udc4b", message);

            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException(nameof(CaloriShippingData));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return new AfterPaymentResponse();
        }

        private async Task UpdatePersonalPlanStatus(CancellationToken cancellationToken, ApplicationUser user)
        {
            var application = await _dbContext.CaloriApplications
                .FirstOrDefaultAsync(a =>
                    a.UserId.ToLower() == user.Id.ToLower(), cancellationToken);

            var personalPlan = await _dbContext.PersonalSlimmingPlan
                .FirstOrDefaultAsync(p =>
                    p.Id == application.PersonalSlimmingPlanId, cancellationToken);

            personalPlan.SubscriptionStatus = SubscriptionStatus.Active;
        }
    }
}