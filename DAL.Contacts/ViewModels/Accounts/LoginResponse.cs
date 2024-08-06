namespace DAL.Contacts.ViewModels.API.Output
{
    public class LoginResponse
    {
        public string UserName {  get; set; }
        public decimal? MobileNumber { get; set; }
        public string JWTToken { get; set; }
        public int AccountId { get; set; }
        public string? EmailId { get; set; }

    }
}
