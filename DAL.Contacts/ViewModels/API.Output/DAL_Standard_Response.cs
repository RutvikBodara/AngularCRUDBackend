namespace DAL.Contacts.ViewModels.API.Output
{
    public class DAL_Standard_Response<T>
    {
        public int code {  get; set; }
        public string message { get; set; }

        public int? pageNumber {  get; set; }
        public int? pageSize { get; set; } 
        public int? dataCount { get; set; }
        public int? maxPage { get; set; }
        public List<DAL_Column_BehaviourViewModel>? columnCredits { get; set; }
        public T responseData{ get; set; }
    }
}
