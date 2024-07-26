using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace ContactsWebApi.Controllers
{
    [ApiController]
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
        public async Task<DAL_Standard_Response<IEnumerable<productDetailsViewModel>>> GetProducts()
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
                IEnumerable<productDetailsViewModel> productDetails =await _IBAL_Products_CRUD.get<productDetailsViewModel>();
                var errorCodeValue = errorList.FirstOrDefault(x => x.errorCode == 100);
                responseAPI.code = errorCodeValue.errorCode;
                responseAPI.message = errorCodeValue.message;
                responseAPI.responseData =productDetails;
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

    }
}
