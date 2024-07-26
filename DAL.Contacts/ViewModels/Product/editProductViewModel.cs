using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Product
{
    public class editProductViewModel
    {
        public int id { get; set; }
        public string name {  get; set; }

        public string description { get; set; }
        public int categoryId { get; set; }
        public string? file { get; set; }
        public string helplineNumber {  get; set; }
        public string lastDate { get; set; }
        public bool availableForSale { get; set; }
        public string launchDate {  get; set; }
        public double price { get; set; }
        public short[] countryServed {  get; set; }

    }
}
