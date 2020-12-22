using BeeNetworkTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace BeeNetworkTest.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public new User User { get; set; }
        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            User = new User();
            if (id == null)
            {
                //create
                return View(User);
            }
            //update
            User = _db.Users.FirstOrDefault(u => u.Id == id);
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }
        public int Countall()
        {
            var result = _db.Users.Count();

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (User.Id == 0)
                {
                    //create
                    _db.Users.Add(User);
                }
                else
                {
                    _db.Users.Update(User);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(User);
        }
        [HttpPost]
        public IActionResult Login(User model)
        {

            // Normally Identity handles sign in, but you can do it directly
            if (ModelState.IsValid)
            {
                var result = (from e in _db.Users
                              where (e.Username == model.Username) && (e.Password == model.Password)
                              select e).FirstOrDefault();

                if (result != null)
                {
                    HttpContext.Session.SetString("username", model.Username);
                    return View("index");
                }
            }
            ModelState.AddModelError("", "Invalid login attempt");

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Home");
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allUsers = await _db.Users.ToListAsync();
            return Json(new { data = allUsers, countUser = allUsers.Count });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var userFromDb = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.Users.Remove(userFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
