namespace CustomerCrud.Models
{
    public class CustomerType
    {
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
