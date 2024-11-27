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

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Customers.Include(c => c.CustomerType);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers
                .Include(c => c.CustomerType)
                .FirstOrDefaultAsync(m => m.CustomersId == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }




        public IActionResult Create()
        {
            ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                "CustomerTypeId", "CustomerTypeName");

            // Pre-generate customer number for the form
            ViewBag.CustomerNumber = GenerateCustomerNumber();

            return View(new CustomerCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateViewModel viewModel)
        {
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
            // Get the last customer number or default to 0
            var lastCustomerNumber = _context.Customers
                .OrderByDescending(c => c.CustomersId)
                .Select(c => c.CustomerNo)
                .FirstOrDefault();

            // If no customers exist, start from 001
            if (string.IsNullOrEmpty(lastCustomerNumber))
            {
                return "001";
            }

            // Increment the last number
            int nextNumber = int.Parse(lastCustomerNumber) + 1;
            return nextNumber.ToString("D3"); // Ensures 3-digit format (001, 002, etc.)
        }



        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            ViewData["CustomerTypeId"] = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeId", customers.CustomerTypeId);
            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomersId,CustomerNo,CustomerName,CustomerAddress,BusinessStart,CustomerTypeId,CreditLimit")] Customers customers)
        {
            if (id != customers.CustomersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomersExists(customers.CustomersId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerTypeId"] = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeId", customers.CustomerTypeId);
            return View(customers);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers
                .Include(c => c.CustomerType)
                .FirstOrDefaultAsync(m => m.CustomersId == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            if (customers != null)
            {
                _context.Customers.Remove(customers);
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
