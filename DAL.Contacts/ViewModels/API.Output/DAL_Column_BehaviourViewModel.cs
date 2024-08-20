using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.API.Output
{
    public class DAL_Column_BehaviourViewModel
    {
        public string columnName { get; set; }

        public bool isEditable { get; set; }
    }
}
