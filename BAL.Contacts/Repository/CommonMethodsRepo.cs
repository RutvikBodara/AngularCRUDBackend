using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Security.AccessControl;
using System.Security.Principal;


namespace BAL.Contacts.Repository
{
    public class CommonMethodsRepo:ICommonMethods
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public CommonMethodsRepo(DAL.Contacts.Data.ConactsDBContext conactsDBContext)
        {
            _db = conactsDBContext;
        }
        public async Task<IEnumerable<T>> AddErrorCode<T>(string segment)
        {
            IEnumerable<T> result = (IEnumerable<T>) (from x1 in _db.ErrorCodes
                                    where x1.Segment == segment
                                    select new DAL.Contacts.ViewModels.EroorCodes.EroorCodeViewModel()
                                    {
                                        errorCode=x1.ErrorCode1,
                                        message=x1.ErrorDescription
                                    });
            return result;
        }

    }
}
