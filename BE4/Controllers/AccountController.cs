using BE4.Models;
using BE4.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BE4.Controllers
{
    public class AccountController : Controller
    {
        private MyStore1Entities db = new MyStore1Entities();
       //Đăng kí
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username
                && u.Password == model.Password
                && u.UserRole == "Customer");
                if (user ==// ) //// thanh đổi giá trị 
                {
                    //lưu trạng thái đăng nhập 
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;
                    // lưu thông tin xác nhận ng dùng
                    FormsAuthentication.SetAuthCookie(user.Username, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên dăng nhập hoặc mật khẩu  không đúng ");
                }
            }
            return View(model);
        }
        public ActionResult LoginOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        //Đăng kí
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                //kiểm tra tên đăng nhập tồn tại chưa 
                var exsistingUser = db.Customers.SingleOrDefault(u => u.Username == model.Username);
                if (exsistingUser == null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }
                //nếu chưa  tạo bản ghi  thông tin tk 
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "Customer"
                };
                db.Users.Add(user);
                //tạo bản ghi thông tin KH
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}