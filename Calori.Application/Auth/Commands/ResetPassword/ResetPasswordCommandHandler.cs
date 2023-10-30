using System;
using System.Threading;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Calori.Application.Services.UserService;
using Calori.Domain.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Calori.Application.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler
        : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager; 

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager, 
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new ResetPasswordResponse();
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                result.Message = $"account with the email address {request.Email} does not exist.";
                return result;
            }
            
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var password = new PasswordGenerator().GeneratePassword();

            await _userManager.ResetPasswordAsync(user, resetToken, password);
            
            string subject = "Reset Password | CaloriApp";
            string body = $"Your login: {request.Email}\nYour new password: {password}";
            
            Console.WriteLine(body);
            
            await _emailService.SendEmailAsync(user.Email, subject, body);
            result.Message = "A new password has been sent to the Email you specified.";

            return result;
        }
    }
}