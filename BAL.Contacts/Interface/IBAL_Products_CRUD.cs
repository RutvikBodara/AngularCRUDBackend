using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Contacts;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BAL.Contacts.Interface
{
    public interface IBAL_Products_CRUD
    {
        Task<patentProductDetailsViewModel> get<T>(string? commonsearch, int? pagenumber, int? pagesize, string? sortedcolumn, string? sorteddirection);
        Task<patentProductDetailsViewModel> getById(int id);
        Task<bool> add(string name, string description, string helplineNumber, string launchDate,string lastDateProduct, string categoryId, IFormFile file,string availableForSale,List<int> Countries, double price);
        Task<bool> update<T>(editProductViewModel contacts);
        Task<bool> delete<T>(int id);
        Task<bool> exists<T>(int id) where T : class;
    }
}
