using CustomerCrud.Models;

namespace CustomerCrud.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public int CustomersId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string? CustomerTypeName { get; set; }
        public decimal CreditLimit { get; set; }
        public DateTime? BusinessStart { get; set; }
        public CustomerType CustomerType { get; set; }
        public List<Address> Addresses { get; set; }
        public List<AddressViewModel> Addresseslo { get; set; }
        
    }

 
   


}
