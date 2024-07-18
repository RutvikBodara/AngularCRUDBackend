using DAL.Contacts.ViewModels.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Interface
{
    public interface IBAL_Jwt_Auth_Interface
    {
        string GenerateJWTAuthetication(UserDataViewModel UserData);
        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
        UserDataViewModel AccessData(string token);
    }
}
