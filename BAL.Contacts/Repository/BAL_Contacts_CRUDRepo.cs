using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Contacts;
using DAL.Contacts.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
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
        public async Task<patentContactsDetailsViewModel> get<T>(string? name, string? surname, int? id, string? typeList, int? pagenumber, int? pagesize, string? sortedcolumn, string? sorteddirection)
        {

            List<int> searchTypeList = new List<int>();
            if (typeList != null)
            {
                searchTypeList = typeList.Split(',').Select(int.Parse).ToList();
            }

            var contactList = _db.Contacts.ToList();

            var query = (IEnumerable<Contact>)(from x1 in contactList
                                                            where
                                                             x1.Isdeleted != true
                                                            &&
                                                            ((name == null || x1.Name.ToLower().Contains(name.ToLower()))
                                                            || (name == null || x1.Surname.ToLower().Contains(name.ToLower())))
                                                            && (id == null || id == x1.Id)
                                                            && (typeList == null || searchTypeList.Any(x => x == x1.ContactTypeId))
                                                            orderby x1.Id
                                                            select new Contact()
                                                            {
                                                                Name = x1.Name,
                                                                Surname = x1.Surname,
                                                                Id = x1.Id
                                                            });

            patentContactsDetailsViewModel model = new patentContactsDetailsViewModel();
            model.maxPage = ((query.Count() % pagesize == 0) ? query.Count() % pagesize : (query.Count() % pagesize) + 1);
            model.dataCount = query.Count();
            // Apply sorting

            if (sortedcolumn == "name")
            {
                query = sorteddirection == "desc"
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name);
            }
            else if (sortedcolumn == "id")
            {
                query = sorteddirection == "desc"
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id);
            }
            else if (sortedcolumn == "surname")
            {
                query = sorteddirection == "desc"
                    ? query.OrderByDescending(x => x.Surname)
                    : query.OrderBy(x => x.Surname);
            }
            
            // Apply pagination if necessary
            if (pagenumber.HasValue && pagesize.HasValue)
            {
                query = query.Skip((pagenumber.Value - 1) * pagesize.Value).Take(pagesize.Value);
            }
            else
            {
                query = query.Take(5);
            }

            List<DAL_Column_BehaviourViewModel> columnFields = new List<DAL_Column_BehaviourViewModel>();

            var firstItem = query.FirstOrDefault();
            if (firstItem != null)
            {
                PropertyInfo[] properties = firstItem.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    DAL_Column_BehaviourViewModel subColumn = new DAL_Column_BehaviourViewModel();
                    subColumn.columnName = property.Name;
                    Console.WriteLine(property.Name);
                    if (property.Name == "Id")
                    {
                        subColumn.columnName = "id";
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "Name")
                    {
                        subColumn.columnName = "name";
                        subColumn.isEditable = true;
                    }
                    else if (property.Name == "Surname")
                    {
                        subColumn.columnName = "surname";
                        subColumn.isEditable = false;
                    }
                    else
                    {
                        continue;
                    }
                    columnFields.Add(subColumn);
                }
            }
            DAL_Column_BehaviourViewModel subColumnAction = new DAL_Column_BehaviourViewModel();
            subColumnAction.columnName = "action";
            subColumnAction.isEditable = false;
            columnFields.Add(subColumnAction);


            model.contactDetails = query;
            model.pageNumber = pagenumber;
            model.pageSize = pagesize;
            model.columnCredits = columnFields;
            return model;
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
