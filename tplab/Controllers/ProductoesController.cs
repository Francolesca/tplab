using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tplab.Models;
using tplab.Models.Entidades;

namespace tplab.Controllers
{
    [Authorize]
    public class ProductoesController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Productoes
        public async Task<IActionResult> Index(
            string currentFilter,
            string searchString,
            int? pageNumber)
        {

            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var appDbContext = from l in _context.Productos.Include(p => p.Marca)
                               select l;
            if (!String.IsNullOrEmpty(searchString))
            {
                appDbContext = appDbContext.Where(s => s.Nombre.Contains(searchString));
            }
            int pageSize = 5;
            return View(await Paginacion<Producto>.CreateAsync(appDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Marca)
                .Include(m => m.Lotes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }
            /*var lotes = await _context.Productos.Where(m => m.Id == id)
                .Include(m => m.Lotes).FirstAsync();
            ViewData["lotesProducto"] = lotes;*/
            return View(producto);
        }

        // GET: Productoes/Create
        public IActionResult Create()
        {
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre");
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,MarcaId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", producto.MarcaId);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", producto.MarcaId);
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,MarcaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", producto.MarcaId);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ProduxProve(int id)
        {
            var appDbContext = _context.Productos
                .Where(l => l.Marca.ProveedorId == id)
                .Include(l => l.Marca);
            var prove = _context.Proveedores.Find(id);
            ViewData["prove"] = prove;
            return View(await appDbContext.ToListAsync());
        }

    }
}
