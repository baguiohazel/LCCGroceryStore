namespace WebApiWithRoleAuthentication.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int StockQuantity { get; set; }  
        public int ReorderLevel { get; set; }
        public string? Barcode { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public Category? Categories { get; set; }
        public Supplier? Suppliers { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
