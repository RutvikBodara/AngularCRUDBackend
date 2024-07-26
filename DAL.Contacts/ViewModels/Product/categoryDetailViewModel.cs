using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Product
{
    public class categoryDetailViewModel
    {
        public int id { get; set; } 
        public string name { get; set; }
        public  DateTime? createdDate { get; set; } = null;
        public int? TotalProducts { get; set; } = null;
    }
}
