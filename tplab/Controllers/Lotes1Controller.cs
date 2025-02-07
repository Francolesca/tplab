using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using tplab.Models;
using tplab.Models.Entidades;

namespace tplab.Controllers
{
    [Authorize]
    public class Lotes1Controller : Controller
    {
        private readonly AppDbContext _context;

        public Lotes1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lotes1
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int days)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;




            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var appDbContext = from l in _context.Lotes.Include(p => p.Producto)
                               select l;
            if (!String.IsNullOrEmpty(searchString))
            {
                appDbContext = appDbContext.Where(s => s.Producto.Nombre.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "ProdOrder":
                    appDbContext = appDbContext.OrderBy(l => l.Producto);
                    break;
                case "DateOrder":
                    appDbContext = appDbContext.OrderBy(l => l.FechaVencimiento);
                    break;
                default:
                    break;
            }
           // if(days != 0)
            //{
            //    DateTime filterxday = DateTime.Today.AddDays(days);
           //     appDbContext = appDbContext.Where(l => l.FechaVencimiento <= filterxday).;
           // }

            int pageSize = 5;
            return View(await Paginacion<Lote>.CreateAsync(appDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));

            //return View(await appDbContext.AsNoTracking().ToListAsync());

        }
        public async Task<IActionResult> LotesXid(int id)
        {
            var appDbContext = _context.Lotes
                .Where(l => l.ProductoId == id)
                .Include(l => l.Producto);
            var prod = _context.Productos.Find(id);
            ViewData["prod"] = prod;
            return View(await appDbContext.ToListAsync());
        }


        // GET: Lotes1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lote == null)
            {
                return NotFound();
            }

            return View(lote);
        }

        // GET: Lotes1/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            return View();
        }

        // POST: Lotes1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cantidad,FechaVencimiento,ProductoId")] Lote lote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", lote.ProductoId);
            return View(lote);
        }

        // GET: Lotes1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", lote.ProductoId);
            return View(lote);
        }

        // POST: Lotes1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cantidad,FechaVencimiento,ProductoId")] Lote lote)
        {
            if (id != lote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoteExists(lote.Id))
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", lote.ProductoId);
            return View(lote);
        }

        // GET: Lotes1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lote == null)
            {
                return NotFound();
            }

            return View(lote);
        }

        // POST: Lotes1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lote = await _context.Lotes.FindAsync(id);
            if (lote != null)
            {
                _context.Lotes.Remove(lote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoteExists(int id)
        {
            return _context.Lotes.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<FileResult> ExportarLotesAExcel()
        {
            var lotes = await _context.Lotes.Include(l => l.Producto).ToListAsync();
            var nombreArchivo = $"Lotes.xlsx";
            return GenerarExcel(nombreArchivo, lotes);
        }



        private FileResult GenerarExcel(string nombreArchivo, IEnumerable<Lote> lotes)
        {
            DataTable dataTable = new DataTable("lotes");

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id"),
                new DataColumn("Cantidad"),
                new DataColumn("FechaVencimiento"),
                new DataColumn("Producto")
            });
            foreach (var lote in lotes)
            {
                dataTable.Rows.Add(lote.Id, lote.Cantidad, lote.FechaVencimiento, lote.Producto.Nombre);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreArchivo);
                }
            }


        }
    }
    
}
