using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IBAR.TradeModel.Business.Utils
{
    public static class SecurityExtensions
    {
        public static SigningCredentials ToIdentitySigningCredentials(this string jwtSecret)
        {
            var symmetricKey = jwtSecret.ToSymmetricSecurityKey();
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            return signingCredentials;
        }

        public static SymmetricSecurityKey ToSymmetricSecurityKey(this string jwtSecret)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        }

        public static int GetVerificationCode()
        {
            return new Random().Next(100_000, 999_999);
        }
    }
}