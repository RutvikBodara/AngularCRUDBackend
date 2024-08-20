using BAL.Contacts.Interface;
using ContactsWebApi.Controllers.Auth;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace ContactsWebApi.Controllers
{
    [ApiController]
    [CustomAuth()]
    public class AccountController : Controller
    {
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
        private readonly BAL.Contacts.Interface.IBAL_Category_CRUD _IBAL_Category_CRUD;
        private readonly IConfiguration _Configuration;
        public AccountController(BAL.Contacts.Interface.ICommonMethods iCommonMethods, IConfiguration configuration, IBAL_Category_CRUD bAL_Category_CRUD)
        {
            _ICommonMethods = iCommonMethods;
            _Configuration = configuration;
            _IBAL_Category_CRUD = bAL_Category_CRUD;
        }

        [HttpGet]
        [Route("~/myprofile")]
        public async Task<DAL_Standard_Response<Account>> myProfile(int id)
        {
            //error codes 
            DAL_Standard_Response<Account> responseAPI = new DAL_Standard_Response<Account>();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Contact_CRUD");
            }
            catch (Exception ex)
            {
                responseAPI.code = 110;
                responseAPI.message = "Error in ErrorCode Fetch";
                //responseAPI.responseData = null;
                return responseAPI;
            }

            try
            {
                Account? account = await _ICommonMethods.GetAccountDetailsById(id);
                if(account == null)
                {
                    var errorCodeValues = errorList.FirstOrDefault(x => x.errorCode == 103);
                    responseAPI.code = errorCodeValues.errorCode;
                    responseAPI.message = errorCodeValues.message;
                    //responseAPI.responseData = CategoryDetails;
                    return responseAPI;
                }
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.responseData = account;
                return responseAPI;
            }
            catch (Exception ex)
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 101);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.responseData = null;
                return responseAPI;
            }

        }

        [HttpPatch]
        [Route("~/updateprofile")]
        public async Task<DAL_ResponseWithOutBody> updateProfile(UpdateProfileViewModel data)
        {
            DAL_ResponseWithOutBody responseAPI = new DAL_ResponseWithOutBody();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Contact_CRUD");
            }
            catch (Exception ex)
            {
                responseAPI.code = 110;
                responseAPI.message = "Error in ErrorCode Fetch";
                //responseAPI.responseData = null;
                return responseAPI;
            }

            try
            {
                bool status = await _ICommonMethods.updateProfile(data);
                if (!status)
                {
                    var errorCodeValues = errorList.FirstOrDefault(x => x.errorCode == 102);
                    responseAPI.code = errorCodeValues.errorCode;
                    responseAPI.message = errorCodeValues.message;
                    //responseAPI.responseData = CategoryDetails;
                    return responseAPI;
                }
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
            catch (Exception ex)
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 101);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
        }
    }
}
