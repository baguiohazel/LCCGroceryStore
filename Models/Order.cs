namespace WebApiWithRoleAuthentication.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public double TotalAmount { get; set; }
        public Customer? Customers { get; set; }
        public UserRole? Users { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
