using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PBMS.Models;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace PBMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult TestAdminDashboard()
        {
            return View();
        }

        public IActionResult TestDashboard()
        {
            return View();
        }

        public ActionResult Import()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook xssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = xssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    sb.Append("<table class='table table-bordered'><tr>");

                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = headerRow.GetCell(j);

                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                        sb.Append("<th>" + cell.ToString() + "</th>");
                    }

                    sb.Append("</tr>");
                    sb.AppendLine("<tr>");

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);

                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)

                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");

                        }

                        sb.AppendLine("</tr>");
                    }

                    sb.Append("</table>");
                }
            }

            return Content(sb.ToString());
        }


        public async Task<IActionResult> Export()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"Contacts.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("contact");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("Id");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Address");
                row.CreateCell(3).SetCellValue("Email");
                row.CreateCell(4).SetCellValue("Number");
                row.CreateCell(5).SetCellValue("Occupation");

                row = excelSheet.CreateRow(1);
                row.CreateCell(0).SetCellValue(1);
                row.CreateCell(1).SetCellValue("Jack Supreu");
                row.CreateCell(2).SetCellValue("HollyWood");
                row.CreateCell(3).SetCellValue("jacksparrow@gmail.com");
                row.CreateCell(4).SetCellValue("01700000001");
                row.CreateCell(5).SetCellValue("Occupation2");

                row = excelSheet.CreateRow(2);
                row.CreateCell(0).SetCellValue(2);
                row.CreateCell(1).SetCellValue("Steve khan");
                row.CreateCell(2).SetCellValue("HollyWood");
                row.CreateCell(3).SetCellValue("stevekhan@gmail.com");
                row.CreateCell(4).SetCellValue("01700000002");
                row.CreateCell(5).SetCellValue("Occupation3");

                row = excelSheet.CreateRow(3);
                row.CreateCell(0).SetCellValue(3);
                row.CreateCell(1).SetCellValue("Romi gill");
                row.CreateCell(2).SetCellValue("HollyWood");
                row.CreateCell(3).SetCellValue("romigill@gmail.com");
                row.CreateCell(4).SetCellValue("01700000003");
                row.CreateCell(5).SetCellValue("Occupation4");

                row = excelSheet.CreateRow(4);
                row.CreateCell(0).SetCellValue(4);
                row.CreateCell(1).SetCellValue("Hider Ali");
                row.CreateCell(2).SetCellValue("HollyWood");
                row.CreateCell(3).SetCellValue("hiderali@gmail.com");
                row.CreateCell(4).SetCellValue("01700000004");
                row.CreateCell(5).SetCellValue("Occupation5");               

                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        public ActionResult Download()
        {
            string Files = "wwwroot/UploadExcel/Contacts.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
            System.IO.File.WriteAllBytes(Files, fileBytes);
            MemoryStream ms = new MemoryStream(fileBytes);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "employee.xlsx");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
