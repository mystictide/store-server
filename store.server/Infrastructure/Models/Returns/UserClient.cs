namespace store.server.Infrastructure.Models.Returns
{
    public class UserClient
    {
        public int? ID { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name
        {
            get { return FirstName + " " + LastName; }
            set { }
        }
    }
}
