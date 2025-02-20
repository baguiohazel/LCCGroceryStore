using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiWithRoleAuthentication.Data;
using WebApiWithRoleAuthentication.Models;

namespace LCCGroceryStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Orders (Includes OrderDetails & Customer) - Accessible to All Authenticated Users
        [HttpGet]
        [Authorize] // Any authenticated user can view orders
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Customers) // Include Customer details
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Products) // Include Product details
                .AsNoTracking()
                .ToListAsync();
        }

        // ✅ GET: api/Orders/5 (Includes OrderDetails & Customer)
        [HttpGet("{id}")]
        [Authorize] // Any authenticated user can view a specific order
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return order;
        }

        // ✅ PUT: api/Orders/5 (Restricted to Admin & Cashiers)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Cashier")] // Only Admin & Cashier can update orders
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest("Order ID mismatch.");
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExists(id))
                {
                    return NotFound("Order not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ POST: api/Orders (Create Order & Deduct Stock)
        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")] // Only Admin & Cashier can create orders
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (order.OrderDetails == null || order.OrderDetails.Count == 0)
            {
                return BadRequest("Order must have at least one OrderDetail.");
            }

            var existingCustomer = await _context.Customers.FindAsync(order.CustomerId);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            decimal totalAmount = 0;

            // Validate and deduct stock for each product in the order
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {orderDetail.ProductId} not found.");
                }

                if (product.StockQuantity < orderDetail.Quantity)
                {
                    return BadRequest($"Not enough stock for product {product.Name}. Available: {product.StockQuantity}");
                }

                // Deduct stock
                product.StockQuantity -= orderDetail.Quantity;
                _context.Entry(product).State = EntityState.Modified;

                // Calculate subtotal & total amount
                orderDetail.Subtotal = product.Price * orderDetail.Quantity;
                totalAmount += (decimal)orderDetail.Subtotal; // Cast to decimal
                orderDetail.Products = product;
            }

            // Assign total amount to order
            order.TotalAmount = (double)totalAmount;

            // Assign total amount to order
            order.TotalAmount = (double)totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // ✅ DELETE: api/Orders/5 (Restricted to Admin & Cashiers)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Cashier")] // Only Admin & Cashier can delete orders
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Include OrderDetails for cascading delete
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // ❌ Prevent deletion if order is already processed
            if (order.OrderDetails.Any(od => od.Products.StockQuantity < od.Quantity))
            {
                return BadRequest("Cannot delete an order that has already affected stock levels.");
            }

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> OrderExists(int id)
        {
            return await _context.Orders.AnyAsync(e => e.Id == id);
        }
    }
}
