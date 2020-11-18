using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Business.Providers
{
    public interface IJwtTokenProvider
    {
        string GenerateAccessToken(User user);
    }

    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly string _appDomain = ConfigurationManager.AppSettings["appDomain"];
        private readonly string _jwtSecret = ConfigurationManager.AppSettings["jwtSecret"];

        public string GenerateAccessToken(User user)
        {
            var jwtSecurityToken = new JwtSecurityToken
            (
                issuer: _appDomain,
                audience: _appDomain,
                claims: CreateClaims(user),
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                signingCredentials: _jwtSecret.ToIdentitySigningCredentials()
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }


        private IEnumerable<Claim> CreateClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.Phone),
                new Claim(ClaimTypes.Email, user.Email),
            };
            claims.AddRange(user.Roles.ToList().Select(role => new Claim(ClaimTypes.Role, role.Name)));
            return claims;
        }
    }
}