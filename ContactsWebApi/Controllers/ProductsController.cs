using BAL.Contacts.Interface;
using ContactsWebApi.Controllers.Auth;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ContactsWebApi.Controllers
{
    [ApiController]
    [CustomAuth()]
    public class ProductsController : Controller
    {
        
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
        private readonly BAL.Contacts.Interface.IBAL_Products_CRUD _IBAL_Products_CRUD;
        private readonly IConfiguration _Configuration;
        public ProductsController(BAL.Contacts.Interface.ICommonMethods iCommonMethods, IConfiguration configuration, IBAL_Products_CRUD bAL_Products_CRUD)
        {
            _ICommonMethods = iCommonMethods;
            _Configuration = configuration;
            _IBAL_Products_CRUD = bAL_Products_CRUD;
        }

        [HttpGet]
        [Route("~/product/getproducts")]
        public async Task<DAL_Standard_Response<IEnumerable<productDetailsViewModel>>> GetProducts(string? commonsearch, int? pagenumber, int? pagesize,string? sortedcolumn,string? sorteddirection, bool? download)
        {
            DAL_Standard_Response<IEnumerable<productDetailsViewModel>> responseAPI = new DAL_Standard_Response<IEnumerable<productDetailsViewModel>>();
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
                patentProductDetailsViewModel productDetails =await _IBAL_Products_CRUD.get<productDetailsViewModel>(commonsearch, pagenumber, pagesize, sortedcolumn, sorteddirection,download);

                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.pageNumber=productDetails.pageNumber;
                responseAPI.dataCount = productDetails.dataCount;
                responseAPI.pageSize=productDetails.pageSize;
                responseAPI.maxPage =productDetails.maxPage;
                responseAPI.columnCredits=productDetails.columnCredits;
                responseAPI.responseData =productDetails.ProductDetails;
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

        [HttpGet]
        [Route("~/product/getproductsbase64")]
        public async Task<DAL_Standard_Response<string>> GetProductsBase64(bool? download,int? doctype)
        {
            DAL_Standard_Response<string> responseAPI = new DAL_Standard_Response<string>();
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
                patentProductDetailsViewModel productDetails = await _IBAL_Products_CRUD.get<productDetailsViewModel>(null, null, null, null, null, download);

                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                //responseAPI.pageNumber = productDetails.pageNumber;
                //responseAPI.dataCount = productDetails.dataCount;
                //responseAPI.pageSize = productDetails.pageSize;
                //responseAPI.maxPage = productDetails.maxPage;
                //responseAPI.columnCredits = productDetails.columnCredits
                responseAPI.responseData = await _ICommonMethods.DTOToBase64(productDetails.ProductDetails,productDetails.columnCredits,doctype);
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
        [Route("~/product/updateproduct")]
        public async Task<DAL_ResponseWithOutBody> EditContactsType(editProductViewModel requestData)
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
                //validations
                if (string.IsNullOrEmpty(requestData.name) || string.IsNullOrEmpty(requestData.description) || requestData.categoryId == 0)
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                bool editStatus = await _IBAL_Products_CRUD.update<editProductViewModel>(requestData);
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
                responseAPI.message = errorCodeValue.message + "( " + ex.ToString() + " )";
                return responseAPI;
            }
        }

        [HttpPost]
        [Route("~/product/addproduct")]
        public async Task<DAL_ResponseWithOutBody> AddProduct([FromForm] IFormCollection requestData)
        {


            var name = requestData["name"];
            var description = requestData["description"];
            var helplineNumber = requestData["helplineNumber"];
            var launchDate = requestData["launchDate"];
            var categoryId = requestData["categoryId"];
            var file = requestData.Files.GetFile("file");
            var availableForSale = requestData["availableForSale"];
            var lastDateProduct = requestData["lastDate"];
            double price = Convert.ToDouble(requestData["price"]);
            var Countries = new List<int>();
            //if (file != null && file.Length > 0)
            //{
            //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await file.CopyToAsync(stream);
            //    }
            //}

            int index = 0;
            while (requestData.ContainsKey($"countries[{index}].id"))
            {
                Countries.Add(int.Parse(requestData[$"countries[{index}].id"]));
                index++;
            }

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
                //validations
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(helplineNumber) || string.IsNullOrEmpty(launchDate) || string.IsNullOrEmpty(categoryId) || file == null )
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }

                bool checkDublicate = await _IBAL_Products_CRUD.add(name,description,helplineNumber,launchDate, lastDateProduct,categoryId, file, availableForSale, Countries,price);
                if (!checkDublicate)
                {
                    errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 107);
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }

                //if(launchDate > lastDateProduct)
                //{

                //}
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
        [Route("~/product/deleteproduct")]
        public async Task<DAL_ResponseWithOutBody> DeleteProudct(int id)
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
                bool deleteStatus = await _IBAL_Products_CRUD.delete<string>(id);
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

        [HttpDelete]
        [Route("~/product/deletebulkproduct")]
        public async Task<DAL_ResponseWithOutBody> DeleteBulkProudct(string idlist)
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
                if (idlist == null)
                {
                    responseAPI.code = errorCodeValue.errorCode;
                    responseAPI.message = errorCodeValue.message;
                    return responseAPI;
                }
                bool deleteStatus = await _IBAL_Products_CRUD.deleteBulk(idlist);
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

        [HttpGet]
        [Route("~/product/getproductbycategory")]
        public async Task<DAL_Standard_Response<IQueryable<productDetailsViewModel>>> GetProductsByCategory(int id)
        {
            DAL_Standard_Response<IQueryable<productDetailsViewModel>> responseAPI = new DAL_Standard_Response<IQueryable<productDetailsViewModel>>();
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
                patentProductDetailsViewModel productDetails = await _IBAL_Products_CRUD.getById(id);
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;           
                responseAPI.responseData = productDetails.ProductDetails;
                responseAPI.columnCredits = productDetails.columnCredits;
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
    }
}
