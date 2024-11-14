using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE4.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items => items;
        // Thêm sản phẩn vào giỏ
        public void AddItem(int productId, string productImage, string productName, decimal unitPrice, int quantity, string category)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == productId);
            if (existingItem != null)
            {
                items.Add(new CartItem
                {
                    ProductID = productId,
                    ProductImage = productImage,
                    ProductName = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }
            else { existingItem.Quantity += quantity; }
        }
        //Xóa sản phẩm ra khỏi giỏ hàng 
        public void RemoveItem(int productId) { items.RemoveAll(i => i.ProductID == productId); }
        //Tinh tổng giỏ hàng 
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }
        //làm trống giở hàng 
        public void Clean()
        {
            items.Clear();
        }
        //cập nhật  số lượng sản phẩm đã chọn
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            { item.Quantity = quantity; }

        }
    }
}