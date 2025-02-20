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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Products (Include Category) - Accessible to All Authenticated Users
        [HttpGet]
        [Authorize] // Any authenticated user can view products
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Categories) // Include Category details
                .AsNoTracking()
                .ToListAsync();
        }

        // ✅ GET: api/Products/5 (Include Category)
        [HttpGet("{id}")]
        [Authorize] // Any authenticated user can view a specific product
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Categories)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return product;
        }

        // ✅ PUT: api/Products/5 (Restricted to Admin & Inventory Manager)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,InventoryManager")] // Only Admin & Inventory Manager can update products
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            // Ensure the category exists before updating
            var existingCategory = await _context.Categories.FindAsync(product.CategoryId);
            if (existingCategory == null)
            {
                return BadRequest("Category not found.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(id))
                {
                    return NotFound("Product not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ POST: api/Products (Restricted to Admin & Inventory Manager)
        [HttpPost]
        [Authorize(Roles = "Admin,InventoryManager")] // Only Admin & Inventory Manager can create products
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Validate that the category exists before creating the product
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // ✅ DELETE: api/Products/5 (Restricted to Admin & Inventory Manager)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,InventoryManager")] // Only Admin & Inventory Manager can delete products
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.OrderDetails) // Include OrderDetails to check dependencies
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // ❌ Prevent deletion if product is linked to any orders
            if (product.OrderDetails != null && product.OrderDetails.Count > 0)
            {
                return BadRequest("Cannot delete this product because it is linked to existing orders.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}
