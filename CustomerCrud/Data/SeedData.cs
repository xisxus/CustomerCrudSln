using CustomerCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCrud.Data
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<CustomerType>().HasData(
                new CustomerType { CustomerTypeId = 1, CustomerTypeName = "Retail", CreatedAt = DateTime.Now },
                new CustomerType { CustomerTypeId = 2, CustomerTypeName = "Wholesale", CreatedAt = DateTime.Now },
                new CustomerType { CustomerTypeId = 3, CustomerTypeName = "Corporate", CreatedAt = DateTime.Now }
            );

           
            modelBuilder.Entity<Customers>().HasData(
                new Customers
                {
                    CustomersId = 1,
                    CustomerNo = "001",
                    CustomerName = "John Doe",
                    CustomerAddress = "123 Main St",
                    BusinessStart = new DateTime(2020, 1, 1),
                    CustomerTypeId = 1,
                    CreditLimit = 5000
                },
                new Customers
                {
                    CustomersId = 2,
                    CustomerNo = "002",
                    CustomerName = "Jane Smith",
                    CustomerAddress = "456 Oak St",
                    BusinessStart = new DateTime(2019, 5, 15),
                    CustomerTypeId = 2,
                    CreditLimit = 10000
                },
                new Customers
                {
                    CustomersId = 3,
                    CustomerNo = "003",
                    CustomerName = "Michael Brown",
                    CustomerAddress = "789 Pine St",
                    BusinessStart = new DateTime(2018, 11, 20),
                    CustomerTypeId = 3,
                    CreditLimit = 15000
                },
                new Customers
                {
                    CustomersId = 4,
                    CustomerNo = "004",
                    CustomerName = "Alice Johnson",
                    CustomerAddress = "101 Maple St",
                    BusinessStart = new DateTime(2021, 6, 10),
                    CustomerTypeId = 1,
                    CreditLimit = 7000
                },
                new Customers
                {
                    CustomersId = 5,
                    CustomerNo = "005",
                    CustomerName = "Robert Williams",
                    CustomerAddress = "202 Elm St",
                    BusinessStart = new DateTime(2017, 4, 5),
                    CustomerTypeId = 2,
                    CreditLimit = 8500
                },
                new Customers
                {
                    CustomersId = 6,
                    CustomerNo = "006",
                    CustomerName = "Emily Davis",
                    CustomerAddress = "303 Cedar St",
                    BusinessStart = new DateTime(2022, 3, 1),
                    CustomerTypeId = 3,
                    CreditLimit = 12000
                },
                new Customers
                {
                    CustomersId = 7,
                    CustomerNo = "007",
                    CustomerName = "William Martinez",
                    CustomerAddress = "404 Birch St",
                    BusinessStart = new DateTime(2019, 7, 21),
                    CustomerTypeId = 1,
                    CreditLimit = 9500
                },
                new Customers
                {
                    CustomersId = 8,
                    CustomerNo = "008",
                    CustomerName = "Olivia Hernandez",
                    CustomerAddress = "505 Ash St",
                    BusinessStart = new DateTime(2020, 8, 13),
                    CustomerTypeId = 2,
                    CreditLimit = 11000
                },
                new Customers
                {
                    CustomersId = 9,
                    CustomerNo = "009",
                    CustomerName = "James Wilson",
                    CustomerAddress = "606 Poplar St",
                    BusinessStart = new DateTime(2016, 2, 10),
                    CustomerTypeId = 3,
                    CreditLimit = 10000
                },
                new Customers
                {
                    CustomersId = 10,
                    CustomerNo = "010",
                    CustomerName = "Sophia Moore",
                    CustomerAddress = "707 Walnut St",
                    BusinessStart = new DateTime(2015, 12, 25),
                    CustomerTypeId = 1,
                    CreditLimit = 7500
                },
                new Customers
                {
                    CustomersId = 11,
                    CustomerNo = "011",
                    CustomerName = "David Taylor",
                    CustomerAddress = "808 Willow St",
                    BusinessStart = new DateTime(2020, 1, 30),
                    CustomerTypeId = 2,
                    CreditLimit = 14000
                },
                new Customers
                {
                    CustomersId = 12,
                    CustomerNo = "012",
                    CustomerName = "Isabella Anderson",
                    CustomerAddress = "909 Hickory St",
                    BusinessStart = new DateTime(2021, 5, 20),
                    CustomerTypeId = 3,
                    CreditLimit = 12500
                },
                new Customers
                {
                    CustomersId = 13,
                    CustomerNo = "013",
                    CustomerName = "Christopher Thomas",
                    CustomerAddress = "1010 Cherry St",
                    BusinessStart = new DateTime(2018, 10, 15),
                    CustomerTypeId = 1,
                    CreditLimit = 10500
                },
                new Customers
                {
                    CustomersId = 14,
                    CustomerNo = "014",
                    CustomerName = "Mia Jackson",
                    CustomerAddress = "1111 Maple St",
                    BusinessStart = new DateTime(2017, 9, 8),
                    CustomerTypeId = 2,
                    CreditLimit = 9000
                },
                new Customers
                {
                    CustomersId = 15,
                    CustomerNo = "015",
                    CustomerName = "Ethan White",
                    CustomerAddress = "1212 Oak St",
                    BusinessStart = new DateTime(2019, 11, 3),
                    CustomerTypeId = 3,
                    CreditLimit = 16000
                },
                new Customers
                {
                    CustomersId = 16,
                    CustomerNo = "016",
                    CustomerName = "Amelia Harris",
                    CustomerAddress = "1313 Elm St",
                    BusinessStart = new DateTime(2021, 4, 22),
                    CustomerTypeId = 1,
                    CreditLimit = 8500
                },
                new Customers
                {
                    CustomersId = 17,
                    CustomerNo = "017",
                    CustomerName = "Daniel Martin",
                    CustomerAddress = "1414 Cedar St",
                    BusinessStart = new DateTime(2020, 7, 18),
                    CustomerTypeId = 2,
                    CreditLimit = 13000
                },
                new Customers
                {
                    CustomersId = 18,
                    CustomerNo = "018",
                    CustomerName = "Charlotte Garcia",
                    CustomerAddress = "1515 Birch St",
                    BusinessStart = new DateTime(2019, 8, 27),
                    CustomerTypeId = 3,
                    CreditLimit = 10000
                },
                new Customers
                {
                    CustomersId = 19,
                    CustomerNo = "019",
                    CustomerName = "Lucas Martinez",
                    CustomerAddress = "1616 Poplar St",
                    BusinessStart = new DateTime(2016, 3, 12),
                    CustomerTypeId = 1,
                    CreditLimit = 9500
                },
                new Customers
                {
                    CustomersId = 20,
                    CustomerNo = "020",
                    CustomerName = "Harper Rodriguez",
                    CustomerAddress = "1717 Hickory St",
                    BusinessStart = new DateTime(2015, 6, 5),
                    CustomerTypeId = 2,
                    CreditLimit = 11000
                }
            
            );


            modelBuilder.Entity<Address>().HasData(
                new Address { Id = 1, AddressName = "Home Address", CustomersId = 1 },
                new Address { Id = 2, AddressName = "Office Address", CustomersId = 1 },
                new Address { Id = 3, AddressName = "Billing Address", CustomersId = 2 },
                new Address { Id = 4, AddressName = "Shipping Address", CustomersId = 2 },
                new Address { Id = 5, AddressName = "Home Address", CustomersId = 3 },
                new Address { Id = 6, AddressName = "Office Address", CustomersId = 4 },
                new Address { Id = 7, AddressName = "Branch Office", CustomersId = 4 },
                new Address { Id = 8, AddressName = "Primary Residence", CustomersId = 5 },
                new Address { Id = 9, AddressName = "Vacation Home", CustomersId = 5 },
                new Address { Id = 10, AddressName = "Headquarters", CustomersId = 6 },
                new Address { Id = 11, AddressName = "Warehouse", CustomersId = 7 },
                new Address { Id = 12, AddressName = "Distribution Center", CustomersId = 7 },
                new Address { Id = 13, AddressName = "Home Address", CustomersId = 8 },
                new Address { Id = 14, AddressName = "Main Office", CustomersId = 9 },
                new Address { Id = 15, AddressName = "Branch Office", CustomersId = 10 },
                new Address { Id = 16, AddressName = "Corporate Office", CustomersId = 11 },
                new Address { Id = 17, AddressName = "Remote Office", CustomersId = 12 },
                new Address { Id = 18, AddressName = "Temporary Residence", CustomersId = 13 },
                new Address { Id = 19, AddressName = "Warehouse", CustomersId = 14 },
                new Address { Id = 20, AddressName = "Regional Office", CustomersId = 15 },
                new Address { Id = 21, AddressName = "Downtown Office", CustomersId = 16 },
                new Address { Id = 22, AddressName = "Storage Facility", CustomersId = 17 },
                new Address { Id = 23, AddressName = "Main Branch", CustomersId = 18 },
                new Address { Id = 24, AddressName = "Corporate HQ", CustomersId = 19 },
                new Address { Id = 25, AddressName = "Overseas Office", CustomersId = 20 }
            );

        }
    }
}
