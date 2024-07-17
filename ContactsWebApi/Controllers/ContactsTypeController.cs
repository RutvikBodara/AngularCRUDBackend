using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.ContactType;
using Microsoft.AspNetCore.Mvc;

namespace ContactsWebApi.Controllers
{
    [Route("/contacts/[controller]")]
    public class ContactsTypeController : Controller
    {
        private readonly BAL.Contacts.Interface.IBAL_Contacts_CRUD _IBAL_Contacts_CRUD;
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
        private readonly BAL.Contacts.Interface.IBAL_Contacts_Type_CRUD _IBAL_Contacts_Type_TypeCRUD;
        private readonly IConfiguration _Configuration;

        public ContactsTypeController(IConfiguration configuration,BAL.Contacts.Interface.IBAL_Contacts_CRUD IBAL_Contacts_CRUD, BAL.Contacts.Interface.ICommonMethods iCommonMethods, BAL.Contacts.Interface.IBAL_Contacts_Type_CRUD bAL_Contacts_Type_CRUDRepo)
        {
            _IBAL_Contacts_CRUD = IBAL_Contacts_CRUD;
            _ICommonMethods = iCommonMethods;
            _IBAL_Contacts_Type_TypeCRUD = bAL_Contacts_Type_CRUDRepo;
            _Configuration = configuration;
        }

        [HttpGet]
        [Route("~/contacts/getcontactstype")]
        public async Task<DAL_Standard_Response<IEnumerable<DAL_ContactTypeViewModel>>> getcontactsType(int? id, string? name, string? typeList)
        {

            DAL_Standard_Response<IEnumerable<DAL_ContactTypeViewModel>> responseAPI = new DAL_Standard_Response<IEnumerable<DAL_ContactTypeViewModel>>();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Contact_Type_CRUD");
            }
            catch (Exception ex)
            {
                responseAPI.code = 110;
                responseAPI.message = "Error in ErrorCode Fetch";
                responseAPI.responseData = null;
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
                IQueryable<DAL_ContactTypeViewModel> contacts = await _IBAL_Contacts_Type_TypeCRUD.get<DAL_ContactTypeViewModel>(name, id, typeList);
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.responseData = contacts;
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
        [Route("~/contacts/updatecontactstype")]
        public async Task<DAL_ResponseWithOutBody> EditContactsType(ContactType requestData)
        {
            DAL_ResponseWithOutBody responseAPI = new DAL_ResponseWithOutBody();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Contact_Type_CRUD");
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
                if (string.IsNullOrEmpty(requestData.Name))
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                bool editStatus = await _IBAL_Contacts_Type_TypeCRUD.update<ContactType>(requestData);
                if (editStatus)
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
        [Route("~/contacts/addcontactstype")]
        public async Task<DAL_ResponseWithOutBody> AddContactsType(DAL.Contacts.ViewModels.Contacts.contactsDetailViewModel<string> requestData)
        {
            DAL_ResponseWithOutBody responseAPI = new DAL_ResponseWithOutBody();
            IEnumerable<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel> errorList = new List<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>();
            try
            {
                errorList = await _ICommonMethods.AddErrorCode<DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel>("Contact_Type_CRUD");
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
                if (string.IsNullOrEmpty(requestData.name))
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }

                await _IBAL_Contacts_Type_TypeCRUD.add(requestData);
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
        [Route("~/contacts/deletecontactstype")]
        public async Task<DAL_ResponseWithOutBody> DeleteContactsType(int id)
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
                bool deleteStatus = await _IBAL_Contacts_Type_TypeCRUD.delete<string>(id);
                if (deleteStatus)
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
