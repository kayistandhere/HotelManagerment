using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;

namespace Hotel.Controllers
{
    public class TacVuPhongController : Controller
    {
        private readonly QlksdbContext _context;

        public TacVuPhongController(QlksdbContext context)
        {
            _context = context;
        }

        // GET: TacVuPhong
        public async Task<IActionResult> Index()
        {
              return _context.TacVuPhongs != null ? 
                          View(await _context.TacVuPhongs.ToListAsync()) :
                          Problem("Entity set 'QlksdbContext.TacVuPhongs'  is null.");
        }

        // GET: TacVuPhong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TacVuPhongs == null)
            {
                return NotFound();
            }

            var tacVuPhongModel = await _context.TacVuPhongs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tacVuPhongModel == null)
            {
                return NotFound();
            }

            return View(tacVuPhongModel);
        }

        // GET: TacVuPhong/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaTvp,TenTvp")] TacVuPhongModel tacVuPhongModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tacVuPhongModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tacVuPhongModel);
        }

        // GET: TacVuPhong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TacVuPhongs == null)
            {
                return NotFound();
            }

            var tacVuPhongModel = await _context.TacVuPhongs.FindAsync(id);
            if (tacVuPhongModel == null)
            {
                return NotFound();
            }
            return View(tacVuPhongModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaTvp,TenTvp")] TacVuPhongModel tacVuPhongModel)
        {
            if (id != tacVuPhongModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tacVuPhongModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TacVuPhongModelExists(tacVuPhongModel.Id))
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
            return View(tacVuPhongModel);
        }

        // GET: TacVuPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TacVuPhongs == null)
            {
                return NotFound();
            }

            var tacVuPhongModel = await _context.TacVuPhongs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tacVuPhongModel == null)
            {
                return NotFound();
            }

            return View(tacVuPhongModel);
        }

        // POST: TacVuPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TacVuPhongs == null)
            {
                return Problem("Entity set 'QlksdbContext.TacVuPhongs'  is null.");
            }
            var tacVuPhongModel = await _context.TacVuPhongs.FindAsync(id);
            if (tacVuPhongModel != null)
            {
                _context.TacVuPhongs.Remove(tacVuPhongModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TacVuPhongModelExists(int id)
        {
          return (_context.TacVuPhongs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
