using BAL.Contacts.Interface;
using DAL.Contacts.DataModels;
using DAL.Contacts.ViewModels.Accounts;
using DAL.Contacts.ViewModels.API.Output;
using DAL.Contacts.ViewModels.Product;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using static iTextSharp.text.pdf.AcroFields;


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
            Account? accountDetails = await _db.Accounts.FirstOrDefaultAsync(x => x.Id == data.Id && x.Isdeleted != true);

            if (accountDetails != null)
            {
                if (accountDetails.Username != data.UserName)
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
                account.Firstname = data.FirstName;
                account.Lastname = data.LastName;
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

        public async Task<string> DTOToBase64(IQueryable<productDetailsViewModel> data, List<DAL_Column_BehaviourViewModel> columns, int? doctype)
        {
            //string jsonData = JsonConvert.SerializeObject(data);

            //// Convert the JSON string to a byte array
            //byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            //// Convert the byte array to a Base64 string
            //return Convert.ToBase64String(dataBytes);
            if (doctype == 1)
            {
                return await dataToExcelBase64(data, columns);
            }
            else if (doctype == 2)
            {
                return await datatoPdfBase64(data, columns);
            }
            return string.Empty;
        }

        public async Task<string> dataToExcelBase64(IQueryable<productDetailsViewModel> data, List<DAL_Column_BehaviourViewModel> columns)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                int count = 1;
                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i].columnName != "checkBox" && columns[i].columnName != "action")
                        worksheet.Cells[1, count++].Value = columns[i].columnName;
                }


                int row = 2;
                count = 1;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        for (int i = 0; i < columns.Count; i++)
                        {

                            var property = typeof(productDetailsViewModel).GetProperty(columns[i].columnName, BindingFlags.Public | BindingFlags.Instance);
                            if (property != null && property.Name != "checkBox" && property.Name != "action")
                            {
                                var value = property.GetValue(item);

                                worksheet.Cells[row, count++].Value = value;
                            }
                        }
                        row++;
                        count = 1;
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    package.SaveAs(memoryStream);


                    byte[] excelBytes = memoryStream.ToArray();


                    string base64String = Convert.ToBase64String(excelBytes);

                    return base64String;
                }
            }
        }

        public async Task<string> datatoPdfBase64(IQueryable<productDetailsViewModel> data, List<DAL_Column_BehaviourViewModel> columns)
        {
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 60, 60);
                var pdfInstance =  PdfWriter.GetInstance(document, memoryStream);
                var base64ImageLogo = "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";
                var base64ImageFooter = "/9j/4AAQSkZJRgABAQEBLAEsAAD/4QBWRXhpZgAATU0AKgAAAAgABAEaAAUAAAABAAAAPgEbAAUAAAABAAAARgEoAAMAAAABAAIAAAITAAMAAAABAAEAAAAAAAAAAAEsAAAAAQAAASwAAAAB/+0ALFBob3Rvc2hvcCAzLjAAOEJJTQQEAAAAAAAPHAFaAAMbJUccAQAAAgAEAP/hDIFodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvADw/eHBhY2tldCBiZWdpbj0n77u/JyBpZD0nVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkJz8+Cjx4OnhtcG1ldGEgeG1sbnM6eD0nYWRvYmU6bnM6bWV0YS8nIHg6eG1wdGs9J0ltYWdlOjpFeGlmVG9vbCAxMS44OCc+CjxyZGY6UkRGIHhtbG5zOnJkZj0naHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyc+CgogPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9JycKICB4bWxuczp0aWZmPSdodHRwOi8vbnMuYWRvYmUuY29tL3RpZmYvMS4wLyc+CiAgPHRpZmY6UmVzb2x1dGlvblVuaXQ+MjwvdGlmZjpSZXNvbHV0aW9uVW5pdD4KICA8dGlmZjpYUmVzb2x1dGlvbj4zMDAvMTwvdGlmZjpYUmVzb2x1dGlvbj4KICA8dGlmZjpZUmVzb2x1dGlvbj4zMDAvMTwvdGlmZjpZUmVzb2x1dGlvbj4KIDwvcmRmOkRlc2NyaXB0aW9uPgoKIDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PScnCiAgeG1sbnM6eG1wTU09J2h0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8nPgogIDx4bXBNTTpEb2N1bWVudElEPnhtcC5paWQ6MTNkNjk3MjEtODU5Mi00NzhkLTk1YTktYjA2NTRjYTMyOWE0PC94bXBNTTpEb2N1bWVudElEPgogIDx4bXBNTTpJbnN0YW5jZUlEPmFkb2JlOmRvY2lkOnN0b2NrOmY2OWJiNWE4LTNjN2EtNGJhNy1hOTM2LWNmMGIzNGE5OTU3MzwveG1wTU06SW5zdGFuY2VJRD4KIDwvcmRmOkRlc2NyaXB0aW9uPgo8L3JkZjpSREY+CjwveDp4bXBtZXRhPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAo8P3hwYWNrZXQgZW5kPSd3Jz8+/9sAQwAFAwQEBAMFBAQEBQUFBgcMCAcHBwcPCwsJDBEPEhIRDxERExYcFxMUGhURERghGBodHR8fHxMXIiQiHiQcHh8e/9sAQwEFBQUHBgcOCAgOHhQRFB4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4eHh4e/8AAEQgBaAGeAwERAAIRAQMRAf/EAB0AAQACAgMBAQAAAAAAAAAAAAAEBgUHAQMIAgn/xABJEAEAAQMCAgQHCwsDAgcAAAAAAQIDBAURBjEHEiFBE1FhcYGhsQgiMjVCUnSDkcHRFBYj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";
                var headingText = "Product List";
                var footerText = "thank you for download details";
                pdfInstance.PageEvent = new PageBorderEvent();
                document.Open();
                if (!string.IsNullOrEmpty(base64ImageLogo))
                {
                    byte[] imageBytes = Convert.FromBase64String(base64ImageLogo);
                    Image image = Image.GetInstance(imageBytes);
                    image.ScaleToFit(120f, 120f);
                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);
                    document.Add(new Paragraph("\n"));

                }

                if (!string.IsNullOrEmpty(headingText))
                {
                    var headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                    var heading = new Paragraph(headingText, headingFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    document.Add(heading);
                    document.Add(new Paragraph("\n"));
                }



                PdfPTable table = new PdfPTable(columns.Count - 2);

                table.WidthPercentage = 111;
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                var oddBackgroundColor = HexToBaseColor("#A0A0A0");
                var evenBackgroundColor = HexToBaseColor("#FFFFFF");

                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i].columnName != "checkBox" && columns[i].columnName != "action")
                    {

                        var headerCell = new PdfPCell(new Phrase(columns[i].columnName, headerFont))
                        {
                            BackgroundColor = oddBackgroundColor,
                            Padding = 5,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            //NoWrap = true
                        };
                        table.AddCell(headerCell);
                    }
                }

                int count = 1;
                // Adding data rows from DTO list
                foreach (var productvalue in data)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {

                        var property = typeof(productDetailsViewModel).GetProperty(columns[i].columnName, BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.Name != "checkBox" && property.Name != "action")
                        {
                            var value = property.GetValue(productvalue);

                            if (value is DateTime dateTimeValue)
                            {
                                var cell = new PdfPCell(new Phrase(dateTimeValue.ToString("MMMM dd, yyyy"), cellFont))
                                {
                                    BackgroundColor = (count % 2 == 0) ? oddBackgroundColor : evenBackgroundColor,
                                    Padding = 5,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    //NoWrap = true
                                };

                                table.AddCell(cell);
                            }
                            else
                            {

                                var cell = new PdfPCell(new Phrase((value != null) ? value.ToString() : "-", cellFont))
                                {
                                    BackgroundColor = (count % 2 == 0) ? oddBackgroundColor : evenBackgroundColor,
                                    Padding = 5,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    //NoWrap = true 
                                };
                                table.AddCell(cell);
                            }

                        }
                    }
                    count++;
                    //table.AddCell(productvalue.id.ToString());
                    //table.AddCell(productvalue.name);
                    //table.AddCell(productvalue.description);
                    //table.AddCell(productvalue.price.ToString());
                    //table.AddCell(productvalue.createddate.ToString("MMMM dd, yyyy")); 
                    ////table.AddCell(DateTime.Now.ToString("MMMM dd, yyyy"));
                }

                document.Add(table);
                document.Add(new Paragraph("\n"));

                if (!string.IsNullOrEmpty(base64ImageFooter))
                {
                    byte[] imageBytes = Convert.FromBase64String(base64ImageFooter);
                    Image image = Image.GetInstance(imageBytes);
                    image.ScaleToFit(400f, 150f);
                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);
                    document.Add(new Paragraph("\n"));
                }

                if (!string.IsNullOrEmpty(footerText))
                {
                    var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                    var footer = new Paragraph(footerText, footerFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    document.Add(footer);
                    document.Add(new Paragraph("\n"));
                }

                document.Close();

                byte[] pdfBytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(pdfBytes);

                return base64String;
            }
        }

        public class PageBorderEvent : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                Rectangle pageSize = document.PageSize;
                PdfContentByte canvas = writer.DirectContent;
                canvas.SetColorStroke(BaseColor.BLACK);
                canvas.SetLineWidth(1.5f);

                // Draw borders
                canvas.Rectangle(pageSize.GetLeft(10), pageSize.GetBottom(10), pageSize.Width - 20, pageSize.Height - 20);
                canvas.Stroke();
            }
        }

        private BaseColor HexToBaseColor(string hex)
        {
            // Remove the # if present
            hex = hex.TrimStart('#');

            // Parse the hex value to integer
            int argb = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            // Extract the RGB components
            byte r = (byte)((argb >> 16) & 0xFF);
            byte g = (byte)((argb >> 8) & 0xFF);
            byte b = (byte)(argb & 0xFF);

            return new BaseColor(r, g, b);
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
