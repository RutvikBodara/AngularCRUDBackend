using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using Microsoft.AspNetCore.Mvc;

namespace ContactsWebApi.Controllers
{
    [ApiController]
    public class CommonController : Controller
    {
        private readonly BAL.Contacts.Interface.ICommonMethods _ICommonMethods;
     
        private readonly IConfiguration _Configuration;
        public CommonController(BAL.Contacts.Interface.ICommonMethods iCommonMethods, IConfiguration configuration)
        {
            _ICommonMethods = iCommonMethods;
            _Configuration = configuration;
            
        }


        [HttpGet]
        [Route("~/common/getcountries")]
        public async Task<IEnumerable<Country>> getCountry()
        {
            return await _ICommonMethods.GetCountryList();
        }
    }
}
