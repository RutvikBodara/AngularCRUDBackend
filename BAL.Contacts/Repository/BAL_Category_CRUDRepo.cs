using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contacts.Repository
{
    public class BAL_Category_CRUDRepo : IBAL_Category_CRUD 
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public BAL_Category_CRUDRepo(DAL.Contacts.Data.ConactsDBContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<T>> get<T>()
        {
            return (IEnumerable<T>)(from x1 in _db.Categories
                                    join x2 in _db.Products on x1.Id equals x2.Categoryid into temp
                                    from X2 in temp.DefaultIfEmpty()
                                    where x1.IsDeleted != true && X2.IsDeleted != true
                                    group new { x1, X2 } by new { x1.Id, x1.Name,x1.Createddate } into G
                                    orderby G.Key.Id
                                    select new categoryDetailViewModel()
                                    {
                                        id = G.Key.Id,
                                        name = G.Key.Name,
                                        createdDate=G.Key.Createddate,
                                        TotalProducts=G.Count(G => G.X2 != null)
                                    });
        }
        public async Task<bool> add(categoryModel requestData)
        {
            bool checkDublicate = _db.Categories.Any(x => x.Name == requestData.name);
            if (!checkDublicate)
            {
                Category newCategory = new Category();
                newCategory.Name = requestData.name;
                newCategory.Createddate = DateTime.Now;
                _db.Categories.Add(newCategory);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<bool> update<T>(categoryDetailViewModel category)
        {
            if (await exists<Category>(category.id))
            {
                Category? details = await _db.Categories.FirstOrDefaultAsync(x => x.Id == category.id);
               bool checkDublicate = _db.Contacts.Any(x => x.Name.ToLower() == category.name.ToLower() && x.Id != category.id && x.Isdeleted != true);
                if (details != null && !checkDublicate)
                {
                        details.Name = category.name;
                        details.Updatedby=DateTime.Now;
                        _db.Categories.Update(details);
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
            if (await exists<Category>(id))
            {
                Category? details = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
                bool checkAvailProduct = _db.Products.Any(x => x.Categoryid == id && x.IsDeleted != true);
                if (details != null && !checkAvailProduct)
                {
                    details.IsDeleted = true;
                    details.Deletedby = DateTime.Now;
                    details.Updatedby = DateTime.Now;
                    _db.Categories.Update(details);
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
