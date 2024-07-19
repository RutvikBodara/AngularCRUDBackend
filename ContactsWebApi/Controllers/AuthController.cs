using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.JWT;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ContactsWebApi.Controllers
{
  
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IBAL_Jwt_Auth_Interface _JwtAuth;
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
        public AuthController(IConfiguration configuration, ICommonMethods iCommonMethods, IBAL_Jwt_Auth_Interface jwtAuth)
        {
            _Configuration = configuration;
            _ICommonMethods = iCommonMethods;
            _JwtAuth = jwtAuth;
        }

        [HttpPost]
        //[Route("/Add")]Addcontacts
        [Route("~/login")]
        public async Task<DAL_Standard_Response<LoginResponse>> Login(AccountDetailsViewModel data)
        {
            DAL_Standard_Response<LoginResponse> responseAPI = new DAL_Standard_Response<LoginResponse>();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Login");
            }
            catch (Exception ex)
            {
                responseAPI.code = 110;
                responseAPI.message = "Error in ErrorCode Fetch";
                return responseAPI;
            }
            var apiKey = HttpContext.Request.Headers["API-KEY"].FirstOrDefault();
            if (apiKey != _Configuration["APIKey"])
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 106);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }

            try
            {
                bool status = await _ICommonMethods.VerifyCredentials(data);
                if (status)
                {
                    UserDataViewModel authuser = new();
                    //send role id with the login 
                    Account? AccountData = await _ICommonMethods.GetAccountDetails(data.UserName);
                    if (AccountData == null) 
                    {
                        var errorCodeVal = errorList.FirstOrDefault(x => x.errorCode == 101);
                        responseAPI.code = errorCodeVal.errorCode;
                        responseAPI.message = errorCodeVal.message;
                        return responseAPI;
                    }
                    authuser.AccountId = AccountData.Id;
                    authuser.Roleid = AccountData.Accounttype;
                    authuser.Email = AccountData.Emailid;
                    authuser.Username = AccountData.Username;
                    var jwtToken = _JwtAuth.GenerateJWTAuthetication(authuser);
                    Response.Cookies.Append("Jwt", jwtToken);

                    LoginResponse loginResponse = new LoginResponse();
                    loginResponse.JWTToken = jwtToken;
                    loginResponse.MobileNumber = AccountData.Mobilenumber;
                    loginResponse.AccountId = AccountData.Id;
                    loginResponse.UserName = AccountData.Username;
                    loginResponse.EmailId = AccountData.Emailid;


                    var errorCodeValues = errorList.FirstOrDefault(x=>x.errorCode == 100);
                    responseAPI.code = errorCodeValues.errorCode;
                    responseAPI.message = errorCodeValues.message;
                    responseAPI.responseData =loginResponse;
                    return responseAPI;
                }
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 102);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
            catch (Exception e)
            {
                var errorCodeVal = errorList.FirstOrDefault(x => x.errorCode == 103);
                responseAPI.code = errorCodeVal.errorCode;
                responseAPI.message = errorCodeVal.message;
                return responseAPI;
            }
        }

    }
}
