namespace CustomerCrud.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomersId { get; set; }
        public virtual Customers? Customers { get; set; }
    }
}
