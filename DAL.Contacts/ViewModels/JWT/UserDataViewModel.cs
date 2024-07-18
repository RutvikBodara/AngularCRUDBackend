using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.JWT
{
    public class UserDataViewModel
    {
        public int? AccountId { get; set; }
        public int? Role { get; set; }
        public int? Roleid { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
