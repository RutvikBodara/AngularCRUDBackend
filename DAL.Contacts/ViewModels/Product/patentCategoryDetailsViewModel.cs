using DAL.Contacts.ViewModels.API.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contacts.ViewModels.Product
{
    public class patentCategoryDetailsViewModel
    {
        public IQueryable<categoryDetailViewModel> CategoryDetails { get; set; }
        public List<DAL_Column_BehaviourViewModel> columnCredits { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
        public int? dataCount { get; set; }
        public int? maxPage { get; set; }
    }
}
