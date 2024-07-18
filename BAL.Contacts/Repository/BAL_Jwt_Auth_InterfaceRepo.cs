using DAL.Contacts.ViewModels.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Repository
{
    public class BAL_Jwt_Auth_InterfaceRepo
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration Configuration;
        public BAL_Jwt_Auth_InterfaceRepo(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.Configuration = Configuration;
        }
        public string GenerateJWTAuthetication(UserDataViewModel UserData)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, UserData.Email),
                new Claim(ClaimTypes.Role, UserData.Role.ToString()),
                new Claim("AccountId", UserData.AccountId.ToString()),
                new Claim("Roleid" , UserData.Roleid.ToString() ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires =DateTime.UtcNow.AddDays(30);

            var token = new JwtSecurityToken(
                Configuration["Jwt:Issuer"],
                Configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler)
        {
            jwtSecurityTokenHandler = null;

            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                jwtSecurityTokenHandler = (JwtSecurityToken)validatedToken;

                if (jwtSecurityTokenHandler != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public UserDataViewModel AccessData(string token)
        {
            UserDataViewModel model = new();
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            model.AccountId = int.Parse(jwt.Claims.First(x => x.Type == "AccountId").Value);
            model.Email = jwt.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            model.Role = int.Parse(jwt.Claims.First(x => x.Type == ClaimTypes.Role).Value);
            model.Roleid = int.Parse(jwt.Claims.First(y => y.Type == "Roleid").Value);
            return model;
        }
    }
}
