using BAL.Contacts.Repository;
using ContactsWebApi.Controllers.Auth;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Contacts;
using DAL.Contacts.ViewModels.ContactType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using System.Runtime.CompilerServices;

namespace ContactsWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [CustomAuth()]
    //[ContactsWebApi.Controllers.Auth.CustomAuth(1)]
    public class ContactsController : ControllerBase
    {
        private readonly BAL.Contacts.Interface.IBAL_Contacts_CRUD _IBAL_Contacts_CRUD;
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
        private readonly BAL.Contacts.Interface.IBAL_Contacts_Type_CRUD _IBAL_Contacts_Type_TypeCRUD;
        private readonly IConfiguration _Configuration;
        public ContactsController(BAL.Contacts.Interface.IBAL_Contacts_CRUD IBAL_Contacts_CRUD, BAL.Contacts.Interface.ICommonMethods iCommonMethods,BAL.Contacts.Interface.IBAL_Contacts_Type_CRUD bAL_Contacts_Type_CRUDRepo,IConfiguration configuration )
        {
            _IBAL_Contacts_CRUD = IBAL_Contacts_CRUD;
            _ICommonMethods = iCommonMethods;
            _IBAL_Contacts_Type_TypeCRUD = bAL_Contacts_Type_CRUDRepo;
            _Configuration = configuration;
        }

        [HttpGet]
        [Route("~/contacts/getcontacts")]
        public async Task<DAL_Standard_Response<IEnumerable<Contact>>> GetContacts(int? pagenumber, int? pagesize, string? sortedcolumn, string? sorteddirection, string? name = null, string? surname = null, int? id = null, string? typeList = null)
        {
            DAL_Standard_Response<IEnumerable<Contact>> responseAPI = new DAL_Standard_Response<IEnumerable<Contact>>();
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
                patentContactsDetailsViewModel contacts = await _IBAL_Contacts_CRUD.get<Contact>(name, surname, id, typeList,pagenumber,pagesize,sortedcolumn,sorteddirection);
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.responseData = contacts.contactDetails;
                responseAPI.pageNumber = contacts.pageNumber;
                responseAPI.dataCount = contacts.dataCount;
                responseAPI.pageSize = contacts.pageSize;
                responseAPI.maxPage = contacts.maxPage;
                responseAPI.columnCredits = contacts.columnCredits;
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
        [Route("~/contacts/updatecontacts")]
        public async Task<DAL_ResponseWithOutBody> EditContacts(Contact requestData)
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
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 102);
                //validations
                if (string.IsNullOrEmpty(requestData.Surname) || string.IsNullOrEmpty(requestData.Name))
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                bool editStatus = await _IBAL_Contacts_CRUD.update<Contact>(requestData);
                if(editStatus)
                {
                    errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                else
                {
                    errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 107);
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
            }
            catch (Exception ex)
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 101);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
        }

        [HttpPost]
        //[Route("/Add")]Addcontacts
        [Route("~/contacts/addcontacts")]
        public async Task<DAL_ResponseWithOutBody> AddContacts(DAL.Contacts.ViewModels.Contacts.contactsDetailViewModel<string> requestData)
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
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 102);
                //validations
                if (string.IsNullOrEmpty(requestData.name) || string.IsNullOrEmpty(requestData.surname))
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }

                bool dublicateCheck = await _IBAL_Contacts_CRUD.add(requestData);
                if(!dublicateCheck)
                {
                    errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 107);
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
            catch (Exception e)
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 101);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
        }

        [HttpDelete]
        [Route("~/contacts/deletecontacts")]
        public async Task<DAL_ResponseWithOutBody> DeleteContacts(int id)
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
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 102);
                if (id == 0)
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                bool deleteStatus = await _IBAL_Contacts_CRUD.delete<string>(id);
                if(deleteStatus)
                {
                    errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                else
                {
                    errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 105);
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
            }
            catch (Exception e)
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 101);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                return responseAPI;
            }
        }
    }
}
