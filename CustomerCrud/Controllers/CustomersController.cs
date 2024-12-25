using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomerCrud.Data;
using CustomerCrud.Models;
using CustomerCrud.ViewModels;
using Humanizer.Configuration;
using System.Net;
using DocumentFormat.OpenXml.Packaging;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


using QuestPDF.Helpers;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Vml.Office;

using System.Drawing;  // For image dimension validation
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Drawing;



namespace CustomerCrud.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CustomersController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        public async Task<IActionResult> Index(int page = 1,  string sortField = "CustomerName", string sortOrder = "asc", string selectedIds = "")
        {
            // Parse the selected IDs (if any)
            var selectedCustomerIds = string.IsNullOrEmpty(selectedIds)
                ? new List<int>()
                : selectedIds.Split(',').Select(int.Parse).ToList();

            // Retrieve the total customer count
            var totalCustomers = await _context.Customers.CountAsync();



            // Base query for customers
            var query = _context.Customers
                .Include(c => c.CustomerType)
                .Include(c => c.AddressList)  // Ensure addresses are included
                .Select(c => new CustomerListViewModel
                {
                    CustomerId = c.CustomersId,
                    CustomerName = c.CustomerName,
                    CustomerNo = c.CustomerNo,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    BusinessStart = c.BusinessStart,
                    CustomerTypeName = c.CustomerType.CustomerTypeName,
                    CustomerAddress = c.CustomerAddress,
                    CreditLimit = c.CreditLimit,
                    AdditionalAddressesCount = c.AddressList.Count,
                    CustomerPhotoLink = c.CustomerPhoto,
                    //CustomerSignature = c.CustomerSignature,
                    IsSelected = selectedCustomerIds.Contains(c.CustomersId),
                    //CustomerSignatureUrl = c.CustomerSignature != null ? Url.Action("DisplaySignature", "Customer", new { customerId = c.CustomersId }) : null,
                    // CustomerSignatureUrl = DisplaySignature(c.CustomersId),

                    //CustomerSignature = c.CustomerSignature != null ? Convert.ToBase64String(c.CustomerSignature) : null,
                    //CustomerSignatureUrl = c.CustomerSignature != null ? "data:image/jpeg;base64," + Convert.ToBase64String(c.CustomerSignature) : null,

                    // Keep CustomerSignature as byte array, but convert it to Base64 for display
                    CustomerSignature = c.CustomerSignature,
                    CustomerSignatureUrl = c.CustomerSignature != null ? "data:image/jpeg;base64," + Convert.ToBase64String(c.CustomerSignature) : null,


                    // Map the address name to the DeliveryAddresses list
                    Addresses = c.AddressList.Select(a => new AddressViewModel
                    {
                        AddressName = a.AddressName  // Assuming AddressName is the field in your Address entity
                    }).ToList()
                });

            // Apply sorting
            query = sortField switch
            {
                "CustomerNo" => sortOrder == "asc" ? query.OrderBy(c => c.CustomerNo) : query.OrderByDescending(c => c.CustomerNo),
                "CustomerName" => sortOrder == "asc" ? query.OrderBy(c => c.CustomerName) : query.OrderByDescending(c => c.CustomerName),
                "CustomerTypeName" => sortOrder == "asc" ? query.OrderBy(c => c.CustomerTypeName) : query.OrderByDescending(c => c.CustomerTypeName),
                "BusinessStart" => sortOrder == "asc" ? query.OrderBy(c => c.BusinessStart) : query.OrderByDescending(c => c.BusinessStart),
                "CreditLimit" => sortOrder == "asc" ? query.OrderBy(c => c.CreditLimit) : query.OrderByDescending(c => c.CreditLimit),
                _ => sortOrder == "asc" ? query.OrderBy(c => c.CustomerName) : query.OrderByDescending(c => c.CustomerName),
            };

            // Apply pagination
            var customers = await query
                //.Skip((page - 1) * pageSize)
                //.Take(pageSize)
                .ToListAsync();

            // Pagination details
            //var startIndex = (page - 1) * pageSize + 1;
            //var endIndex = Math.Min(page * pageSize, totalCustomers);

            // Create the ViewModel
            var viewModel = new CustomerIndexViewModel
            {
                Customers = customers,
                TotalCustomers = totalCustomers,
                CurrentPage = page,
               // PageSize = pageSize,
               // TotalPages = (int)Math.Ceiling((double)totalCustomers / pageSize),
                //StartIndex = startIndex,
                //EndIndex = endIndex,
                SortField = sortField,
                SortOrder = sortOrder,
                SelectedIds = selectedIds // Preserve selected IDs
            };

            return View(viewModel);
        }


        //public async Task<IActionResult> DisplaySignature(int customerId)
        //{
        //    // Retrieve the customer by ID from the database
        //    var customer = await _context.Customers
        //        .FirstOrDefaultAsync(c => c.CustomersId == customerId);

        //    if (customer != null && customer.CustomerSignature != null)
        //    {
        //        // Return the signature as an image
        //        return File(customer.CustomerSignature, "image/jpeg");  // Adjust MIME type if necessary (e.g., "image/png")
        //    }

        //    return NotFound();  // Return 404 if no signature found
        //}



        public async Task<IActionResult> Index1(int page = 1, int pageSize = 5, string sortField = "CustomerName", string sortOrder = "asc", string selectedIds = "")
        {
            // Parse the selected IDs (if any)
            var selectedCustomerIds = string.IsNullOrEmpty(selectedIds)
                ? new List<int>()
                : selectedIds.Split(',').Select(int.Parse).ToList();

            // Retrieve the total customer count
            var totalCustomers = await _context.Customers.CountAsync();

            // Base query for customers
            var query = _context.Customers
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
                    AdditionalAddressesCount = c.AddressList.Count,
                    IsSelected = selectedCustomerIds.Contains(c.CustomersId) ,
                    Addresses = c.AddressList.Select(a => new AddressViewModel
                    {
                        AddressName = a.AddressName
                    }).ToList()
                });

            // Apply sorting
            query = sortField switch
            {
                "CustomerNo" => sortOrder == "asc" ? query.OrderBy(c => c.CustomerNo) : query.OrderByDescending(c => c.CustomerNo),
                "CustomerName" => sortOrder == "asc" ? query.OrderBy(c => c.CustomerName) : query.OrderByDescending(c => c.CustomerName),
                "CustomerTypeName" => sortOrder == "asc" ? query.OrderBy(c => c.CustomerTypeName) : query.OrderByDescending(c => c.CustomerTypeName),
                "BusinessStart" => sortOrder == "asc" ? query.OrderBy(c => c.BusinessStart) : query.OrderByDescending(c => c.BusinessStart),
                "CreditLimit" => sortOrder == "asc" ? query.OrderBy(c => c.CreditLimit) : query.OrderByDescending(c => c.CreditLimit),
                _ => sortOrder == "asc" ? query.OrderBy(c => c.CustomerName) : query.OrderByDescending(c => c.CustomerName),
            };

            // Apply pagination
            var customers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pagination details
            var startIndex = (page - 1) * pageSize + 1;
            var endIndex = Math.Min(page * pageSize, totalCustomers);

            // Create the ViewModel
            var viewModel = new CustomerIndexViewModel
            {
                Customers = customers,
                TotalCustomers = totalCustomers,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCustomers / pageSize),
                StartIndex = startIndex,
                EndIndex = endIndex,
                SortField = sortField,
                SortOrder = sortOrder,
                SelectedIds = selectedIds // Preserve selected IDs
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> GenerateReport(string reportType, List<int> selectedCustomerIds)
        {
            var customers = await _context.Customers.Include(e=>e.CustomerType)
                .Where(c => selectedCustomerIds.Contains(c.CustomersId))
                .ToListAsync();

            // Logic for generating PDF, Excel, or Word
            byte[] reportData;
            string fileName;

            switch (reportType.ToLower())
            {
                case "pdf":
                    reportData = GeneratePdfUsingQuestPDF(customers);
                    fileName = "CustomerReport.pdf";
                    break;
                case "excel":
                    reportData = GenerateExcel(customers);
                    fileName = "CustomerReport.xlsx";
                    break;
                case "word":
                    reportData = GenerateWord(customers);
                    fileName = "CustomerReport.docx";
                    break;
                default:
                    return BadRequest("Invalid report type.");
            }

            return File(reportData, "application/octet-stream", fileName);
        }



        //private byte[] GeneratePdfUsingSyncfusion(List<Customers> customers)
        //{
        //    using var memoryStream = new MemoryStream();
        //    var document = new Syncfusion.Pdf.PdfDocument();
        //    var page = document.Pages.Add();
        //    var graphics = page.Graphics;

        //    // Add Title
        //    var titleFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 16);
        //    graphics.DrawString("Customer Report", titleFont, Syncfusion.Pdf.Graphics.PdfBrushes.Black, new Syncfusion.Pdf.Graphics.PointF(200, 20));

        //    // Add Table
        //    var pdfGrid = new Syncfusion.Pdf.Tables.PdfGrid();
        //    pdfGrid.Columns.Add(6);

        //    // Add Headers
        //    pdfGrid.Headers.Add(1);
        //    var header = pdfGrid.Headers[0];
        //    header.Cells[0].Value = "Customer No";
        //    header.Cells[1].Value = "Name";
        //    header.Cells[2].Value = "Type";
        //    header.Cells[3].Value = "Address";
        //    header.Cells[4].Value = "Business Start";
        //    header.Cells[5].Value = "Credit Limit";

        //    // Add Rows
        //    foreach (var customer in customers)
        //    {
        //        pdfGrid.Rows.Add(new string[]
        //        {
        //    customer.CustomerNo,
        //    customer.CustomerName,
        //    customer.CustomerType?.CustomerTypeName ?? "N/A",
        //    customer.CustomerAddress ?? "N/A",
        //    customer.BusinessStart.ToString("yyyy-MM-dd") ?? "N/A",
        //    customer.CreditLimit.ToString("C") ?? "N/A"
        //        });
        //    }

        //    pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, 50));
        //    document.Save(memoryStream);
        //    return memoryStream.ToArray();
        //}

        private byte[] GeneratePdfUsingQuestPDF(List<Customers> customers)
        {
            try
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(11.69f, 8.27f, Unit.Inch);
                        page.Margin(1, Unit.Inch);
                        page.Header().Text("Customer Report").Bold().FontSize(20).AlignCenter();

                        page.Content().Table(table =>
                        {
                            // Define columns (added 2 more for photo and signature)
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(); // Photo
                                columns.RelativeColumn(); // Signature
                            });

                            // Add Headers (including new columns)
                            table.Header(header =>
                            {
                                // Previous headers remain the same
                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Customer ID").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Name").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Type").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Email").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Phone").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Address").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Business Start").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Credit Limit").Bold();

                                // New headers for photo and signature
                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Photo").Bold();

                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text("Signature").Bold();
                            });

                            // Add Rows
                            foreach (var customer in customers)
                            {
                                // Previous cells remain the same
                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text(customer.CustomerNo);

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignLeft()
                                    .Text(customer.CustomerName);

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text(customer.CustomerType?.CustomerTypeName ?? "N/A");

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignLeft()
                                    .Text(customer.Email);

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignLeft()
                                    .Text(customer.PhoneNumber);

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignLeft()
                                    .Text(customer.CustomerAddress ?? "N/A");

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text(customer.BusinessStart.ToString("dd-MM-yyyy") ?? "N/A");

                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .AlignRight()
                                    .Text(customer.CreditLimit == 0 ? "0" : customer.CreditLimit.ToString("N0"));

                                // Add photo cell
                                table.Cell()
                                     .Border(1).BorderColor(Colors.Black)
                                     .Padding(5)
                                     .Element(cell =>
                                     {
                                         if (!string.IsNullOrEmpty(customer.CustomerPhoto))
                                         {
                                             try
                                             {
                                                 // Convert relative path to absolute path
                                                 string relativePath = customer.CustomerPhoto.TrimStart('/', '\\');
                                                 string fullPath = Path.Combine(_env.WebRootPath, relativePath);

                                                 if (System.IO.File.Exists(fullPath))
                                                 {
                                                     byte[] imageBytes = System.IO.File.ReadAllBytes(fullPath);
                                                     var image = QuestPDF.Infrastructure.Image.FromBinaryData(imageBytes);

                                                     cell.Image(image)
                                                         .FitArea()
                                                         .WithCompressionQuality(ImageCompressionQuality.Medium);
                                                        // .WithHeight(50);
                                                 }
                                                 else
                                                 {
                                                     cell.Text($"Photo not found").FontSize(8);
                                                 }
                                             }
                                             catch (Exception ex)
                                             {
                                                 cell.Text($"Error: {ex.Message}").FontSize(8);
                                             }
                                         }
                                         else
                                         {
                                             cell.Text("No Photo").FontSize(8);
                                         }
                                     });


                                // Add signature cell
                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5)
                                    .Element(cell =>
                                    {
                                        if (customer.CustomerSignature != null && customer.CustomerSignature.Length > 0)
                                        {
                                            try
                                            {
                                                using (var signatureStream = new MemoryStream(customer.CustomerSignature))
                                                {
                                                    cell.Image(signatureStream)
                                                        .FitArea()
                                                        .WithCompressionQuality(ImageCompressionQuality.Medium);
                                                        //.WithHeight(50);
                                                }
                                            }
                                            catch
                                            {
                                                cell.Text("Error loading signature");
                                            }
                                        }
                                        else
                                        {
                                            cell.Text("No Signature");
                                        }
                                    });
                            }
                        });
                    });
                });

                using var memoryStream = new MemoryStream();
                document.GeneratePdf(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public byte[] GenerateExcel(List<Customers> customers)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Customers");

            // Set title
            worksheet.Cells[1, 1].Value = "Customer Report";
            worksheet.Cells[1, 1, 1, 10].Merge = true; // Merge cells for the title
            worksheet.Cells[1, 1].Style.Font.Size = 16; // Title font size
            worksheet.Cells[1, 1].Style.Font.Bold = true; // Title bold
            worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center align title
            worksheet.Cells[1, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Fill pattern
            worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Title background color

            // Add headers
            worksheet.Cells[2, 1].Value = "Customer ID";
            worksheet.Cells[2, 2].Value = "Name";
            worksheet.Cells[2, 3].Value = "Type";
            worksheet.Cells[2, 4].Value = "Email";
            worksheet.Cells[2, 5].Value = "Phone";
            worksheet.Cells[2, 6].Value = "Address";
            worksheet.Cells[2, 7].Value = "Business Start";
            worksheet.Cells[2, 8].Value = "Credit Limit";
            worksheet.Cells[2, 9].Value = "Photo";
            worksheet.Cells[2, 10].Value = "Signature";

            // Style headers
            using (var headerRange = worksheet.Cells[2, 1, 2, 10])
            {
                headerRange.Style.Font.Bold = true; // Bold headers
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Fill pattern
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Header background color
                headerRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black); // Border around headers
            }

            // Add data
            for (int i = 0; i < customers.Count; i++)
            {
                int rowIndex = i + 3;

                worksheet.Cells[rowIndex, 1].Value = customers[i].CustomerNo;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 2].Value = customers[i].CustomerName;
                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 3].Value = customers[i].CustomerType?.CustomerTypeName ?? "N/A";
                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 4].Value = customers[i].Email ?? "N/A";
                worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 5].Value = customers[i].PhoneNumber ?? "N/A";
                worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 6].Value = customers[i].CustomerAddress ?? "N/A";
                worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 7].Value = customers[i].BusinessStart.ToString("dd-MM-yyyy") ?? "N/A";
                worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 8].Value = customers[i].CreditLimit == 0 ? "0" : customers[i].CreditLimit.ToString("N0");
                worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                // Add photo
                if (!string.IsNullOrEmpty(customers[i].CustomerPhoto))
                {
                    try
                    {
                        var relativePath = customers[i].CustomerPhoto.TrimStart('/', '\\');
                        var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                        if (System.IO.File.Exists(fullPath))
                        {
                            var image = System.IO.File.ReadAllBytes(fullPath);
                            var picture = worksheet.Drawings.AddPicture($"Photo_{i}", new MemoryStream(image));
                            picture.SetPosition(rowIndex - 1, 0, 8, 0); // Row, RowOffset, Column, ColumnOffset
                            picture.SetSize(60, 60); // Set image size
                            worksheet.Row(rowIndex).Height = 50;
                        }
                    }
                    catch
                    {
                        worksheet.Cells[rowIndex, 9].Value = "Error loading photo";
                    }
                }
                else
                {
                    worksheet.Cells[rowIndex, 9].Value = "No Photo";
                }

                // Add signature
                if (customers[i].CustomerSignature != null && customers[i].CustomerSignature.Length > 0)
                {
                    try
                    {
                        var picture = worksheet.Drawings.AddPicture($"Signature_{i}", new MemoryStream(customers[i].CustomerSignature));
                        picture.SetPosition(rowIndex - 1, 0, 9, 0); // Row, RowOffset, Column, ColumnOffset
                        picture.SetSize(60, 10); // Set image size
                        worksheet.Row(rowIndex).Height = 50;
                    }
                    catch
                    {
                        worksheet.Cells[rowIndex, 10].Value = "Error loading signature";
                    }
                }
                else
                {
                    worksheet.Cells[rowIndex, 10].Value = "No Signature";
                }
            }

            // Loop through each cell in the worksheet to apply borders
            int lastRow = customers.Count + 2; // Total number of rows (including header)
            int lastColumn = 10; // Number of columns (Customer details + Photo + Signature)

            for (int row = 1; row <= lastRow; row++)
            {
                for (int col = 1; col <= lastColumn; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }
            }




            //// Add borders to all cells with data
            //var dataRange = worksheet.Cells[2, 1, customers.Count + 2, 10];
            //dataRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black); // Border around all data

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }





        //public byte[] GenerateExcel(List<Customers> customers)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

        //    using var package = new ExcelPackage();

        //    var worksheet = package.Workbook.Worksheets.Add("Customers");

        //    // Add headers
        //    worksheet.Cells[1, 1].Value = "Customer ID";
        //    worksheet.Cells[1, 2].Value = "Name";
        //    worksheet.Cells[1, 3].Value = "Type";
        //    worksheet.Cells[1, 4].Value = "Address";
        //    worksheet.Cells[1, 5].Value = "Business Start";
        //    worksheet.Cells[1, 6].Value = "Credit Limit";

        //    // Add data
        //    for (int i = 0; i < customers.Count; i++)
        //    {
        //        //worksheet.Cells[i + 2, 1].Value = customers[i].CustomerNo;
        //        //worksheet.Cells[i + 2, 2].Value = customers[i].CustomerName;
        //        //worksheet.Cells[i + 2, 3].Value = customers[i].CustomerType?.CustomerTypeName ?? "N/A";
        //        //worksheet.Cells[i + 2, 4].Value = customers[i].CustomerAddress ?? "N/A";
        //        //worksheet.Cells[i + 2, 5].Value = customers[i].BusinessStart.ToString("dd-MM-yyyy") ?? "N/A";
        //        //worksheet.Cells[i + 2, 6].Value = customers[i].CreditLimit.ToString("N2") ?? "N/A";

        //        // Right align "Customer No" and "Credit Limit"
        //        worksheet.Cells[i + 2, 1].Value = customers[i].CustomerNo;
        //        worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //        worksheet.Cells[i + 2, 2].Value = customers[i].CustomerName;
        //        worksheet.Cells[i + 2, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left; // Left align Name

        //        worksheet.Cells[i + 2, 3].Value = customers[i].CustomerType?.CustomerTypeName ?? "N/A";
        //        worksheet.Cells[i + 2, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Left align Type

        //        worksheet.Cells[i + 2, 4].Value = customers[i].CustomerAddress ?? "N/A";
        //        worksheet.Cells[i + 2, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left; // Left align Address

        //        worksheet.Cells[i + 2, 5].Value = customers[i].BusinessStart.ToString("dd-MM-yyyy") ?? "N/A";
        //        worksheet.Cells[i + 2, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center align Business Start

        //        //worksheet.Cells[i + 2, 6].Value = customers[i].CreditLimit.ToString() ?? "0";
        //        worksheet.Cells[i + 2, 6].Value = customers[i].CreditLimit == 0 ? "0" : customers[i].CreditLimit.ToString("N0");
        //        worksheet.Cells[i + 2, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right; // Right align Credit Limit
        //    }

        //    return package.GetAsByteArray();
        //}




        //private byte[] GenerateExcel(List<Customers> customers)
        //{
        //    using var package = new OfficeOpenXml.ExcelPackage();
        //    var worksheet = package.Workbook.Worksheets.Add("Customer Report");

        //    // Add headers
        //    worksheet.Cells[1, 1].Value = "Customer No";
        //    worksheet.Cells[1, 2].Value = "Name";
        //    worksheet.Cells[1, 3].Value = "Type";
        //    worksheet.Cells[1, 4].Value = "Address";
        //    worksheet.Cells[1, 5].Value = "Business Start";
        //    worksheet.Cells[1, 6].Value = "Credit Limit";

        //    // Add customer data
        //    for (int i = 0; i < customers.Count; i++)
        //    {
        //        var customer = customers[i];
        //        worksheet.Cells[i + 2, 1].Value = customer.CustomerNo;
        //        worksheet.Cells[i + 2, 2].Value = customer.CustomerName;
        //        worksheet.Cells[i + 2, 3].Value = customer.CustomerType?.CustomerTypeName ?? "N/A";
        //        worksheet.Cells[i + 2, 4].Value = customer.CustomerAddress ?? "N/A";
        //        worksheet.Cells[i + 2, 5].Value = customer.BusinessStart.ToString("dd-MM-yyyy") ?? "N/A";
        //        worksheet.Cells[i + 2, 6].Value = customer.CreditLimit;
        //    }

        //    return package.GetAsByteArray();
        //}

        private byte[] GenerateWord(List<Customers> customers)
        {
            using var memoryStream = new MemoryStream();
            using (var wordDocument = WordprocessingDocument.Create(memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                // Add the main document part
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                var body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());

                // Add Title
                var titleParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                        new DocumentFormat.OpenXml.Wordprocessing.Text("Customer Report"))
                );
                titleParagraph.ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties
                {
                    Justification = new DocumentFormat.OpenXml.Wordprocessing.Justification
                    {
                        Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Center
                    },
                    SpacingBetweenLines = new DocumentFormat.OpenXml.Wordprocessing.SpacingBetweenLines { After = "200" }
                };
                body.Append(titleParagraph);

                // Create Table
                var table = new DocumentFormat.OpenXml.Wordprocessing.Table();

                // Add Table Properties
                var tableProperties = new DocumentFormat.OpenXml.Wordprocessing.TableProperties(
                    new DocumentFormat.OpenXml.Wordprocessing.TableBorders(
                        new DocumentFormat.OpenXml.Wordprocessing.TopBorder { Val = DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single, Size = 8 },
                        new DocumentFormat.OpenXml.Wordprocessing.BottomBorder { Val = DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single, Size = 8 },
                        new DocumentFormat.OpenXml.Wordprocessing.LeftBorder { Val = DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single, Size = 8 },
                        new DocumentFormat.OpenXml.Wordprocessing.RightBorder { Val = DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single, Size = 8 },
                        new DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder { Val = DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single, Size = 8 },
                        new DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder { Val = DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single, Size = 8 }
                    )
                );
                table.AppendChild(tableProperties);

                // Add Headers
                var headerRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                headerRow.Append(
                    CreateHeaderCell("Customer ID"),
                    CreateHeaderCell("Name"),
                    CreateHeaderCell("Type"),
                    CreateHeaderCell("Email"),
                    CreateHeaderCell("Phone"),
                    CreateHeaderCell("Address"),
                    CreateHeaderCell("Business Start"),
                    CreateHeaderCell("Credit Limit"),
                    CreateHeaderCell("Photo"),
                    CreateHeaderCell("Signature")
                );
                table.Append(headerRow);

                // Add Customer Data
                foreach (var customer in customers)
                {
                    var row = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                    row.Append(
                        CreateTableCell(customer.CustomerNo),
                        CreateTableCell(customer.CustomerName),
                        CreateTableCell(customer.CustomerType?.CustomerTypeName ?? "N/A"),
                        CreateTableCell(customer.Email ?? "N/A"),
                        CreateTableCell(customer.PhoneNumber ?? "N/A"),
                        CreateTableCell(customer.CustomerAddress ?? "N/A"),
                        CreateTableCell(customer.BusinessStart.ToString("dd-MM-yyyy")),
                        CreateTableCell(customer.CreditLimit == 0 ? "0" : customer.CreditLimit.ToString("N0")),
                        CreateImageCell(mainPart, imagePath: customer.CustomerPhoto), // For Photo from File
                        CreateImageCell(mainPart, imageBytes: customer.CustomerSignature) // For Signature from Database (byte[])
                    );
                    table.Append(row);
                }

                // Append the table to the document body
                body.Append(table);

                // Save changes to the Word document
                mainPart.Document.Save();
            }

            return memoryStream.ToArray();
        }


        private DocumentFormat.OpenXml.Wordprocessing.TableCell CreateImageCell(MainDocumentPart mainPart, string imagePath = null, byte[] imageBytes = null)
        {
            if (string.IsNullOrEmpty(imagePath) && imageBytes != null)
            //if (!string.IsNullOrEmpty(imagePath) && imageBytes != null)
            {
                return CreateTableCell("N/A");
            }

            ImagePart imagePart = null;

            try
            {
                // If imageBytes are provided, create a memory stream
                if (imageBytes != null)
                {
                    imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                    using (var imageStream = new MemoryStream(imageBytes))
                    {
                        imagePart.FeedData(imageStream);
                    }
                }
                else if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                {
                    imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                    using (var imageStream = new FileStream(imagePath, FileMode.Open))
                    {
                        imagePart.FeedData(imageStream);
                    }
                }
                else
                {
                    return CreateTableCell("N/A");
                }

                // Check if imagePart is correctly created
                if (imagePart == null)
                {
                    return CreateTableCell("Image load error");
                }

                // Set the desired size for the image (2 inches wide by 1.5 inches tall)
                long widthInEMU = 2 * 91440;  // 2 inches = 182880 EMUs
                long heightInEMU = 1 * 91440;  // 1.5 inches = 137160 EMUs

                var image = new Drawing(
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                        new DocumentFormat.OpenXml.Drawing.Graphic(
                            new DocumentFormat.OpenXml.Drawing.GraphicData(
                                new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                        new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties
                                        {
                                            Id = (UInt32Value)1U,
                                            Name = "Picture"
                                        },
                                        new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()
                                    ),
                                    new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                        new DocumentFormat.OpenXml.Drawing.Blip { Embed = mainPart.GetIdOfPart(imagePart) },
                                        new DocumentFormat.OpenXml.Drawing.Stretch(new DocumentFormat.OpenXml.Drawing.FillRectangle())
                                    ),
                                    new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties()
                                )
                            )
                            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                        )
                    )
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U,
                        Extent = new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent { Cx = widthInEMU, Cy = heightInEMU }
                    });

                var paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                    new DocumentFormat.OpenXml.Wordprocessing.Run(image));

                return new DocumentFormat.OpenXml.Wordprocessing.TableCell(paragraph);
            }
            catch (Exception ex)
            {
                // Log the exception (or output to debug console)
                Console.WriteLine($"Error embedding image: {ex.Message}");
                return CreateTableCell("Image error");
            }
        }


        // Helper Method: Create Table Cell
        private DocumentFormat.OpenXml.Wordprocessing.TableCell CreateTableCell(string text)
        {
            return new DocumentFormat.OpenXml.Wordprocessing.TableCell(
                new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                        new DocumentFormat.OpenXml.Wordprocessing.Text(text)
                    )
                )
            );
        }

        // Helper Method: Create Header Cell
        private DocumentFormat.OpenXml.Wordprocessing.TableCell CreateHeaderCell(string text)
        {
            return new DocumentFormat.OpenXml.Wordprocessing.TableCell(
                new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties(
                    new DocumentFormat.OpenXml.Wordprocessing.Shading { Fill = "CCCCCC" } // Light gray background
                ),
                new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                        new DocumentFormat.OpenXml.Wordprocessing.Text(text)
                    )
                )
            );
        }

        // Helper Method: Create Image Cell



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

                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                CustomerPhotoLink = customer.CustomerPhoto,



                CustomerSignatureShow = customer.CustomerSignature,
                CustomerSignatureLink = customer.CustomerSignature != null ? "data:image/jpeg;base64," + Convert.ToBase64String(customer.CustomerSignature) : null,




                Addresseslo = customer.AddressList.Select(a => new AddressViewModel
                {
                    ContactPerson = a.ContactPerson,
                    PhoneNumber = a.PhoneNumber,
                    AddressName = a.AddressName
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult DeleteCustomerTypes( List<int> customerTypeIds)
        {
            if (customerTypeIds == null || !customerTypeIds.Any())
                return Json(new { success = false, message = "No customer types selected." });

            try
            {
                foreach (var item in customerTypeIds)
                {
                    var customer = _context.CustomerTypes.FirstOrDefault(c => c.CustomerTypeId == item);
                    _context.CustomerTypes.Remove(customer);
                    _context.SaveChanges();

                }
                // Perform deletion logic here
                // Example: _customerTypeService.DeleteCustomerTypes(customerTypeIds);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        public IActionResult Create()
        {
            ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                "CustomerTypeId", "CustomerTypeName");

            // Pre-generate customer number for the form
            ViewBag.CustomerNumber = GenerateCustomerNumber();
            ViewBag.CustomerTypeNumber = GenerateCustomerTypeNumber();

            var viewModel = new CustomerCreateViewModel();
            // Optionally, add an initial empty address if you want
            viewModel.Addresses.Add(new AddressViewModel());

            return View(viewModel);
        }


       

        private async Task<IActionResult> SaveCustomerPhotoAsync(IFormFile photo, string customFileName)
        {
            if (photo != null && photo.Length > 0)
            {
                // Check the file size (must be less than 100 KB)
                if (photo.Length > 1000 * 1024)  // 100 KB in bytes
                {
                    return Json(new { success = false, message = "File size exceeds the maximum limit of 100 KB." });
                }

                // Define the folder path where you want to save the image
                var folderPath = Path.Combine(_env.WebRootPath, "images");

                // Create the folder if it does not exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Get the file extension
                var fileExtension = Path.GetExtension(photo.FileName).ToLower();

                // Allow only image extensions (e.g., .jpg, .jpeg, .png)
                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                {
                    return Json(new { success = false, message = "Invalid file type. Only .jpg, .jpeg, or .png files are allowed." });
                   // return "Invalid file type. Only .jpg, .jpeg, or .png files are allowed.";
                }

                // Generate the custom file name with the original file extension
                var fileName = customFileName + fileExtension;

                // Define the full file path
                var filePath = Path.Combine(folderPath, fileName);

                // Check the image dimensions (must be at least 300x300)
                using (var stream = new MemoryStream())
                {
                    await photo.CopyToAsync(stream);
                    using (var image = System.Drawing.Image.FromStream(stream))
                    {
                        if (image.Width != 300 || image.Height != 300)
                        {
                            return Json(new { success = false, message = "Image dimensions must be at least 300x300 pixels." });

                            //return ";
                        }
                    }
                }

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Return the relative path to the image
               // return "/images/" + fileName;
                return Json(new { success = true, message = "/images/" + fileName });
                }

            return Json(new { success = false, message = "No file uploaded" });

            //return "No file uploaded.";
        }


    

       

        private async Task<(string message, byte[] fileData)> ConvertFileToByteArray(IFormFile file)
            {
                if (file != null && file.Length > 0)
                {
                    // Check the file size (must be less than 100 KB)
                    if (file.Length > 1000 * 1024)  // 100 KB in bytes
                    {
                        return ("File size exceeds the maximum limit of 100 KB.", null);
                    }

                    // Get the file extension and validate that it's an image (JPEG, PNG, etc.)
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        return ("Invalid file type. Only .jpg, .jpeg, or .png files are allowed.", null);
                    }

                    // Check the image dimensions (must be between 80x80 and 300x300)
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);  // Copy the file content to the memory stream
                        using (var image = System.Drawing.Image.FromStream(memoryStream))  // Load the image
                        {
                            if (image.Height != 80 || image.Width != 300) //|| image.Height == 80 || image.Height == 300
                        {
                                return ("Image dimensions must be between  300x80 pixels.", null);
                            }

                            // If everything is valid, return the byte array and a success message
                            return ("File uploaded successfully.", memoryStream.ToArray());
                        }
                    }
                }

                return ("No file uploaded.", null);
            }






    [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateViewModel viewModel)
        {
            try
            {

                var customerNumber = GenerateCustomerNumber();  

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

                if (viewModel.Addresses.Count == 1)
                {
                    foreach (var item in viewModel.Addresses)
                    {
                        if ((item.AddressName == null && item.ContactPerson == null && item.PhoneNumber == null))
                        {
                            addrss.Clear();
                        }
                        //else
                        //{
                        //    return Json(new { success = false, message = "Please Enter AddressName or ContactPerson or PhoneNumber" });
                        //}
                    }
                }
                else
                {
                    foreach (var item in viewModel.Addresses)
                    {

                        if ((item.AddressName == null || item.ContactPerson == null || item.PhoneNumber == null))
                        {
                            return Json(new { success = false, message = "Please Enter AddressName or ContactPerson or PhoneNumber" });
                        }

                        if (item.AddressName != null && item.ContactPerson != null && item.PhoneNumber != null)
                        {
                            addrss.Add(item);
                        }
                    }
                }


                var customerPhoto = "";


           

                var result = await SaveCustomerPhotoAsync(viewModel.CustomerPhoto, customerNumber);
                var jsonResult = result as JsonResult;

                if (jsonResult != null)
                {
                    var jsonData = jsonResult.Value as dynamic;
                    if (jsonData.success == true)
                    {
                        customerPhoto = jsonData.message;
                    }
                    else
                    {
                        return Json(new { success = false, message = jsonData.message });
                    }
                }


                byte[] sign = null;

                var (message, fileData) = await ConvertFileToByteArray(viewModel.CustomerSignature);

                
                if (message == "File uploaded successfully.")
                {
                    sign = fileData;
                }
                else
                {
                    Console.WriteLine(message);
                    return Json(new { success = false, message = message });
                    // Display the error message to the user
                    
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
                    CustomerNo = customerNumber,
                    CustomerName = viewModel.CustomerName,
                    CustomerAddress = viewModel.CustomerAddress,
                    BusinessStart = viewModel.BusinessStart,
                    CreditLimit = viewModel.CreditLimit,
                    CustomerTypeId = viewModel.CustomerTypeId,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    CustomerPhoto = customerPhoto,
                    CustomerSignature = sign,
                    AddressList = viewModel.Addresses.Select(a => new Address
                    {
                        ContactPerson = a.ContactPerson,
                        PhoneNumber = a.PhoneNumber,
                        AddressName = a.AddressName
                    }).ToList()
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                // Return success message
                return Json(new { success = true, message = "Customer created successfully!" });
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }



        [HttpPost]
        public async Task<JsonResult> CreateCustomerType(CustomerTypeCreateVM model)
        {
            if (string.IsNullOrWhiteSpace(model.CustomerTypeName))
            {
                return Json(new { success = false, message = "Name cannot be empty" });
            }

            var customerType = new CustomerType
            {
                CustomerTypeName = model.CustomerTypeName,
                CustomerTypeNo = GenerateCustomerTypeNumber(),
                ShortName = model.ShortName
            };

            _context.CustomerTypes.Add(customerType);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                customerTypeId = customerType.CustomerTypeId,
                customerTypeName = customerType.CustomerTypeName,
                customerTypeNo = customerType.CustomerTypeNo,
                shortName = customerType.ShortName

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
                ct.CustomerTypeNo,
                ct.CustomerTypeName,
                ct.ShortName // Include this field
            }).ToListAsync();
            return Ok(customerTypes); // Return JSON response
        }

        //[HttpGet]
        //public IActionResult GetCustomerTypeSuggestions(string term)
        //{
        //    var customerTypes = _context.CustomerTypes
        //        .Where(ct => ct.CustomerTypeName.Contains(term))
        //        .Select(ct => new
        //        {
        //            label = ct.CustomerTypeName,
        //            value = ct.CustomerTypeId
        //        })
        //        .ToList();

        //    return Json(customerTypes);
        //}



        [HttpPost]
        public async Task<IActionResult> UpdateCustomerType(int customerTypeId, string customerTypeName, string shortName)
        {
            if (string.IsNullOrWhiteSpace(customerTypeName))
            {
                return Json(new { success = false, message = "Customer Type Name is required." });
            }

            try
            {
                // Fetch the customer type by ID
                var customerType = await _context.CustomerTypes.FindAsync(customerTypeId);

                if (customerType == null)
                {
                    return Json(new { success = false, message = "Customer type not found." });
                }

                // Update fields
                customerType.CustomerTypeName = customerTypeName;
                customerType.ShortName = shortName;

                // Save changes to the database
                _context.CustomerTypes.Update(customerType);
                await _context.SaveChangesAsync();

                // Return success response
                return Json(new
                {
                    success = true,
                    customerTypeId = customerType.CustomerTypeId,
                    customerTypeName = customerType.CustomerTypeName,
                    customerTypeNo = customerType.CustomerTypeNo,
                    shortName = customerType.ShortName
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
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

        public string GenerateCustomerTypeNumber()
        {

            var lastCustomerTypeNumber = _context.CustomerTypes
                .OrderByDescending(c => c.CustomerTypeId)
                .Select(c => c.CustomerTypeNo)
                .FirstOrDefault();


            if (string.IsNullOrEmpty(lastCustomerTypeNumber))
            {
                return "001";
            }


            int nextNumber = int.Parse(lastCustomerTypeNumber) + 1;
            return nextNumber.ToString("D3");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.AddressList)
                .Include(c=> c.CustomerType)
                .FirstOrDefaultAsync(c => c.CustomersId == id);

            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.CustomerTypes = new SelectList(_context.CustomerTypes,
                "CustomerTypeId", "CustomerTypeName", customer.CustomerTypeId);
            ViewBag.CustomerTypeNumber = GenerateCustomerTypeNumber();

            ViewBag.CustomerTypeName = customer.CustomerType.CustomerTypeName;

            var viewModel = new CustomerCreateViewModel
            {
                CustomerNo = customer.CustomerNo,
                CustomerName = customer.CustomerName,
                CustomerAddress = customer.CustomerAddress,
                BusinessStart = customer.BusinessStart,
                CreditLimit = customer.CreditLimit,
                CustomerTypeId = customer.CustomerTypeId,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                CustomerPhotoLink = customer.CustomerPhoto,

               

                CustomerSignatureShow = customer.CustomerSignature,
                CustomerSignatureLink = customer.CustomerSignature != null ? "data:image/jpeg;base64," + Convert.ToBase64String(customer.CustomerSignature) : null,





                CustomerTypeName = customer.CustomerType.CustomerTypeName,
                Addresses = customer.AddressList
                    .Select(a => new AddressViewModel 
                    { 
                        ContactPerson = a.ContactPerson,
                        PhoneNumber = a.PhoneNumber,
                        AddressName = a.AddressName 
                    })
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

                if (item.AddressName == null || item.ContactPerson == null || item.PhoneNumber == null)
                {
                    return Json(new { success = false, message = "Please Enter AddressName or ContactPerson or PhoneNumber" });
                }

                if (item.AddressName != null && item.ContactPerson != null && item.PhoneNumber != null)
                {
                    addrss.Add(item);
                }
            }

            var customer = await _context.Customers
                .Include(c => c.AddressList)
                .FirstOrDefaultAsync(c => c.CustomersId == id);

            if (customer == null)
            {
                return Json(new { success = false, message = "Customer not found" });
            }



            if (viewModel.CustomerPhoto != null)
            {
               

                var result = await SaveCustomerPhotoAsync(viewModel.CustomerPhoto, viewModel.CustomerNo);
                var jsonResult = result as JsonResult;

                if (jsonResult != null)
                {
                    var jsonData = jsonResult.Value as dynamic;
                    if (jsonData.success == true)
                    {
                        customer.CustomerPhoto = jsonData.message;
                    }
                    else
                    {
                        return Json(new { success = false, message = jsonData.message });
                    }
                }

            }



            if (viewModel.CustomerSignature != null)
            {
                

                var (message, fileData) = await ConvertFileToByteArray(viewModel.CustomerSignature);


                if (message == "File uploaded successfully.")
                {
                    customer.CustomerSignature = fileData;
                }
                else
                {
                    Console.WriteLine(message);
                    return Json(new { success = false, message = message });
                    // Display the error message to the user

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

            

            // Update the customer details
            customer.CustomerName = viewModel.CustomerName;
            customer.CustomerAddress = viewModel.CustomerAddress;
            customer.BusinessStart = viewModel.BusinessStart;
            customer.CreditLimit = viewModel.CreditLimit;
            customer.CustomerTypeId = viewModel.CustomerTypeId;
            customer.Email = viewModel.Email;
            customer.PhoneNumber = viewModel.PhoneNumber;

            // Update addresses
            customer.AddressList.Clear();
            foreach (var address in viewModel.Addresses)
            { 
                customer.AddressList.Add(new Address 
                {
                    ContactPerson = address.ContactPerson,
                    PhoneNumber = address.PhoneNumber,
                    AddressName = address.AddressName 
                });
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
        [Route("Customers/Delete/{id}")]
        
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers.Include(c => c.AddressList).FirstOrDefault(c => c.CustomersId == id);
            if (customer == null)
            {
                return Json(new { success = false, message = "Customer not found." });
            }

             _context.Addresses.RemoveRange(customer.AddressList); // Only if you need to remove related addresses

            //_context.Addresses

            // Remove the customer
            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Json(new { success = true, message = "Customer deleted successfully." });
        }


    }
}
