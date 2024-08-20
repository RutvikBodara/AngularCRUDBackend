using BAL.Contacts.Interface;
using ContactsWebApi.Controllers.Auth;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace ContactsWebApi.Controllers
{
    [ApiController]
    [CustomAuth()]
    public class CategoryController : Controller
    {
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
        private readonly BAL.Contacts.Interface.IBAL_Category_CRUD _IBAL_Category_CRUD;
        private readonly IConfiguration _Configuration;
        public CategoryController(BAL.Contacts.Interface.ICommonMethods iCommonMethods, IConfiguration configuration, IBAL_Category_CRUD bAL_Category_CRUD)
        {
            _ICommonMethods = iCommonMethods;
            _Configuration = configuration;
            _IBAL_Category_CRUD = bAL_Category_CRUD;
        }

        [HttpGet]
        [Route("~/category/getcategory")]
        public async Task<DAL_Standard_Response<IEnumerable<categoryDetailViewModel>>> GetCategory(string? commonsearch, int? pagenumber, int? pagesize, string? sortedcolumn, string? sorteddirection)
        {
            DAL_Standard_Response<IEnumerable<categoryDetailViewModel>> responseAPI = new DAL_Standard_Response<IEnumerable<categoryDetailViewModel>>();
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

            //var apiKey = HttpContext.Request.Headers["API-KEY"].FirstOrDefault();
            //if (apiKey != _Configuration["APIKey"])
            //{
            //    var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 106);
            //    responseAPI.code = errorCodeValue.errorCode;
            //    responseAPI.message = errorCodeValue.message;
            //    return responseAPI;
            //}
            try
            {

                patentCategoryDetailsViewModel CategoryDetails = await _IBAL_Category_CRUD.get<categoryDetailViewModel>(commonsearch, pagenumber, pagesize, sortedcolumn, sorteddirection);
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.pageNumber = CategoryDetails.pageNumber;
                responseAPI.dataCount = CategoryDetails.dataCount;
                responseAPI.columnCredits=CategoryDetails.columnCredits;
                responseAPI.pageSize = CategoryDetails.pageSize;
                responseAPI.maxPage = CategoryDetails.maxPage;
                responseAPI.responseData = CategoryDetails.CategoryDetails;
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

        [HttpPost]
        [Route("~/category/addcategory")]
        public async Task<DAL_ResponseWithOutBody> AddCategory(categoryModel requestData)
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
            ////var apiKey = HttpContext.Request.Headers["API-KEY"].FirstOrDefault();
            ////if (apiKey != _Configuration["APIKey"])
            ////{
            ////    var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 106);
            ////    responseAPI.code = errorCodeValue.errorCode;
            ////    responseAPI.message = errorCodeValue.message;
            ////    return responseAPI;
            ////}

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

                bool checkDublicate = await _IBAL_Category_CRUD.add(requestData);
                if (!checkDublicate)
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

        [HttpPatch]
        [Route("~/category/updatecategory")]
        public async Task<DAL_ResponseWithOutBody> EditContacts(categoryDetailViewModel requestData)
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
            ////var apiKey = HttpContext.Request.Headers["API-KEY"].FirstOrDefault();
            ////if (apiKey != _Configuration["APIKey"])
            ////{
            ////    var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 106);
            ////    responseAPI.code = errorCodeValue.errorCode;
            ////    responseAPI.message = errorCodeValue.message;
            ////    return responseAPI;
            ////}

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
                bool editStatus = await _IBAL_Category_CRUD.update<categoryDetailViewModel>(requestData);
                if (editStatus)
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
        [HttpDelete]
        [Route("~/category/deletecategory")]
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

            //var apiKey = HttpContext.Request.Headers["API-KEY"].FirstOrDefault();
            //if (apiKey != _Configuration["APIKey"])
            //{
            //    var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 106);
            //    responseAPI.code = errorCodeValue.errorCode;
            //    responseAPI.message = errorCodeValue.message;
            //    return responseAPI;
            //}

            try
            {
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 102);
                if (id == 0)
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                bool deleteStatus = await _IBAL_Category_CRUD.delete<string>(id);
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
