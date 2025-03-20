using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JFGM20250320.AppWebMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace JFGM20250320.AppWebMVC.Controllers
{

    [Authorize]
    public class ProductsController : Controller
    {
        private readonly Test20250320DbContext _context;

        public ProductsController(Test20250320DbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(Product producto)
        {

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(producto.ProductName))
                query = query.Where(s => s.ProductName.Contains(producto.ProductName));

            if (producto.BrandId > 0)
                query = query.Where(s => s.BrandId == producto.BrandId);

            if (producto.WarehouseId > 0)
                query = query.Where(s => s.WarehouseId == producto.WarehouseId);


            query = query
                .Include(p => p.Warehouse).Include(p => p.Brand);

            var marcas = _context.Brands.ToList();
            marcas.Add(new Brand { BrandName = "SELECCIONAR", BrandId = 0 });

            var bodegas = _context.Warehouses.ToList();
            bodegas.Add(new Warehouse { WarehouseName = "SELECCIONAR", WarehouseId = 0 });

            ViewData["WarehouseId"] = new SelectList(bodegas, "WarehouseId", "WarehouseName", 0);
            ViewData["BrandId"] = new SelectList(marcas, "BrandId", "BrandName", 0);

            return View(await query.ToListAsync());
        }

        // GET: Products/Details/5
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

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "Notes");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "Notes", product.WarehouseId);
            return View(product);
        }

        // GET: Products/Edit/5
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
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "Notes", product.WarehouseId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "Notes", product.WarehouseId);
            return View(product);
        }

        // GET: Products/Delete/5
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

        // POST: Products/Delete/5
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
