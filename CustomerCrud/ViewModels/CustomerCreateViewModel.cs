using System.ComponentModel.DataAnnotations;

namespace CustomerCrud.ViewModels
{
    public class CustomerCreateViewModel
    {
        public int? CustomersId { get; set; }
        [Required(ErrorMessage = "Customer name is Required.")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        public string? CustomerNo { get; set; }


        [Required(ErrorMessage = "Address is Required.")]
        public string CustomerAddress { get; set; }

        public DateTime BusinessStart { get; set; } 

        public decimal CreditLimit { get; set; }

        public int CustomerTypeId { get; set; }

        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public string AddressName { get; set; }
    }
}
