using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Product
{
    public class productDetailsViewModel
    {
        public int id { get; set; }
        public string name { get; set; }

        public string description { get; set; } 
        public int categoryId { get; set; }
        public string categoryName { get; set; }  
        public DateTime createddate { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? image {  get; set; }
        public double? rating { get; set; }
        //public IFormFile? file {  get; set; } 
        public string? launchDate { get; set; }
        public string helplineNumber { get; set; } 
        public string imageName { get; set; }
        public double? price { get; set; }
        public short[]? countryServed { get; set; }
        public string? lastDate { get; set; }
        public bool? availableForSale { get; set; }

    }
}
