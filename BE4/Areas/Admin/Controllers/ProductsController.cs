using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE4.Models;
using BE4.Models.ViewModel;
using PagedList;
namespace BE4.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStore1Entities db = new MyStore1Entities();

        // GET: Admin/Products
        public ActionResult Index(string SearchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(SearchTerm))
            {//tìm kiếm sản phẩm dựa trên từ khóa 
                products = products.Where(p =>
                p.ProductName.Contains(SearchTerm) ||
                p.ProductDescription.Contains(SearchTerm) ||
                p.Category.CategoryName.Contains(SearchTerm)
                );
            }
            //Tìm kiếm sản phẩm dựa trên giá trị tối thiểu
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice.Value;
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }
            //Tìm kiếm sản phẩm dựa trên giá tối đa 
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }
            //Áp dụng sắp xếp dựa trên lựa chọn của người 
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default: //Mặc đinh sắp xếp theo tên
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOder = sortOrder;
            //Đoạn code liên quan tới phân trang 
            //Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị )
            int pageNumber = page ?? 1;
            int pageSize = 2; //Số sản phẩm mỗi trang

            //đóng câu lệnh này, sử dụng ToPageList để lấy danh sách đã phân trang
            //model.Products = products.ToList();

            model.Products = products.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
