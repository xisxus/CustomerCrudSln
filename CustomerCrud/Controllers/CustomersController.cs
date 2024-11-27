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
            viewModel.Addresses = viewModel.Addresses
           .Where(a => !string.IsNullOrWhiteSpace(a.AddressName))
           .ToList();

            if (!ModelState.IsValid)
            {
                ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                    "CustomerTypeId", "CustomerTypeName");
                return View(viewModel);
            }

            var customer = new Customers
            {
                CustomerNo = GenerateCustomerNumber(), // Regenerate to ensure uniqueness
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

            return RedirectToAction(nameof(Index));
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
            if (id <= 0)
            {
                return NotFound();
            }

            viewModel.Addresses = viewModel.Addresses
                .Where(a => !string.IsNullOrWhiteSpace(a.AddressName))
                .ToList();

            if (!ModelState.IsValid)
            {
                ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                    "CustomerTypeId", "CustomerTypeName", viewModel.CustomerTypeId);
                return View(viewModel);
            }

            var customer = await _context.Customers
                .Include(c => c.AddressList)
                .FirstOrDefaultAsync(c => c.CustomersId == id);

            if (customer == null)
            {
                return NotFound();
            }

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

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


       

        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.CustomersId == id);
        }
    }
}
