using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE4.Models.ViewModel
{
    public class ProductDetailVM
    {
       public Product product {  get; set; }
        public int quantity { get; set; } = 1;
        public decimal estimatedValue => quantity * product.ProductPrice;
        public int PageNumber{ get; set; }
        public int PageSize{ get; set; }
        public PagedList.IPagedList<Product> RelatedProducts { get; set; }
        public PagedList.IPagedList<Product> TopProducts { get; set; }

    }
}