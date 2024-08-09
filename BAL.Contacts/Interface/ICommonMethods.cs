using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
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

        Task<IEnumerable<Country>> GetCountryList();

        Task<bool> register(RegisterDetailsViewModel data);
        //Task<bool> dublicateCheck(string tableName, string columnName, string value);
    }
}
