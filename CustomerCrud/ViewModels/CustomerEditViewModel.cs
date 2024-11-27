using System.ComponentModel.DataAnnotations;

namespace CustomerCrud.ViewModels
{
    public class CustomerEditViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Customer Type")]
        public int? CustomerTypeId { get; set; }

        [Display(Name = "Primary Address")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string CustomerAddress { get; set; }

        [Display(Name = "Credit Limit")]
        [Range(0, double.MaxValue, ErrorMessage = "Credit limit must be a positive value.")]
        public decimal? CreditLimit { get; set; }

        [Display(Name = "Additional Addresses")]
        public List<AddressViewModels> Addresses { get; set; } = new List<AddressViewModels>();
    }

    public class AddressViewModels
    {
        public int? AddressId { get; set; } // For existing addresses
        [Required]
        [StringLength(250, ErrorMessage = "Address name cannot exceed 250 characters.")]
        public string AddressName { get; set; }
    }
}
