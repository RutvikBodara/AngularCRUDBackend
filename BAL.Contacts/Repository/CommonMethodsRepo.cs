using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;


namespace BAL.Contacts.Repository
{
    public class CommonMethodsRepo : ICommonMethods
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public CommonMethodsRepo(DAL.Contacts.Data.ConactsDBContext conactsDBContext)
        {
            _db = conactsDBContext;
        }
        public async Task<IEnumerable<T>> AddErrorCode<T>(string segment)
        {
            IEnumerable<T> result = (IEnumerable<T>)(from x1 in _db.ErrorCodes
                                                     where x1.Segment == segment
                                                     select new DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel()
                                                     {
                                                         errorCode = x1.ErrorCode1,
                                                         message = x1.ErrorDescription
                                                     });
            return result;
        }
        public async Task<bool> VerifyCredentials(AccountDetailsViewModel data)
        {
            return _db.Accounts.Any(x => x.Username.ToLower() == data.UserName.ToLower() && x.Password == data.Password);

        }
        public async Task<Account?> GetAccountDetails(string username)
        {
            Account? account = await _db.Accounts.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower() && x.Isdeleted != true);
            return account;
        }
        public async Task<bool> updateProfile(UpdateProfileViewModel data)
        {
            Account? accountDetails =await _db.Accounts.FirstOrDefaultAsync(x => x.Id == data.Id && x.Isdeleted != true);

            if(accountDetails != null)
            {
                if(accountDetails.Username != data.UserName)
                {
                    return false;
                }
                accountDetails.Firstname = data.FirstName;
                accountDetails.Lastname = data.LastName;
                accountDetails.Emailid = data.Email;
                _db.Accounts.Update(accountDetails);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<Account?> GetAccountDetailsById(int id)
        {
            Account? account = await _db.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            return account;
        }
        public async Task<IEnumerable<Country>> GetCountryList()
        {
            return _db.Countries.ToList();
        }
        public async Task<bool> register(RegisterDetailsViewModel data)
        {
            bool checkDublicate = await _db.Accounts.AnyAsync(x => x.Username.ToLower() == data.UserName.ToLower());
            if (!checkDublicate)
            {
                Account account = new();
                account.Username = data.UserName;
                account.Password = data.Password;
                account.Createddate = DateTime.Now;
                account.Emailid = data.Email;
                account.Firstname =data.FirstName; 
                account.Lastname =data.LastName;
                account.Accounttype = 1;
                _db.Accounts.Add(account);
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        //public async Task<bool> columnExist<T>(string tableName, string columnName)
        //{
        //    // Get the entity type
        //    var entityType = _db.Model.GetEntityTypes()
        //                        .FirstOrDefault(t => t.GetTableName().Equals(tableName, StringComparison.OrdinalIgnoreCase));

        //    if (entityType == null)
        //    {
        //        return false;
        //    }
        //    var column = entityType.GetProperties().FirstOrDefault(p => p.GetColumnName().Equals(columnName, StringComparison.OrdinalIgnoreCase));
        //    return column != null;
        //}
    }
}
