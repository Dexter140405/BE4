using BE4.Models.ViewModel;
using BE4.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BE4.Controllers
{
    public class HomeController : Controller
    {
        private MyStore1Entities db = new MyStore1Entities();
        public ActionResult Index(string searchTerm, int? page)
        {
            //var products = db.Products.Include(p => p.Category);
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            //Tìm  kiếm sản phẩm dựa trên từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                           p.ProductName.Contains(searchTerm) ||
                           p.ProductDescription.Contains(searchTerm) ||
                           p.Category.CategoryName.Contains(searchTerm));
            }
            //Đoạn code liên quan tới phân trang 
            //Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = 6; //Số sản phẩm mỗi trang 

            //Lấy top 10 sản phẩm bán chạy nhất 
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();

            //Lấy 20 sản phẩm bán ế nhất và phân trang 
            model.NewProducts = products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        //GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            //Lấy tất cả các sản phẩm cùng danh mục 
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            ProductDetailVM model = new ProductDetailVM();

            //Đoạn code liên quan tới phân trang
            //Lấy số trang hiện tại (mặc địn là trang 1 nếu không có giá trị )
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }
            return View(model);
        
        } 
        
    }

}
