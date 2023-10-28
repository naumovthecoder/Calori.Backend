using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Calori.Application.Services.UserService
{
    public class ReferralLinkService
    {
        public async Task<string> GenerateReferralLink(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user Id.");

            var userIdBytes = Encoding.UTF8.GetBytes(userId.ToString());

            using (var sha256 = SHA256.Create())
            using (var stream = new MemoryStream(userIdBytes))
            {
                byte[] hashBytes = await sha256.ComputeHashAsync(stream);

                string referralCode = BitConverter
                    .ToString(hashBytes, 0, 4)
                    .Replace("-", "").ToLower();

                string referralLink =
                    $"https://calori.fi/register?ref={referralCode}&user={userId}";

                return referralLink;
            }
        }
    }
}