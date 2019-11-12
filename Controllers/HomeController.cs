using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;


namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult RegisterPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User newuser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newuser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("RegisterPage");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newuser.Password = Hasher.HashPassword(newuser, newuser.Password);
                dbContext.Add(newuser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("user_id", newuser.UserId);
                return RedirectToAction("AccountPage");
                }
            return View("RegisterPage"); 
        }
        [HttpGet("account")]
        public IActionResult AccountPage()
        {
            if (HttpContext.Session.GetInt32("user_id") != null)
            {
                int userid = (int)HttpContext.Session.GetInt32("user_id");
                User theuser = dbContext.Users.Where(user => user.UserId == userid).Include(u => u.TransactionsMade).FirstOrDefault();
                double balance = 0;
                foreach (Transaction transaction in theuser.TransactionsMade)
                {
                    balance += transaction.Amount;
                }
                UserAndTransactionViewModel ViewModel = new UserAndTransactionViewModel()
                {
                    TheUser = dbContext.Users.Where(user => user.UserId == userid).Include(u => u.TransactionsMade).FirstOrDefault(),
                    Balance = balance

                };
                
                Console.WriteLine($"****************WORKING --> {ViewModel.Balance}*****************");
                return View(ViewModel);
            }
            return View("RegisterPage");
        }
        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginCredentials loggeduser)
        {
            if (ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == loggeduser.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("EmailAddress", "Invalid Email/Password");
                    return View("LoginPage");
                }
                var hasher = new PasswordHasher<LoginCredentials>();
                var result = hasher.VerifyHashedPassword(loggeduser, userInDb.Password, loggeduser.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("LoginPage");
                }
                HttpContext.Session.SetInt32("user_id", userInDb.UserId);
                int user_id = (int)HttpContext.Session.GetInt32("user_id");
                return RedirectToAction("AccountPage");
            }
            return View("LoginPage");
        }
        [HttpPost]
        public IActionResult MakeTransaction(UserAndTransactionViewModel modelData)
        {
            if(ModelState.IsValid)
            {
            Transaction newtransaction = modelData.TheTransaction;
            Console.WriteLine(ModelState.ErrorCount);
            Console.WriteLine(JsonConvert.SerializeObject(ModelState.Keys));
            Console.WriteLine(JsonConvert.SerializeObject(ModelState.Values));
            Console.WriteLine(ModelState.ValidationState);
                Console.WriteLine($"**************THIS IS THE USERID****************{newtransaction.UserId}********************************");
                newtransaction.UserId = (int)HttpContext.Session.GetInt32("user_id");

                dbContext.Add(newtransaction);
                dbContext.SaveChanges();
                return RedirectToAction("AccountPage");
            }
            int userid = (int)HttpContext.Session.GetInt32("user_id");
            modelData.TheUser = dbContext.Users.Where(user => user.UserId == userid).Include(u => u.TransactionsMade).FirstOrDefault();

            return View("AccountPage",modelData);
            

            
                
                  
            
            
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("RegisterPage");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
