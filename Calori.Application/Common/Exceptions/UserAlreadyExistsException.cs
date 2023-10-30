using System;

namespace Calori.Application.Common.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string email)
            : base($"User with Email {email} already exists.") { }
    }
}