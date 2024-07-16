using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Interface
{
    public interface IBAL_Contacts_Type_CRUD
    {
        Task<IEnumerable<T>> get<T>(string? name, int? id,string? typeList);
        //Task<T> add<T>(DAL.Contacts.ViewModels.Contacts.contactsDetailViewModel<T> requestData);
        Task add(contactsDetailViewModel<string> requestData);
        Task<bool> update<T>(ContactType contacts);
        Task<bool> delete<T>(int id);
        Task<bool> exists<T>(int id) where T : class;
    }
}
