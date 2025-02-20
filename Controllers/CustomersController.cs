using System.Collections.Generic;
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
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Customers (Accessible to All Authenticated Users)
        [HttpGet]
        [Authorize] // Any authenticated user can view customers
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        // ✅ GET: api/Customers/5 (Accessible to All Authenticated Users)
        [HttpGet("{id}")]
        [Authorize] // Any authenticated user can view customer details
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            return customer;
        }

        // ✅ PUT: api/Customers/5 (Restricted to Admin & Cashiers)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Cashier")] // Only Admin & Cashier can update customer details
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest("Customer ID mismatch.");
            }

            if (customer.LoyaltyPoints < 0)
            {
                return BadRequest("Loyalty points cannot be negative.");
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(id))
                {
                    return NotFound("Customer not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ POST: api/Customers (Restricted to Admin & Cashiers)
        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")] // Only Admin & Cashier can add customers
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (customer == null || string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Contact))
            {
                return BadRequest("Invalid customer data.");
            }

            if (customer.LoyaltyPoints < 0)
            {
                return BadRequest("Loyalty points cannot be negative.");
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // ✅ DELETE: api/Customers/5 (Restricted to Admin & Cashiers)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Cashier")] // Only Admin & Cashier can delete customers
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // ❌ Prevent deletion if customer has orders
            if (customer.Orders != null && customer.Orders.Count > 0)
            {
                return BadRequest("Cannot delete a customer with existing orders.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CustomerExists(int id)
        {
            return await _context.Customers.AnyAsync(e => e.Id == id);
        }
    }
}
