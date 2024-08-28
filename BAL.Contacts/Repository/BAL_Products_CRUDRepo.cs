using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BAL.Contacts.Repository
{
    public class BAL_Products_CRUDRepo : IBAL_Products_CRUD
    {
        private readonly DAL.Contacts.Data.ConactsDBContext _db;

        public BAL_Products_CRUDRepo(DAL.Contacts.Data.ConactsDBContext db)
        {
            _db = db;
        }
        public async Task<patentProductDetailsViewModel> get<T>(string? commonsearch, int? pagenumber, int? pagesize, string? sortedcolumn, string? sorteddirection, bool? download)
        {

            //List<Product> product = _db.Products.ToList();
            //List<Category> categories = _db.Categories.ToList();

            var query = (IQueryable<productDetailsViewModel>)(from x1 in _db.Products
                                                              join x2 in _db.Categories on x1.Categoryid equals x2.Id
                                                              where x1.IsDeleted != true
                                                              && (commonsearch == null
                                                                  || x1.Id.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || x1.Name.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  //|| x2.Name.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  //|| x1.Createddate.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  //|| (x1.Updatedby != null && x1.Updatedby.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower()))
                                                                  || x1.Description.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || (x1.HelplineNumber != null && x1.HelplineNumber.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())))
                                                              orderby x1.Id
                                                              select new productDetailsViewModel
                                                              {
                                                                  id = x1.Id,
                                                                  name = x1.Name,
                                                                  categoryName = x2.Name,
                                                                  categoryId = x2.Id,
                                                                  createddate = x1.Createddate,
                                                                  description = x1.Description,
                                                                  updatedDate = x1.Updatedby,
                                                                  helplineNumber = x1.HelplineNumber.ToString() ?? "",
                                                                  rating = x1.Rating,
                                                                  image = x1.ProductImage,
                                                                  imageName = x1.Image,
                                                                  launchDate = x1.LaunchDate.ToString(),
                                                                  lastDate = x1.LastDate.ToString(),
                                                                  price = x1.Price,
                                                                  countryServed = x1.CountryServed,
                                                                  availableForSale = x1.Availableforsale
                                                              }).AsQueryable();

            if (download == true)
            {
                query = (IQueryable<productDetailsViewModel>)(from x1 in _db.Products
                                                              join x2 in _db.Categories on x1.Categoryid equals x2.Id
                                                              where x1.IsDeleted != true
                                                              && (commonsearch == null
                                                                  || x1.Id.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || x1.Name.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || x2.Name.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || x1.Createddate.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || (x1.Updatedby != null && x1.Updatedby.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower()))
                                                                  || x1.Description.Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())
                                                                  || (x1.HelplineNumber != null && x1.HelplineNumber.ToString().Replace(" ", string.Empty).ToLower().Contains(commonsearch.Replace(" ", string.Empty).ToLower())))
                                                              orderby x1.Id
                                                              select new productDetailsViewModel
                                                              {
                                                                  id = x1.Id,
                                                                  name = x1.Name,
                                                                  categoryName = x2.Name,
                                                                  categoryId = x2.Id,
                                                                  createddate = x1.Createddate,
                                                                  description = x1.Description,
                                                                  updatedDate = x1.Updatedby,
                                                                  helplineNumber = x1.HelplineNumber.ToString() ?? "",
                                                                  rating = x1.Rating,
                                                                  //image = x1.ProductImage,
                                                                  imageName = x1.Image,
                                                                  launchDate = x1.LaunchDate.ToString(),
                                                                  lastDate = x1.LastDate.ToString(),
                                                                  price = x1.Price,
                                                                  countryServed = x1.CountryServed,
                                                                  availableForSale = x1.Availableforsale
                                                              }).AsQueryable();
            }
            patentProductDetailsViewModel model = new patentProductDetailsViewModel();
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
            else if (sortedcolumn == "categoryName")
            {
                query = sorteddirection == "desc"
                    ? query.OrderByDescending(x => x.categoryName)
                    : query.OrderBy(x => x.categoryName);
            }
            else if (sortedcolumn == "createddate")
            {
                query = sorteddirection == "desc"
                   ? query.OrderByDescending(x => x.createddate)
                   : query.OrderBy(x => x.createddate);
            }

            // Apply pagination if necessary
            if (pagenumber.HasValue && pagesize.HasValue && download != true)
            {
                query = query.Skip((pagenumber.Value - 1) * pagesize.Value).Take(pagesize.Value);
            }
            else
            {
                if (download != true)
                {
                query = query.Take(5);
                }
            }

            //add editable columns
            List<DAL_Column_BehaviourViewModel> columnFields = new List<DAL_Column_BehaviourViewModel>();
            DAL_Column_BehaviourViewModel subColumnDefault = new DAL_Column_BehaviourViewModel();
            subColumnDefault.columnName = "checkBox";
            columnFields.Add(subColumnDefault);

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
                    else if (property.Name == "createddate")
                    {
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "updatedDate")
                    {
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "categoryName")
                    {
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "helplineNumber")
                    {
                        subColumn.isEditable = true;
                    }
                    else if (property.Name == "rating")
                    {
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

            model.ProductDetails = query;
            model.pageNumber = pagenumber;
            model.pageSize = pagesize;
            model.columnCredits = columnFields;
            return model;
        }

        public async Task<patentProductDetailsViewModel> getById(int id)
        {
            var query = (IQueryable<productDetailsViewModel>)(from x1 in _db.Products
                                                         join x2 in _db.Categories on x1.Categoryid equals x2.Id
                                                         where x1.IsDeleted != true
                                                         && x1.Categoryid == id
                                                         orderby x1.Id
                                                         select new productDetailsViewModel
                                                         {
                                                             id = x1.Id,
                                                             name = x1.Name,
                                                             categoryName = x2.Name,
                                                             categoryId = x2.Id,
                                                             createddate = x1.Createddate,
                                                             description = x1.Description,
                                                             updatedDate = x1.Updatedby,
                                                             helplineNumber = x1.HelplineNumber.ToString() ?? "",
                                                             rating = x1.Rating,
                                                             image = x1.ProductImage,
                                                             imageName = x1.Image,
                                                             launchDate = x1.LaunchDate.ToString(),
                                                             lastDate = x1.LastDate.ToString(),
                                                             price = x1.Price,
                                                             countryServed = x1.CountryServed,
                                                             availableForSale = x1.Availableforsale
                                                         }).AsQueryable();

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
                    else if (property.Name == "createddate")
                    {
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "updatedDate")
                    {
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "categoryName")
                    {
                        subColumn.isEditable = false;
                    }
                    else if (property.Name == "helplineNumber")
                    {
                        subColumn.isEditable = true;
                    }
                    else if (property.Name == "rating")
                    {
                        subColumn.isEditable = false;
                    }
                    else
                    {
                        continue;
                    }
                    columnFields.Add(subColumn);
                }
            }
            patentProductDetailsViewModel model = new patentProductDetailsViewModel();
            model.ProductDetails = query;
            model.columnCredits = columnFields;
            return model;
        }

        public async Task<bool> add(string name, string description, string helplineNumber, string launchDate, string lastDateProduct, string categoryId, IFormFile file, string availableForSale, List<int> Countries, double price)
        {

            bool checkDublicate = _db.Products.Any(x => x.Name.ToLower() == name.ToLower() && x.IsDeleted != true);
            DateOnly dateOnly;
            if (!checkDublicate)
            {
                Product product = new Product();
                product.Name = name;
                product.HelplineNumber = decimal.Parse(helplineNumber);
                //process and store image here
                product.Createddate = DateTime.Now;
                product.Description = description;
                product.Categoryid = int.Parse(categoryId);
                product.Availableforsale = bool.Parse(availableForSale);
                product.Price = price;


                int index = 0;
                if (Countries.Count == 1 && Countries[0] == -1)
                {
                    IEnumerable<int> countries = _db.Countries.Select(x => x.Id);
                    short[] array = new short[countries.Count()];
                    foreach (var x in countries)
                    {
                        array[index++] = (short)x;
                    }
                    product.CountryServed = array;
                }
                else
                {
                    short[] array = new short[Countries.Count()];
                    foreach (var x in Countries)
                    {
                        array[index++] = (short)x;
                    }
                    product.CountryServed = array;
                }

                if (DateOnly.TryParse(launchDate, out dateOnly))
                {
                    product.LaunchDate = dateOnly;
                }

                if (DateOnly.TryParse(lastDateProduct, out dateOnly))
                {
                    product.LastDate = dateOnly;
                }

                //byte[] fileData;
                //using (var memoryStream = new MemoryStream())
                //{
                //    await file.CopyToAsync(memoryStream);
                //    fileData = memoryStream.ToArray();
                //}

                //upload photo here
                if (file != null && file.Length > 0)
                {
                    Guid myuuid = Guid.NewGuid();
                    var filename = Path.GetFileName(file.FileName);
                    var FinalFileName = myuuid.ToString() + filename;

                    var filepath = Path.Combine(Environment.CurrentDirectory, "upload", "product", FinalFileName);

                    using (var str = new FileStream(filepath, FileMode.Create))
                    {
                        await file.CopyToAsync(str);
                    }
                    product.Image = filepath;

                    Byte[] bytes = File.ReadAllBytes(filepath);
                    string file64 = Convert.ToBase64String(bytes);
                    product.ProductImage = file64;
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> update<T>(editProductViewModel product)
        {
            DateOnly dateOnly;
            if (await exists<Product>(product.id))
            {
                Product? details = await _db.Products.FirstOrDefaultAsync(x => x.Id == product.id);
                bool checkDublicate = _db.Products.Any(x => x.Name.ToLower() == product.name.ToLower() && x.Id != product.id && x.IsDeleted != true);
                if (!checkDublicate)
                {
                    if (details != null)
                    {
                        details.Name = product.name;
                        details.HelplineNumber = decimal.Parse(product.helplineNumber);
                        //details.Image = product.image;
                        details.Updatedby = DateTime.Now;
                        details.Description = product.description;
                        details.Categoryid = product.categoryId;
                        if (product.file != null)
                            details.ProductImage = product.file;
                        details.Availableforsale = product.availableForSale;
                        details.Price = product.price;

                        int index = 0;
                        if (product.countryServed.Count() == 1 && product.countryServed[0] == -1)
                        {
                            IEnumerable<int> countries = _db.Countries.Select(x => x.Id);
                            short[] array = new short[countries.Count()];
                            foreach (var x in countries)
                            {
                                array[index++] = (short)x;
                            }
                            details.CountryServed = array;
                        }
                        else
                        {
                            details.CountryServed = product.countryServed;
                        }


                        if (DateOnly.TryParse(product.launchDate, out dateOnly))
                        {
                            details.LaunchDate = dateOnly;
                        }

                        if (DateOnly.TryParse(product.lastDate, out dateOnly))
                        {
                            details.LastDate = dateOnly;
                        }

                        _db.Products.Update(details);
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
            if (await exists<Product>(id))
            {
                Product? details = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (details != null)
                {
                    details.IsDeleted = true;
                    details.DeletedBy = DateTime.Now;
                    _db.Products.Update(details);
                    _db.SaveChanges();
                    return true;
                }
                //we can use ref to add specific logs
                return false;
            }
            return false;
        }
        public async Task<bool> deleteBulk(string idlist)
        {
            string[] productIds = idlist.Split(',');

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (string id in productIds)
                    {
                        bool status = await delete<int>(int.Parse(id));
                        if (!status)
                        {
                            throw new Exception();
                        }
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception as needed
                    return false;
                }
            }
        }
        public async Task<bool> exists<T>(int id) where T : class
        {
            return await _db.Set<T>().FindAsync(id) != null;
        }
    }
}
