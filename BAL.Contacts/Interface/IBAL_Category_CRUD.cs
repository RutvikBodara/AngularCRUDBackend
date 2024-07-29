using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Contacts;
using DAL.Contacts.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Interface
{
    public interface IBAL_Category_CRUD
    {
        Task<IQueryable<T>> get<T>(string? commonsearch);
        Task<bool> add(categoryModel requestData);
        Task<bool> update<T>(categoryDetailViewModel contacts);
        Task<bool> delete<T>(int id);
        Task<bool> exists<T>(int id) where T : class;
    }
}
