using DAL.Contacts.ViewModels.JWT;
using System.IdentityModel.Tokens.Jwt;

namespace BAL.Contacts.Interface
{
    public interface IBAL_Jwt_Auth_Interface
    {
        string GenerateJWTAuthetication(UserDataViewModel Data);
        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
        UserDataViewModel AccessData(string token);
    }
}
