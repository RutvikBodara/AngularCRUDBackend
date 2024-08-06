using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Contacts
{
    public class patentContactsDetailsViewModel
    {

        public IEnumerable<Contact> contactDetails { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
        public int? dataCount { get; set; }
        public int? maxPage { get; set; }
    }
}
