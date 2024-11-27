using System.ComponentModel.DataAnnotations;

namespace CustomerCrud.ViewModels
{
    public class CustomerCreateViewModel
    {
        [Required(ErrorMessage = "Customer name is Required.")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Address is Required.")]
        public string CustomerAddress { get; set; }

        public DateTime BusinessStart { get; set; } = DateTime.Now;

        public decimal CreditLimit { get; set; }

        public int CustomerTypeId { get; set; }

        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public string AddressName { get; set; }
    }
}
