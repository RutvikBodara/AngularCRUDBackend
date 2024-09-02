using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using Microsoft.EntityFrameworkCore.Query;
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

        Task<bool> VerifyCredentials(AccountDetailsViewModel data);
        Task<Account?> GetAccountDetails(string username);
        Task<Account?> GetAccountDetailsById(int id);
        Task<bool> updateProfile(UpdateProfileViewModel data);

        Task<IEnumerable<Country>> GetCountryList();

        Task<bool> register(RegisterDetailsViewModel data);

        Task<string> DTOToBase64(IQueryable<productDetailsViewModel> data, List<DAL_Column_BehaviourViewModel> columns, int? doctype);
        Task<string> dataToExcelBase64(IQueryable<productDetailsViewModel> data, List<DAL_Column_BehaviourViewModel> columns);
        //Task<bool> dublicateCheck(string tableName, string columnName, string value);
    }
}
