using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Repository
{
    public class BAL_Contacts_CRUDRepo : IBAL_Contacts_CRUD
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public BAL_Contacts_CRUDRepo(DAL.Contacts.Data.ConactsDBContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<T>> get<T>(string? name, string? surname, int? id, string? typeList)
        {

            List<int> searchTypeList = new List<int>();
            if (typeList != null)
            {
                searchTypeList = typeList.Split(',').Select(int.Parse).ToList();
            }

            var contactList = _db.Contacts.ToList();

            IEnumerable<T> fetchContacts = (IEnumerable<T>)(from x1 in contactList
                                                            where
                                                            (name == null || x1.Name.ToLower().Contains(name.ToLower()))
                                                            && (surname == null || x1.Surname.ToLower().Contains(surname.ToLower()))
                                                            && (id == null || id == x1.Id)
                                                            && (typeList == null || searchTypeList.Any(x => x == x1.ContactTypeId))
                                                            && x1.Isdeleted != true
                                                            orderby x1.Id
                                                            select new Contact()
                                                            {
                                                                Name = x1.Name,
                                                                Surname = x1.Surname,
                                                                Id = x1.Id
                                                            });
            return fetchContacts;

        }
        public async Task<bool> add(DAL.Contacts.ViewModels.Contacts.contactsDetailViewModel<string> requestData)
        {
            bool checkDublicate = _db.Contacts.Any(x => x.Name.ToLower() == requestData.name.ToLower()&& x.Surname.ToLower() == requestData.surname.ToLower() && x.Isdeleted != true);
            if (!checkDublicate)
            {
                Contact contact = new Contact();
                contact.Name = requestData.name;
                contact.Surname = requestData.surname;
                _db.Contacts.Add(contact);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> update<T>(Contact contacts)
        {
            if (await exists<Contact>(contacts.Id))
            {
                Contact? details = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == contacts.Id);
                if (details != null)
                {
                    bool checkDublicate = _db.Contacts.Any(x => x.Name.ToLower() == contacts.Name.ToLower() && x.Surname.ToLower() == contacts.Surname.ToLower()  &&x.Id != contacts.Id && x.Isdeleted != true);
                    if (!checkDublicate)
                    {
                        details.Name = contacts.Name;
                        details.Surname = contacts.Surname;
                        _db.Contacts.Update(details);
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
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
            if(await exists<Contact>(id)) 
            {
                Contact? details = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id);
                if (details != null)
                {
                    details.Isdeleted = true;
                    _db.Contacts.Update(details);
                    _db.SaveChanges();
                    return true;
                }
                //we can use ref to add specific 
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
