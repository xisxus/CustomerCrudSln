using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerCrud.Models
{
    public class Customers
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomersId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? CustomerNo { get; set; }

        [Required(ErrorMessage = "Customer name is Required."), Display(Name = "Customer Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Address is Required.")]
        [Column(TypeName = "nvarchar(max)")]
        public string CustomerAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerPhoto { get; set; }
        public byte[] CustomerSignature { get; set; }
        public DateTime BusinessStart { get; set; }
        public int CustomerTypeId { get; set; }
        public decimal CreditLimit { get; set; }
        public ICollection<Address> AddressList { get; set; }
        public virtual CustomerType? CustomerType { get; set; }
    }
}
