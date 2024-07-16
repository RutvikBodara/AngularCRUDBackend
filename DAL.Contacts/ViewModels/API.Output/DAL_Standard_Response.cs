using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.API.Output
{
    public class DAL_Standard_Response<T>
    {
        public int code {  get; set; }
        public string message { get; set; }

        public T responseData{ get; set; }
    }
}
