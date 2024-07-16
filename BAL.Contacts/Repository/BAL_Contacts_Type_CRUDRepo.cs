using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.ContactType;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Repository
{
    public class BAL_Contacts_Type_CRUDRepo : IBAL_Contacts_Type_CRUD
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public BAL_Contacts_Type_CRUDRepo(DAL.Contacts.Data.ConactsDBContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<T>> get<T>(string? name, int? id, string? typeList)
        {

            List<int> searchTypeList = new List<int>();
            if (typeList != null)
            {
                searchTypeList = typeList.Split(',').Select(int.Parse).ToList();
            }

            var contactTypeList = _db.ContactTypes.ToList();
            var contactList = _db.Contacts.ToList();
            IEnumerable<T> fetchContactsType = (IEnumerable<T>)(from x1 in contactTypeList
                                                                join x2 in contactList on x1.Id equals x2.ContactTypeId into temp
                                                                from X2 in temp.DefaultIfEmpty()
                                                                where (name == null || x1.Name.ToLower().Contains(name.ToLower()))
                                                                   && (id == null || id == x1.Id)
                                                                   && x1.Isdeleted != true
                                                                   && (typeList == null || searchTypeList.Any(x => x == x1.Id ))
                                                                group new { x1, X2 } by new { x1.Id, x1.Name } into G
                                                                orderby G.Key.Id
                                                                select new DAL_ContactTypeViewModel()
                                                                {
                                                                    id = G.Key.Id,
                                                                    name = G.Key.Name,
                                                                    count = G.Count(g => g.X2 != null)
                                                                });

            return fetchContactsType;

        }
        public async Task add(DAL.Contacts.ViewModels.Contacts.contactsDetailViewModel<string> requestData)
        {
            ContactType contactType = new ContactType();
            contactType.Name = requestData.name;
            _db.ContactTypes.Add(contactType);
            await _db.SaveChangesAsync();
        }
        public async Task<bool> update<T>(ContactType contactsType)
        {
            if (await exists<ContactType>(contactsType.Id))
            {
                ContactType? details = await _db.ContactTypes.FirstOrDefaultAsync(x => x.Id == contactsType.Id);
                if (details != null)
                {
                    details.Name = contactsType.Name;
                    _db.ContactTypes.Update(details);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> delete<T>(int id)
        {
            if (await exists<ContactType>(id))
            {
                ContactType? details = await _db.ContactTypes.FirstOrDefaultAsync(x => x.Id == id);
                if (details != null)
                {
                    details.Isdeleted = true;
                    _db.ContactTypes.Update(details);
                    _db.SaveChanges();
                    return true;
                }
                //we can use ref to add specific logs
                return false;
            }
            return false;
        }
        public async Task<bool> exists<T>(int id) where T : class
        {
            return await _db.Set<T>().FindAsync(id) != null;
        }
    }
}
