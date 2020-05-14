using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PBMS.Data;
using PBMS.Models;

namespace PBMS.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ContactsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contacts.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }      

        public JsonResult IsNumberExist(string number)
        {
            bool result = true;

            if (_context.Contacts.Any(c => c.Number == number))
            {
                result = false;
            }
            return Json(result);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Email,Number,Occupation")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }


        public ActionResult InsertBulkDatafromExcelfile()
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

                    for(int i = 0; i < cellCount; i++)
                    {
                        var cellName = headerRow.GetCell(i).ToString();

                        if(cellName == "Number")
                        {
                            List<Contact> contacts = new List<Contact>();

                            for (int j = (sheet.FirstRowNum + 1); j <= sheet.LastRowNum; j++) //Read Excel File
                            {
                                IRow row = sheet.GetRow(j);

                                if (row == null) continue;

                                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                                if (row.GetCell(i) == null || row.GetCell(i).ToString() == "") continue;
                                
                                if(!_context.Contacts.Any(c => c.Number == row.GetCell(i).ToString()))
                                {
                                    Contact contact = new Contact
                                    {                                        
                                        Number = row.GetCell(i).ToString()
                                    };

                                    for(int k = 0; k < cellCount; k++)
                                    {
                                        var celName = headerRow.GetCell(k).ToString();

                                        if(celName == "Name")
                                        {
                                            contact.Name = row.GetCell(k).ToString();
                                        }
                                        else if(celName == "Address")
                                        {
                                            contact.Address = row.GetCell(k).ToString();
                                        }
                                        else if(celName == "Email")
                                        {
                                            contact.Email = row.GetCell(k).ToString();
                                        }
                                        else if (celName == "Occupation")
                                        {
                                            contact.Occupation = row.GetCell(k).ToString();
                                        }                                       
                                    }

                                    contacts.Add(contact);
                                }                                                           
                            }

                            _context.BulkInsert(contacts);

                            sb.Append("<h5> Insertion Successfull!</h5>");
                        }
                    }                                      
                }
            }

            return Content(sb.ToString());
        }


        //public ActionResult InsertBulkDatafromExcelfile()
        //{
        //    IFormFile file = Request.Form.Files[0];
        //    string folderName = "UploadExcel";
        //    string webRootPath = _hostingEnvironment.WebRootPath;
        //    string newPath = Path.Combine(webRootPath, folderName);
        //    StringBuilder sb = new StringBuilder();

        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }

        //    if (file.Length > 0)
        //    {
        //        string sFileExtension = Path.GetExtension(file.FileName).ToLower();
        //        ISheet sheet;
        //        string fullPath = Path.Combine(newPath, file.FileName);

        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //            stream.Position = 0;

        //            if (sFileExtension == ".xls")
        //            {
        //                HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
        //                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
        //            }
        //            else
        //            {
        //                XSSFWorkbook xssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
        //                sheet = xssfwb.GetSheetAt(0); //get first sheet from workbook   
        //            }

        //            IRow headerRow = sheet.GetRow(0); //Get Header Row
        //            int cellCount = headerRow.LastCellNum;

        //            List<Contact> contacts = new List<Contact>();                    

        //            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //            {
        //                IRow row = sheet.GetRow(i);

        //                if (row == null) continue;

        //                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

        //                var firstCell = row.FirstCellNum;

        //                Contact contact = new Contact
        //                {
        //                    Name = row.GetCell(firstCell).ToString(),
        //                    Address = row.GetCell(firstCell + 1).ToString(),
        //                    Email = row.GetCell(firstCell + 2).ToString(),                          
        //                    Number = row.GetCell(firstCell + 3).ToString(),
        //                    Occupation = row.GetCell(firstCell + 4).ToString()
        //                };

        //                contacts.Add(contact);
        //            }

        //            _context.BulkInsert(contacts);

        //            sb.Append("<h5> Insertion Successfull!</h5>");
        //        }
        //    }

        //    return Content(sb.ToString());
        //}


        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.SingleOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Email,Number,Occupation")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.SingleOrDefaultAsync(m => m.Id == id);
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
