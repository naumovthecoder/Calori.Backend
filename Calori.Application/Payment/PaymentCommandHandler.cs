using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper.Configuration;
using Calori.Application.Auth.Commands.Register;
using Calori.Application.Interfaces;
using Calori.Domain.Models.Auth;
using Calori.Domain.Models.CaloriAccount;
using Calori.Domain.Models.Payment;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.Payment
{
    public class PaymentCommandHandler : IRequestHandler<PaymentCommand, PaymentResponse>
    {
        private readonly ICaloriDbContext _dbContext;
        
        public PaymentCommandHandler(ICaloriDbContext context)
        {
            _dbContext = context;
        }

        public async Task<PaymentResponse> Handle(PaymentCommand request, CancellationToken cancellationToken)
        {
            var response = new PaymentResponse();

            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.SessionId))
            {
                response.Message = "Error. No data.";
                return response;
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var userPayment = new UserPayment
                    {
                        UserId = request.UserId,
                        SessionId = request.SessionId
                    };

                    var payment = await _dbContext.UserPayments
                        .FirstOrDefaultAsync(p => p.UserId.ToLower() == request.UserId.ToLower()
                        && p.IsPaid == false, cancellationToken);
                    
                    if (payment != null)
                    {
                        _dbContext.UserPayments.Remove(payment);
                    }

                    await _dbContext.UserPayments.AddAsync(userPayment, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    scope.Complete();
                }
                catch (DbUpdateException e)
                {
                    response.Message = "Database error: " + e.Message;
                }
                catch (Exception e)
                {
                    response.Message = "An error occurred: " + e.Message;
                }
            }

            return response;
        }


    }
}