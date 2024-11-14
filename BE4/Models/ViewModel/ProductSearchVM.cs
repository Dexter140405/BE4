using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE4.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }
        //tiêu chí để search theo tên, mô tả sp
        //hoặc loại sản phẩm

        //các tiêu chí để search theo giá
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        //Thứ tự sắp xếp 
        public string SortOder { get; set; }

        //Các thuộc tính hỗ trợ phân trang 
        public int PageNumber { get; set; }//Trang hiện tại 
        public int PageSize { get; set; } = 10;//Số sản phẩm mỗi trang

        //Danh sách sản phẩm đã phân trang 
        public PagedList.IPagedList<Product> Products { get; set; }

        ////danh sách sản phẩm thỏa điều kiện tìm kiếm
        //public List<Product> Products { get; set; }
    }
}