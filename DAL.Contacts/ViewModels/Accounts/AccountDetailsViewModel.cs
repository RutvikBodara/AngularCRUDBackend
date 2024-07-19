using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Accounts
{
    public class AccountDetailsViewModel
    {
        public required string UserName {  get; set; }
        public required string Password { get; set; }
    }
}
