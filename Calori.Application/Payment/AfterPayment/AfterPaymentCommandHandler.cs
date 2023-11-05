using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
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

        public AfterPaymentCommandHandler(ICaloriDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _dbContext = context;
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

                var user = await _userManager.FindByIdAsync(userPayment.UserId);

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
    }
}