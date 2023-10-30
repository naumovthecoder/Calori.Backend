using Calori.Domain.Models.Auth;
using MediatR;

namespace Calori.Application.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
    {
        public string Email { get; set; }
    }
}