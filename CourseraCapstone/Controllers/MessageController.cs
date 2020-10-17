using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CourseraCapstone.Data;
using CourseraCapstone.Models;
using CourseraCapstone.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CourseraCapstone.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMsgRepository msgRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger<MessageController> logger;
        private readonly MyConfiguration myConfiguration;

        public MessageController(IMsgRepository msgRepository, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, ILogger<MessageController> logger, MyConfiguration myConfiguration)
        {
            this.msgRepository = msgRepository;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.myConfiguration = myConfiguration;
        }

        public IActionResult Index()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            var model = msgRepository.GetAllMyMessages(userId);
            return View(model);
        }
        public ViewResult Details(int? id)
        {
            if(!id.HasValue)            
                id = 1;
            
            Msg msg = msgRepository.GetMsg(id.Value);
            if (msg == null)
            {
                return View("~/Views/Account/AccessDenied.cshtml");
            }
            else
            {
                var userId = userManager.GetUserId(HttpContext.User);
                if (msg.RecieverId == userId)
                {
                    Msg newMsg = msg;
                    newMsg.Content = Decrypt(msg.Content);
                    return View(newMsg);
                }
                else
                {
                    return View("~/Views/Account/AccessDenied.cshtml");
                }
            }
        }
        [HttpGet]
        public ViewResult Send()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(HttpContext.User);
                var recieverId = await userManager.FindByNameAsync(model.UserName);
                if (recieverId != null)
                {
                    Msg newMsg = new Msg()
                    {
                        SenderId = userId,
                        Content = Encrypt(model.Content),
                        RecieverId = recieverId.Id
                    };
                    msgRepository.Add(newMsg);

                    ViewBag.SuccessTitle = "Send Confirmed";
                    ViewBag.SuccessMessage = "Your message has been sent.";
                    return View("Success");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Invalid Username");
                }
            }
            return View();
        }
        private string Encrypt(string clearText)
        {
            string EncryptionKey = myConfiguration.Key;
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        private string Decrypt(string cipherText)
        {
            string EncryptionKey = myConfiguration.Key;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
