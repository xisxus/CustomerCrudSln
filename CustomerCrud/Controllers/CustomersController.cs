
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
using DocumentFormat.OpenXml.Packaging;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


using QuestPDF.Helpers;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Vml.Office;



namespace CustomerCrud.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string sortField = "CustomerName", string sortOrder = "asc", string selectedIds = "")
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
                    BusinessStart = c.BusinessStart,
                    CustomerTypeName = c.CustomerType.CustomerTypeName,
                    CustomerAddress = c.CustomerAddress,
                    CreditLimit = c.CreditLimit,
                    AdditionalAddressesCount = c.AddressList.Count,
                    IsSelected = selectedCustomerIds.Contains(c.CustomersId),

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
                        page.Size(8.27f, 11.69f, Unit.Inch);
                        page.Margin(1, Unit.Inch);
                        page.Header().Text("Customer Report").Bold().FontSize(20).AlignCenter();

                        page.Content().Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Add Headers
                            table.Header(header =>
                            {
                                header.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5) // Add padding to the header
                                    .AlignCenter() // Center-align the text
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
                            });

                            // Add Rows
                            foreach (var customer in customers)
                            {
                                table.Cell()
                                    .Border(1).BorderColor(Colors.Black)
                                    .Padding(5) // Add padding to rows
                                    .AlignCenter() // Center-align the text
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
            worksheet.Cells[1, 1, 1, 6].Merge = true; // Merge cells for the title
            worksheet.Cells[1, 1].Style.Font.Size = 16; // Title font size
            worksheet.Cells[1, 1].Style.Font.Bold = true; // Title bold
            worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center align title
            worksheet.Cells[1, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Fill pattern
            worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Title background color

            // Add headers
            worksheet.Cells[2, 1].Value = "Customer ID";
            worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 2].Value = "Name";
            worksheet.Cells[2, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 3].Value = "Type";
            worksheet.Cells[2, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 4].Value = "Address";
            worksheet.Cells[2, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 5].Value = "Business Start";
            worksheet.Cells[2, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 6].Value = "Credit Limit";
            worksheet.Cells[2, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            // Style headers
            using (var headerRange = worksheet.Cells[2, 1, 2, 6])
            {
                headerRange.Style.Font.Bold = true; // Bold headers
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Fill pattern
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Header background color
                headerRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black); // Border around headers
            }

            // Add data
            for (int i = 0; i < customers.Count; i++)
            {
                worksheet.Cells[i + 3, 1].Value = customers[i].CustomerNo;
                worksheet.Cells[i + 3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[i + 3, 2].Value = customers[i].CustomerName;
                worksheet.Cells[i + 3, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left; // Left align Name

                worksheet.Cells[i + 3, 3].Value = customers[i].CustomerType?.CustomerTypeName ?? "N/A";
                worksheet.Cells[i + 3, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center align Type

                worksheet.Cells[i + 3, 4].Value = customers[i].CustomerAddress ?? "N/A";
                worksheet.Cells[i + 3, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left; // Left align Address

                worksheet.Cells[i + 3, 5].Value = customers[i].BusinessStart.ToString("dd-MM-yyyy") ?? "N/A";
                worksheet.Cells[i + 3, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center align Business Start

                worksheet.Cells[i + 3, 6].Value = customers[i].CreditLimit == 0 ? "0" : customers[i].CreditLimit.ToString("N0");
                worksheet.Cells[i + 3, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right; // Right align Credit Limit
            }

            // Add borders to all cells with data
            var dataRange = worksheet.Cells[2, 1, customers.Count + 2, 6];
            dataRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black); // Border around all data

            // Add borders to each cell in the data range
            for (int row = 2; row <= customers.Count + 2; row++)
            {
                for (int col = 1; col <= 6; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }
            }

            // Add border to the title
            worksheet.Cells[1, 1, 1, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black); // Border around the title

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
            var wordDocument = WordprocessingDocument.Create(memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);

            // Add main document part
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            var body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());

            // Add Title
            var title = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                new DocumentFormat.OpenXml.Wordprocessing.Run(
                    new DocumentFormat.OpenXml.Wordprocessing.Text("Customer Report"))
                {
                    RunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties
                    {
                        Bold = new DocumentFormat.OpenXml.Wordprocessing.Bold(),
                        FontSize = new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = "28" }
                    }
                });
            body.Append(title);

            // Create a table
            var table = new DocumentFormat.OpenXml.Wordprocessing.Table();

            // Set table properties for borders
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

            // Add headers
            var headerRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
            headerRow.Append(
                CreateHeaderCell("Customer ID"),
                CreateHeaderCell("Name"),
                CreateHeaderCell("Type"),
                CreateHeaderCell("Address"),
                CreateHeaderCell("Business Start"),
                CreateHeaderCell("Credit Limit")
            );
            table.Append(headerRow);

            // Add customer data
            foreach (var customer in customers)
            {
                var row = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                row.Append(
                    CreateTableCell(customer.CustomerNo),
                    CreateTableCell(customer.CustomerName),
                    CreateTableCell(customer.CustomerType?.CustomerTypeName ?? "N/A"),
                    CreateTableCell(customer.CustomerAddress ?? "N/A"),
                    CreateTableCell(customer.BusinessStart.ToString("dd-MM-yyyy") ?? "N/A"),
                    //CreateTableCell(customer.CreditLimit.ToString() ?? "0")
                    CreateTableCell(customer.CreditLimit == 0 ? "0" : customer.CreditLimit.ToString("N0"))

                //CreateTableCell(customer.CustomerNo, AlignmentValues.Right), // Right-align Customer No
                //CreateTableCell(customer.CustomerName, AlignmentValues.Left), // Left-align Name
                //CreateTableCell(customer.CustomerType?.CustomerTypeName ?? "N/A", AlignmentValues.Center), // Center-align Type
                //CreateTableCell(customer.CustomerAddress ?? "N/A", AlignmentValues.Left), // Left-align Address
                //CreateTableCell(customer.BusinessStart.ToString("dd-MM-yyyy") ?? "N/A", AlignmentValues.Center), // Center-align Business Start
                //CreateTableCell(customer.CreditLimit.ToString() ?? "N/A", AlignmentValues.Right) // Right-align Credit Limit
                );
                table.Append(row);
            }


            //DocumentFormat.OpenXml.Wordprocessing.TableCell CreateTableCell(string text, AlignmentValues alignment)
            //{
            //    return new DocumentFormat.OpenXml.Wordprocessing.TableCell(
            //        new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties(
            //            new DocumentFormat.OpenXml.Wordprocessing.TableCellWidth { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa, Width = "2400" },
            //            new DocumentFormat.OpenXml.Wordprocessing.TableCellMargin(
            //                new DocumentFormat.OpenXml.Wordprocessing.TopMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
            //                new DocumentFormat.OpenXml.Wordprocessing.BottomMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
            //                new DocumentFormat.OpenXml.Wordprocessing.LeftMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
            //                new DocumentFormat.OpenXml.Wordprocessing.RightMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa }
            //            )
            //        ),
            //        new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
            //            new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(
            //                new DocumentFormat.OpenXml.Wordprocessing.Justification { Val = alignment }
            //            ),
            //            new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(text))
            //        )
            //    );
            //}


            // Helper methods
            DocumentFormat.OpenXml.Wordprocessing.TableCell CreateTableCell(string text)
            {
                return new DocumentFormat.OpenXml.Wordprocessing.TableCell(
                    new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties(
                        new DocumentFormat.OpenXml.Wordprocessing.TableCellWidth { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa, Width = "2400" },
                        new DocumentFormat.OpenXml.Wordprocessing.TableCellMargin(
                            new DocumentFormat.OpenXml.Wordprocessing.TopMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
                            new DocumentFormat.OpenXml.Wordprocessing.BottomMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
                            new DocumentFormat.OpenXml.Wordprocessing.LeftMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
                            new DocumentFormat.OpenXml.Wordprocessing.RightMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa }
                        )
                    ),
                    new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(text))
                    )
                );
            }

            DocumentFormat.OpenXml.Wordprocessing.TableCell CreateHeaderCell(string text)
            {
                return new DocumentFormat.OpenXml.Wordprocessing.TableCell(
                    new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties(
                        new DocumentFormat.OpenXml.Wordprocessing.TableCellWidth { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa, Width = "2400" },
                        new DocumentFormat.OpenXml.Wordprocessing.Shading
                        {
                            Fill = "CCCCCC" // Light gray background
                        },
                        new DocumentFormat.OpenXml.Wordprocessing.TableCellMargin(
                            new DocumentFormat.OpenXml.Wordprocessing.TopMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
                            new DocumentFormat.OpenXml.Wordprocessing.BottomMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
                            new DocumentFormat.OpenXml.Wordprocessing.LeftMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa },
                            new DocumentFormat.OpenXml.Wordprocessing.RightMargin { Width = "100", Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa }
                        )
                    ),
                    new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(
                            new DocumentFormat.OpenXml.Wordprocessing.Text(text) {   }
                        )
                    )
                );
            }

            body.Append(table);
            wordDocument.Clone();
          //  wordDocument.Close();

            return memoryStream.ToArray();
        }

        private DocumentFormat.OpenXml.Wordprocessing.TableCell CreateTableCell(string text)
        {
            return new DocumentFormat.OpenXml.Wordprocessing.TableCell(
                new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                        new DocumentFormat.OpenXml.Wordprocessing.Text(text))));
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


        [HttpPost]
        
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomersId == id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
