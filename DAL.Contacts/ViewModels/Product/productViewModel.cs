using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Product
{
    public class productViewModel
    {

        public string name { get; set; }
        public string description { get; set; }
        public int categoryId { get; set; }
        public IFormFile file {  get; set; } 
        public string launchDate { get; set; }
        public string helplineNumber { get; set; }

    }
}
