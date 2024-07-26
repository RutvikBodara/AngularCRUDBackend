using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<T>> get<T>()
        {
            return (IEnumerable<T>)(from x1 in _db.Products
                                    join x2 in _db.Categories on x1.Categoryid equals x2.Id
                                    where x1.IsDeleted != true
                                    select new productDetailsViewModel()
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
                                        lastDate=x1.LastDate.ToString(),
                                        price = x1.Price,
                                        countryServed=(x1.CountryServed == null) ? null : x1.CountryServed,
                                        availableForSale = x1.Availableforsale
                                    }
                                    );
        }
        public async Task<bool> add(string name, string description, string helplineNumber, string launchDate,string lastDateProduct, string categoryId, IFormFile file, string availableForSale, List<int> Countries,double price)
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
                        if(product.file != null)
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
        public async Task<bool> exists<T>(int id) where T : class
        {
            return await _db.Set<T>().FindAsync(id) != null;
        }
    }
}
