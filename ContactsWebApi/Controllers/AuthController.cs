using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.JWT;
using Microsoft.AspNetCore.Mvc;

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
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Login_Code");
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
                    var cookieOptions = new CookieOptions
                    {
                        //HttpOnly = true, // Prevents access to the cookie via client-side scripts
                        //Secure = true,   // Ensures the cookie is sent only over HTTPS
                        //SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddDays(1),
                        Path = "/"  // Sets an expiration time for the cookie
                    };

                    Response.Cookies.Append("JwtToken", jwtToken, cookieOptions);
                    var token = Request.Cookies["JwtToken"];
                    //Console.WriteLine(token);
                    //Response.Cookies.Append("JwtToken", jwtToken);
                    LoginResponse loginResponse = new LoginResponse();
                    loginResponse.JWTToken = jwtToken;
                    loginResponse.MobileNumber = AccountData.Mobilenumber;
                    loginResponse.AccountId = AccountData.Id;
                    loginResponse.UserName = AccountData.Username;
                    loginResponse.EmailId = AccountData.Emailid;
                    loginResponse.FirstName = AccountData.Firstname;
                    loginResponse.LastName =AccountData.Lastname;

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

        [HttpPost]
        //[Route("/Add")]Addcontacts
        [Route("~/register")]
        public async Task<DAL_Standard_Response<LoginResponse>> Register(RegisterDetailsViewModel data)
        {
            DAL_Standard_Response<LoginResponse> responseAPI = new DAL_Standard_Response<LoginResponse>();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Login_Code");
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
                bool registerStatus = await _ICommonMethods.register(data);
                if (!registerStatus)
                {
                    var errorCodeValues = errorList.FirstOrDefault(x => x.errorCode == 107);
                    responseAPI.code = errorCodeValues.errorCode;
                    responseAPI.message = errorCodeValues.message;
                    return responseAPI;
                }
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
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
