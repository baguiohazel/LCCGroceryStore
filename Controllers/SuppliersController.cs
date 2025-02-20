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
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Suppliers (Includes Products) - Accessible to All Authenticated Users
        [HttpGet]
        [Authorize] // Any authenticated user can view suppliers
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers
                .Include(s => s.Products) // Include Products related to Supplier
                .AsNoTracking()
                .ToListAsync();
        }

        // ✅ GET: api/Suppliers/5 (Includes Products)
        [HttpGet("{id}")]
        [Authorize] // Any authenticated user can view a specific supplier
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            return supplier;
        }

        // ✅ PUT: api/Suppliers/5 (Restricted to Admin & Inventory Manager)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,InventoryManager")] // Only Admin & Inventory Manager can update suppliers
        public async Task<IActionResult> PutSupplier(int id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest("Supplier ID mismatch.");
            }

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SupplierExists(id))
                {
                    return NotFound("Supplier not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ POST: api/Suppliers (Restricted to Admin & Inventory Manager)
        [HttpPost]
        [Authorize(Roles = "Admin,InventoryManager")] // Only Admin & Inventory Manager can create suppliers
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
        }

        // ✅ DELETE: api/Suppliers/5 (Restricted to Admin & Inventory Manager)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,InventoryManager")] // Only Admin & Inventory Manager can delete suppliers
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Products) // Check if supplier has products
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
            {
                return NotFound("Supplier not found.");
            }

            // ❌ Prevent deletion if supplier has products
            if (supplier.Products != null && supplier.Products.Count > 0)
            {
                return BadRequest("Cannot delete supplier with existing products.");
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> SupplierExists(int id)
        {
            return await _context.Suppliers.AnyAsync(e => e.Id == id);
        }
    }
}
