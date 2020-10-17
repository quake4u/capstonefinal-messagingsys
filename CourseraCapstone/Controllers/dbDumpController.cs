using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseraCapstone.Data;
using DbDumpClassLibrary;
using Microsoft.AspNetCore.Mvc;

namespace CourseraCapstone.Controllers
{
    public class dbDumpController : Controller
    {
        private readonly MyConfiguration _myConfiguration;

        public dbDumpController(MyConfiguration myConfiguration)
        {
            _myConfiguration = myConfiguration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dump()
        {
            string conn = _myConfiguration.CS;
            string path = _myConfiguration.Path;

            exportDB db = new exportDB(conn, path);
            db.doThat();
            return RedirectToAction("DownloadFile");
        }
        public FileResult DownloadFile()
        {
            string path = _myConfiguration.Path;
            return PhysicalFile(path, "text/plain", "DB.txt");
        }
    }
}
