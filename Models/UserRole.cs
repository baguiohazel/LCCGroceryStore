namespace WebApiWithRoleAuthentication.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public ICollection<Order>? Orders { get; set; }
    }
}
