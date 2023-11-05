using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Calori.Domain.Models.CaloriAccount;
using MediatR;

namespace Calori.Application.Feedback
{
    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, CaloriFeedbackResponse>
    {
        private readonly ICaloriDbContext _dbContext;
        
        public CreateFeedbackCommandHandler(ICaloriDbContext context)
        {
            _dbContext = context;
        }
        
        public async Task<CaloriFeedbackResponse> Handle(CreateFeedbackCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CaloriFeedbackResponse();
            result.Email = request.Email;
            result.Phone = request.Phone;
            result.Message = request.Message;

            var feedback = new CaloriFeedback();
            feedback.Email = request.Email;
            feedback.Phone = request.Phone;
            feedback.Message = request.Message;
                
            try
            {
                await _dbContext.CaloriFeedback.AddAsync(feedback);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
            return result;
        }
    }
}