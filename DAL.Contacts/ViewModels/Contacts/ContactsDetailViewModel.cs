using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Contacts
{
    public class contactsDetailViewModel<T>
    {
        public T name { get; set; }
        public T surname { get; set; }
    }
}
