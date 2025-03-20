using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JFGM20250320.AppWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Prueba19Definitiva.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Test20250320DbContext _context;

        public ProductsController(Test20250320DbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(Product product, int topRegis = 10)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(product.ProductName))
                query = query.Where(s => s.ProductName.Contains(product.ProductName));
            if (!string.IsNullOrWhiteSpace(product.Description))
                query = query.Where(s => s.Description.Contains(product.Description));
            if (product.BrandId > 0)
                query = query.Where(s => s.BrandId == product.BrandId);
            if (product.WarehouseId > 0)
                query = query.Where(s => s.WarehouseId == product.WarehouseId);
            if (topRegis > 0)
                query = query.Take(topRegis);
            query = query
                .Include(p => p.Warehouse).Include(p => p.Brand);

            var brand = _context.Brands.ToList();
            brand.Add(new Brand { BrandName = "SELECCIONAR", BrandId = 0 });
            var categorie = _context.Warehouses.ToList();
            categorie.Add(new Warehouse { WarehouseName = "SELECCIONAR", WarehouseId = 0 });
            ViewData["WarehouseId"] = new SelectList(categorie, "WarehouseId", "WarehouseName", 0);
            ViewData["BrandId"] = new SelectList(brand, "BrandId", "BrandName", 0);

            return View(await query.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["CategoryId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,Price,WarehouseId,BrandId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseId", product.WarehouseId);
            return View(product);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseName", product.WarehouseId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Description,Price,WarehouseId,BrandId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", product.BrandId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseId", product.WarehouseId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
