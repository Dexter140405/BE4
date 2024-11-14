using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BE4.Models.ViewModel;
using BE4.Models;

namespace BE4.Controllers
{
    public class CartController : Controller
    {
        private MyStore1Entities db = new MyStore1Entities();

        // Hàm lấy dịch vụ giỏ hàng
        public CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: Hiển thị giỏ hàng không gom nhóm theo danh mục
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();  // Lấy giỏ hàng từ dịch vụ

            // Trả về View với dữ liệu giỏ hàng
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);  // Tìm sản phẩm theo ID

            if (product != null)  // Kiểm tra xem sản phẩm có tồn tại không
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(
                    product.ProductID,
                    product.ProductImage,
                    product.ProductName,
                    product.ProductPrice,
                    quantity,
                    product.Category.CategoryName);
            }

            return RedirectToAction("Index");  // Sau khi thêm, chuyển đến trang giỏ hàng
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);  // Xóa sản phẩm theo ID

            return RedirectToAction("Index");  // Sau khi xóa, chuyển đến trang giỏ hàng
        }

        // Làm trống giỏ hàng
        public ActionResult ClearCart()
        {
            var cartService = GetCartService();
            cartService.ClearCart();  // Xóa tất cả sản phẩm trong giỏ

            return RedirectToAction("Index");  // Quay lại giỏ hàng
        }

        // Cập nhật số lượng sản phẩm trong giỏ
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)  // Kiểm tra nếu số lượng <= 0 thì không cập nhật
            {
                // Có thể trả về thông báo lỗi hoặc chỉ không làm gì
                return RedirectToAction("Index");
            }

            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);  // Cập nhật số lượng sản phẩm trong giỏ

            return RedirectToAction("Index");  // Sau khi cập nhật, quay lại trang giỏ hàng
        }
    }
}
