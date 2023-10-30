using System;
using System.Text;

namespace Calori.Application.Services.UserService
{
    public class PasswordGenerator
    {
        public string GeneratePassword()
        {
            Random random = new Random();
            string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
            StringBuilder password = new StringBuilder();
            password.Append(validChars[random.Next(26)]);
            password.Append(validChars[random.Next(26, 52)]);
            password.Append(validChars[random.Next(52, 62)]);
            password.Append(validChars[random.Next(62, 74)]);
            for (int i = 0; i < 6; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }
            for (int i = 0; i < password.Length; i++)
            {
                int swapIndex = random.Next(password.Length);
                (password[i], password[swapIndex]) = (password[swapIndex], password[i]);
            }

            return password.ToString();
        }
    }
}