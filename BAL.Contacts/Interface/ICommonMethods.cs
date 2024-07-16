using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Interface
{
    public interface ICommonMethods
    {
       Task<IEnumerable<T>> AddErrorCode<T>(string segment);
        
    }
}
