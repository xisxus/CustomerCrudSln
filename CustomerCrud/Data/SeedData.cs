using CustomerCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCrud.Data
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<CustomerType>().HasData(
                new CustomerType { CustomerTypeId=-1, CustomerTypeName = "Retail", CreatedAt = DateTime.Now },
                new CustomerType { CustomerTypeId = -2, CustomerTypeName = "Wholesale", CreatedAt = DateTime.Now },
                new CustomerType { CustomerTypeId = -3, CustomerTypeName = "Corporate", CreatedAt = DateTime.Now }
            );

           
            modelBuilder.Entity<Customers>().HasData(
                new Customers
                {
                    
                    CustomerNo = "001",
                    CustomerName = "John Doe",
                    CustomerAddress = "123 Main St",
                    BusinessStart = new DateTime(2020, 1, 1),
                    CustomerTypeId = 1,
                    CreditLimit = 5000
                },
                new Customers
                {
                   
                    CustomerNo = "002",
                    CustomerName = "Jane Smith",
                    CustomerAddress = "456 Oak St",
                    BusinessStart = new DateTime(2019, 5, 15),
                    CustomerTypeId = 2,
                    CreditLimit = 10000
                },
                new Customers
                {
                    
                    CustomerNo = "003",
                    CustomerName = "Michael Brown",
                    CustomerAddress = "789 Pine St",
                    BusinessStart = new DateTime(2018, 11, 20),
                    CustomerTypeId = 3,
                    CreditLimit = 15000
                },
                new Customers
                {
                    
                    CustomerNo = "004",
                    CustomerName = "Alice Johnson",
                    CustomerAddress = "101 Maple St",
                    BusinessStart = new DateTime(2021, 6, 10),
                    CustomerTypeId = 1,
                    CreditLimit = 7000
                },
                new Customers
                {
                  
                    CustomerNo = "005",
                    CustomerName = "Robert Williams",
                    CustomerAddress = "202 Elm St",
                    BusinessStart = new DateTime(2017, 4, 5),
                    CustomerTypeId = 2,
                    CreditLimit = 8500
                },
                new Customers
                {
                   
                    CustomerNo = "006",
                    CustomerName = "Emily Davis",
                    CustomerAddress = "303 Cedar St",
                    BusinessStart = new DateTime(2022, 3, 1),
                    CustomerTypeId = 3,
                    CreditLimit = 12000
                },
                new Customers
                {
                    
                    CustomerNo = "007",
                    CustomerName = "William Martinez",
                    CustomerAddress = "404 Birch St",
                    BusinessStart = new DateTime(2019, 7, 21),
                    CustomerTypeId = 1,
                    CreditLimit = 9500
                },
                new Customers
                {
                 
                    CustomerNo = "008",
                    CustomerName = "Olivia Hernandez",
                    CustomerAddress = "505 Ash St",
                    BusinessStart = new DateTime(2020, 8, 13),
                    CustomerTypeId = 2,
                    CreditLimit = 11000
                },
                new Customers
                {
                   
                    CustomerNo = "009",
                    CustomerName = "James Wilson",
                    CustomerAddress = "606 Poplar St",
                    BusinessStart = new DateTime(2016, 2, 10),
                    CustomerTypeId = 3,
                    CreditLimit = 10000
                },
                new Customers
                {
                   
                    CustomerNo = "010",
                    CustomerName = "Sophia Moore",
                    CustomerAddress = "707 Walnut St",
                    BusinessStart = new DateTime(2015, 12, 25),
                    CustomerTypeId = 1,
                    CreditLimit = 7500
                },
                new Customers
                {
                   
                    CustomerNo = "011",
                    CustomerName = "David Taylor",
                    CustomerAddress = "808 Willow St",
                    BusinessStart = new DateTime(2020, 1, 30),
                    CustomerTypeId = 2,
                    CreditLimit = 14000
                },
                new Customers
                {
                    
                    CustomerNo = "012",
                    CustomerName = "Isabella Anderson",
                    CustomerAddress = "909 Hickory St",
                    BusinessStart = new DateTime(2021, 5, 20),
                    CustomerTypeId = 3,
                    CreditLimit = 12500
                },
                new Customers
                {
                    
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
                   
                    CustomerNo = "015",
                    CustomerName = "Ethan White",
                    CustomerAddress = "1212 Oak St",
                    BusinessStart = new DateTime(2019, 11, 3),
                    CustomerTypeId = 3,
                    CreditLimit = 16000
                },
                new Customers
                {
                   
                    CustomerNo = "016",
                    CustomerName = "Amelia Harris",
                    CustomerAddress = "1313 Elm St",
                    BusinessStart = new DateTime(2021, 4, 22),
                    CustomerTypeId = 1,
                    CreditLimit = 8500
                },
                new Customers
                {
                  
                    CustomerNo = "017",
                    CustomerName = "Daniel Martin",
                    CustomerAddress = "1414 Cedar St",
                    BusinessStart = new DateTime(2020, 7, 18),
                    CustomerTypeId = 2,
                    CreditLimit = 13000
                },
                new Customers
                {
                   
                    CustomerNo = "018",
                    CustomerName = "Charlotte Garcia",
                    CustomerAddress = "1515 Birch St",
                    BusinessStart = new DateTime(2019, 8, 27),
                    CustomerTypeId = 3,
                    CreditLimit = 10000
                },
                new Customers
                {
                    
                    CustomerNo = "019",
                    CustomerName = "Lucas Martinez",
                    CustomerAddress = "1616 Poplar St",
                    BusinessStart = new DateTime(2016, 3, 12),
                    CustomerTypeId = 1,
                    CreditLimit = 9500
                },
                new Customers
                {
                    
                    CustomerNo = "020",
                    CustomerName = "Harper Rodriguez",
                    CustomerAddress = "1717 Hickory St",
                    BusinessStart = new DateTime(2015, 6, 5),
                    CustomerTypeId = 2,
                    CreditLimit = 11000
                }
            
            );


            modelBuilder.Entity<Address>().HasData(
                new Address {  AddressName = "Home Address", CustomersId = 1 },
                new Address {  AddressName = "Office Address", CustomersId = 1 },
                new Address { AddressName = "Billing Address", CustomersId = 2 },
                new Address {  AddressName = "Shipping Address", CustomersId = 2 },
                new Address {  AddressName = "Home Address", CustomersId = 3 },
                new Address {  AddressName = "Office Address", CustomersId = 4 },
                new Address {  AddressName = "Branch Office", CustomersId = 4 },
                new Address {  AddressName = "Primary Residence", CustomersId = 5 },
                new Address { AddressName = "Vacation Home", CustomersId = 5 },
                new Address {  AddressName = "Headquarters", CustomersId = 6 },
                new Address { AddressName = "Warehouse", CustomersId = 7 },
                new Address { AddressName = "Distribution Center", CustomersId = 7 },
                new Address { AddressName = "Home Address", CustomersId = 8 },
                new Address { AddressName = "Main Office", CustomersId = 9 },
                new Address { AddressName = "Branch Office", CustomersId = 10 },
                new Address { AddressName = "Corporate Office", CustomersId = 11 },
                new Address { AddressName = "Remote Office", CustomersId = 12 },
                new Address { AddressName = "Temporary Residence", CustomersId = 13 },
                new Address { AddressName = "Warehouse", CustomersId = 14 },
                new Address { AddressName = "Regional Office", CustomersId = 15 },
                new Address { AddressName = "Downtown Office", CustomersId = 16 },
                new Address { AddressName = "Storage Facility", CustomersId = 17 },
                new Address { AddressName = "Main Branch", CustomersId = 18 },
                new Address { AddressName = "Corporate HQ", CustomersId = 19 },
                new Address { AddressName = "Overseas Office", CustomersId = 20 }
            );

        }
    }
}
