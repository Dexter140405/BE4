using BE4.Models.ViewModel;
using BE4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BE4.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        private MyStore1Entities db = new MyStore1Entities();
        public ActionResult CheckoutVM()
        {
            return View();
        }

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //Kiểm tra giỏ hàng trong sesion,
            //nếu giỏ hàng hoặc không có sản phẩm thì chuyển hướng về trang chủ 
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //Xác thực người dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới trang Đăng Nhập\
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) { return RedirectToAction("Login", "Account"); }
            //Lấy thông tin khách hàng từ CSDL, nếu chưa có thì chuyển hướng tới trang đăng nhập
            //nếu có rồi thì lấy địa chỉ của khách hàng và gắn vào ShippingAdress của CheckoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null) { return RedirectToAction("Login", "Account"); }

            var model = new CheckoutVM //Tạo dữ liệu hiển thị cho CheckoutVM
            {
                CartItems = cart, //Lấy danh sách các sản phẩm trong giỏ hàng 
                TotalAmount = (decimal)cart.Sum(item => item.TotalPrice), //Tổng giá trị của các mặt hàng trong giỏ hàng
                OrderDate = DateTime.Now,//Mặc định lấy bằng thời điểm đặt hàng 
                ShippingAddress = customer.CustomerAddress, // Lấy địa chỉ mặc định từ bảng Customer
                CustomerID = customer.CustomerID,//Lấy mã khách hàng từ bảng Customer
                Username = customer.Username//Lấy tên đăng nhập từ bảng Customer
            };
            return View(model);
        }
        //POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as List<CartItem>;
                //Nếu giỏ hàng rỗng sẽ điều hướng tới trang Home
                if (cart == null || !cart.Any()) { return RedirectToAction("Index", "Home"); }
                //Nếu người dùng chưa đăng nhập sẽ điều hướng tới trang Login
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null) { return RedirectToAction("Login", "Account"); }
                //Nếu khách hàng không khớp với tên đăng nhập, sẽ điều hướng tới trang Login
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }
                //Nếu người dùng chọn thanh toán bằng Paypal, sẽ điều hướng tới trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {
                    return RedirectToAction("PaymentWithPaypal", "PayPal", model);
                }
                //Thiết lập PaymetStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }
                //Tạo đơn hàng và chi tiết đơn hàng liên 
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetails = cart.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };
                //Lưu đơn hàng vào CSDL
                db.Orders.Add(order);
                db.SaveChanges();
                //Xóa giỏ hàng sau khi đặt hàng thành công
                Session["Cart"] = null;
                //Điều hướng tới trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }
    }
}