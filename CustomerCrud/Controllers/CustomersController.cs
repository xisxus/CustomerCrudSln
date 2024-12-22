using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomerCrud.Data;
using CustomerCrud.Models;
using CustomerCrud.ViewModels;
using Humanizer.Configuration;
using System.Net;

namespace CustomerCrud.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var totalCustomers = await _context.Customers.CountAsync();

            var customers = await _context.Customers
                .Include(c => c.CustomerType)
                .Include(c => c.AddressList)
                .Select(c => new CustomerListViewModel
                {
                    CustomerId = c.CustomersId,
                    CustomerName = c.CustomerName,
                    CustomerNo = c.CustomerNo,
                    BusinessStart = c.BusinessStart,
                    CustomerTypeName = c.CustomerType.CustomerTypeName,
                    CustomerAddress = c.CustomerAddress,
                    CreditLimit = c.CreditLimit,
                    AdditionalAddressesCount = c.AddressList.Count
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var startIndex = (page - 1) * pageSize + 1;
            var endIndex = Math.Min(page * pageSize, totalCustomers);

            var viewModel = new CustomerIndexViewModel
            {
                Customers = customers,
                TotalCustomers = totalCustomers,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCustomers / pageSize),
                StartIndex = startIndex,
                EndIndex = endIndex
            };

            return View(viewModel);
        }



       
        public IActionResult Details(int id)
        {
            var customer = _context.Customers
                .Include(c => c.AddressList)
                .Include(c => c.CustomerType)
                .FirstOrDefault(c => c.CustomersId == id);

            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerDetailsViewModel
            {
                CustomerNo = customer.CustomerNo,
                CustomerName = customer.CustomerName,
                BusinessStart = customer.BusinessStart,
                CustomerTypeName = customer.CustomerType.CustomerTypeName,
                CustomerAddress = customer.CustomerAddress,
                CreditLimit = customer.CreditLimit,
                Addresseslo = customer.AddressList.Select(a => new AddressViewModel
                {
                    AddressName = a.AddressName
                }).ToList()
            };

            return View(viewModel);
        }





        public IActionResult Create()
        {
            ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                "CustomerTypeId", "CustomerTypeName");

            // Pre-generate customer number for the form
            ViewBag.CustomerNumber = GenerateCustomerNumber();

            var viewModel = new CustomerCreateViewModel();
            // Optionally, add an initial empty address if you want
            viewModel.Addresses.Add(new AddressViewModel());

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateViewModel viewModel)
        {
            if (viewModel.CustomerName == null || viewModel.CustomerAddress == null || viewModel.CustomerTypeId == 0)
            {
                if (viewModel.CustomerName == null && viewModel.CustomerAddress == null && viewModel.CustomerTypeId == 0)
                {
                    return Json(new { success = false, message = "Enter Customer Name And Address And type" });
                }
                else if (viewModel.CustomerName == null)
                {
                    return Json(new { success = false, message = "Enter Customer Name" });
                }
                else if (viewModel.CustomerAddress == null)
                {
                    return Json(new { success = false, message = "Enter Delivery Address" });
                }
                else
                {
                    return Json(new { success = false, message = "Enter Customer Type" });

                }
            }

            List<AddressViewModel> addrss = new List<AddressViewModel>();

            foreach (var item in viewModel.Addresses)
            {
                if (item.AddressName != null)
                {
                    addrss.Add(item);
                }
            }

            viewModel.Addresses = addrss;

            //viewModel.Addresses = viewModel.Addresses
            //    .Where(a => !string.IsNullOrWhiteSpace(a.AddressName))
            //    .ToList();

            //if (!ModelState.IsValid)
            //{
            //    ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
            //        "CustomerTypeId", "CustomerTypeName");
            //    return View(viewModel);
            //}

            var customer = new Customers
            {
                CustomerNo = GenerateCustomerNumber(),
                CustomerName = viewModel.CustomerName,
                CustomerAddress = viewModel.CustomerAddress,
                BusinessStart = viewModel.BusinessStart,
                CreditLimit = viewModel.CreditLimit,
                CustomerTypeId = viewModel.CustomerTypeId,
                AddressList = viewModel.Addresses.Select(a => new Address
                {
                    AddressName = a.AddressName
                }).ToList()
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Return success message
            return Json(new { success = true, message = "Customer created successfully!" });
        }



        [HttpPost]
        public async Task<JsonResult> CreateCustomerType(string customerTypeName)
        {
            if (string.IsNullOrWhiteSpace(customerTypeName))
            {
                return Json(new { success = false, message = "Name cannot be empty" });
            }

            var customerType = new CustomerType
            {
                CustomerTypeName = customerTypeName
            };

            _context.CustomerTypes.Add(customerType);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                customerTypeId = customerType.CustomerTypeId,
                customerTypeName = customerType.CustomerTypeName
            });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetCustomerType()
        //{
        //    var result = await _context.CustomerTypes.ToListAsync();
        //    return Json(result); 
        //}

        [HttpGet]
        public async Task<IActionResult> GetCustomerTypes()
        {
            var customerTypes = await _context.CustomerTypes.Select(ct => new {
                ct.CustomerTypeId,
                ct.CustomerTypeName
            }).ToListAsync();
            return Ok(customerTypes); // Return JSON response
        }







        public string GenerateCustomerNumber()
        {
           
            var lastCustomerNumber = _context.Customers
                .OrderByDescending(c => c.CustomersId)
                .Select(c => c.CustomerNo)
                .FirstOrDefault();

          
            if (string.IsNullOrEmpty(lastCustomerNumber))
            {
                return "001";
            }

         
            int nextNumber = int.Parse(lastCustomerNumber) + 1;
            return nextNumber.ToString("D3"); 
        }



        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.AddressList)
                .FirstOrDefaultAsync(c => c.CustomersId == id);

            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                "CustomerTypeId", "CustomerTypeName", customer.CustomerTypeId);

            var viewModel = new CustomerCreateViewModel
            {
                CustomerNo = customer.CustomerNo,
                CustomerName = customer.CustomerName,
                CustomerAddress = customer.CustomerAddress,
                BusinessStart = customer.BusinessStart,
                CreditLimit = customer.CreditLimit,
                CustomerTypeId = customer.CustomerTypeId,
                Addresses = customer.AddressList
                    .Select(a => new AddressViewModel { AddressName = a.AddressName })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CustomerCreateViewModel viewModel)
        {
            // Validate customer name and address
            if (viewModel.CustomerName == null || viewModel.CustomerAddress == null || viewModel.CustomerTypeId == 0)
            {
                if (viewModel.CustomerName == null && viewModel.CustomerAddress == null && viewModel.CustomerTypeId == 0)
                {
                    return Json(new { success = false, message = "Enter Customer Name And Address And type" });
                }
                else if (viewModel.CustomerName == null)
                {
                    return Json(new { success = false, message = "Enter Customer Name" });
                }
                else if (viewModel.CustomerAddress == null)
                {
                    return Json(new { success = false, message = "Enter Delivery Address" });
                }
                else
                {
                    return Json(new { success = false, message = "Enter Customer Type" });

                }
            }

            List<AddressViewModel> addrss = new List<AddressViewModel>();

            foreach (var item in viewModel.Addresses)
            {
                if (item.AddressName != null)
                {
                    addrss.Add(item);
                }
            }

            viewModel.Addresses = addrss;

            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid Customer ID" });
            }

            // Remove empty addresses
            //viewModel.Addresses = viewModel.Addresses
            //    .Where(a => !string.IsNullOrWhiteSpace(a.AddressName))
            //    .ToList();

            //if (!ModelState.IsValid)
            //{
            //    // If validation fails, return the view with the model state and errors
            //    ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName", viewModel.CustomerTypeId);
            //    return Json(new { success = false, message = "Please fix validation errors." });
            //}

            var customer = await _context.Customers
                .Include(c => c.AddressList)
                .FirstOrDefaultAsync(c => c.CustomersId == id);

            if (customer == null)
            {
                return Json(new { success = false, message = "Customer not found" });
            }

            // Update the customer details
            customer.CustomerName = viewModel.CustomerName;
            customer.CustomerAddress = viewModel.CustomerAddress;
            customer.BusinessStart = viewModel.BusinessStart;
            customer.CreditLimit = viewModel.CreditLimit;
            customer.CustomerTypeId = viewModel.CustomerTypeId;

            // Update addresses
            customer.AddressList.Clear();
            foreach (var address in viewModel.Addresses)
            { 
                customer.AddressList.Add(new Address { AddressName = address.AddressName });
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return success message after update
            return Json(new { success = true, message = "Customer updated successfully!" });
        }

     



        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.CustomersId == id);
        }

        public async Task<IActionResult> ExistCustomerAndAddress(string? customerName, string? customerAddress)
        {
            
            var exists = await _context.Customers.AnyAsync(e => (customerName != null && e.CustomerName == customerName) || (customerAddress != null && e.CustomerAddress == customerAddress));

            
            if (exists)
            {
                return Json(new { success = true, message = "Data already exists!" });
            }
            else
            {
                return Json(new { success = false, message = "Customer or Address does not exist." });
            }
        }

        public async Task<IActionResult> ExistCustomerType(string? customerType)
        {

            var exists = await _context.CustomerTypes.AnyAsync(e => (customerType != null && e.CustomerTypeName == customerType));


            if (exists)
            {
                return Json(new { success = true, message = "Data already exists!" });
            }
            else
            {
                return Json(new { success = false, message = "Customer or Address does not exist." });
            }
        }


    }
}
