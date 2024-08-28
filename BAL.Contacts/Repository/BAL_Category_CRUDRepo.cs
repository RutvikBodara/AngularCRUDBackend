using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Contacts;
using DAL.Contacts.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BAL.Contacts.Repository
{
    public class BAL_Category_CRUDRepo : IBAL_Category_CRUD
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public BAL_Category_CRUDRepo(DAL.Contacts.Data.ConactsDBContext db)
        {
            _db = db;
        }
        public async Task<patentCategoryDetailsViewModel> get<T>(string? commonsearch, int? pagenumber, int? pagesize, string? sortedcolumn, string? sorteddirection)
        {
            //List<Product> product = _db.Products.ToList();
            //List<Category> categories = _db.Categories.ToList();

            var query = (from x1 in _db.Categories
                                   join x2 in _db.Products on x1.Id equals x2.Categoryid into temp
                                   from X2 in temp.DefaultIfEmpty()
                                   where x1.IsDeleted != true
                                   && (X2 == null || X2.IsDeleted != true)
                                   && (commonsearch == null
                                       || x1.Id.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                       || x1.Name.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                       //|| x1.Createddate.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                       )
                                   group new { x1, X2 } by new { x1.Id, x1.Name, x1.Createddate } into G
                                   orderby G.Key.Id
                                   select new categoryDetailViewModel()
                                   {
                                       id = G.Key.Id,
                                       name = G.Key.Name,
                                       createdDate = G.Key.Createddate,
                                       TotalProducts = G.Count(g => g.X2 != null)
                                   }).AsQueryable();

            patentCategoryDetailsViewModel model = new patentCategoryDetailsViewModel();
            model.maxPage = ((query.Count() % pagesize == 0) ? query.Count() % pagesize : (query.Count() % pagesize) + 1);
            model.dataCount = query.Count();
            // Apply sorting

            if (sortedcolumn == "name")
            {
                query = sorteddirection == "desc"
                    ? query.OrderByDescending(x => x.name)
                    : query.OrderBy(x => x.name);
            }
            else if (sortedcolumn == "id")
            {
                query = sorteddirection == "desc"
                    ? query.OrderByDescending(x => x.id)
                    : query.OrderBy(x => x.id);
            }
            else if (sortedcolumn == "createdDate")
            {
                query = sorteddirection == "desc"
                   ? query.OrderByDescending(x => x.createdDate)
                   : query.OrderBy(x => x.createdDate);
            }
            else if(sortedcolumn == "totalProducts")
            {
                query = sorteddirection == "desc"
             ? query.OrderByDescending(x => x.TotalProducts)
             : query.OrderBy(x => x.TotalProducts);
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
                    if (property.Name == "id")
                    {
  
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "name")
                    {

                        subColumn.isEditable = true;
                    }
                    else if (property.Name == "createdDate")
                    {

                        subColumn.isEditable = false;
                    }
                    else if(property.Name == "TotalProducts")
                    {
                        subColumn.columnName = "totalProducts";
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

            model.CategoryDetails = query;
            model.pageNumber = pagenumber;
            model.pageSize = pagesize;
            model.columnCredits = columnFields;
            
            return model;

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
                    details.Updatedby = DateTime.Now;
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
