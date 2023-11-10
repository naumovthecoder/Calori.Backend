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
        private readonly IEmailService _emailService;

        
        public CreateFeedbackCommandHandler(ICaloriDbContext context, IEmailService emailService)
        {
            _dbContext = context;
            _emailService = emailService;
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

            var phoneNumber = "";
            if (!string.IsNullOrEmpty(request.Phone))
            {
                phoneNumber = $"\nPhone Number: {request.Phone}";
            }
            
            var subjectToCustomer = "Assistance Request | Calori";
            var messageToCustomer = 
                                    "\n\nThank you for reaching out to our support team. " +
                                    "We appreciate your inquiry and the opportunity to assist you. " +
                                    "Your satisfaction is our top priority." +
                                    "\n\nWe have received your message through the \"Contact Us\" form, " +
                                    "and we are committed to resolving your issue as quickly as possible. " +
                                    "Our team is currently reviewing the details you provided, and we will " +
                                    "get back to you shortly." +
                                    "\n\nOur support team is dedicated to providing you with the best possible assistance. " +
                                    "Please allow us some time to investigate your inquiry thoroughly. " +
                                    "If we require any additional information, we will reach out to you promptly." +
                                    "\n\nMeanwhile, if you have any further details or thoughts regarding your request, " +
                                    "please feel free to respond to this email.\n\nThank you for choosing Calori. " +
                                    "We value your business and appreciate your patience as we work to address your concerns." +
                                    "\n\nBest regards," +
                                    "\nCalori" +
                                    "\ninfo@calori.fi";

            var subjectToCalori = "New Support Request | Calori";
            var messageToCalori = $"Customer Details:" +
                                  $"\nEmail: {feedback.Email} {phoneNumber} " +
                                  $"\n\nMessage: {request.Message}";
                
            await _emailService.SendEmailAsync(request.Email,
                subjectToCustomer, messageToCustomer);
            
            await _emailService.SendEmailAsync("info@calori.fi",
                subjectToCalori, messageToCalori);
            
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