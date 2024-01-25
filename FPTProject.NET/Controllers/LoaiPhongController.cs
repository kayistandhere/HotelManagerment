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
    public class LoaiPhongController : Controller
    {
        private readonly QlksdbContext _context;

        public LoaiPhongController(QlksdbContext context)
        {
            _context = context;
        }

        // GET: LoaiPhong
        public async Task<IActionResult> Index()
        {
              return _context.LoaiPhongs != null ? 
                          View(await _context.LoaiPhongs.ToListAsync()) :
                          Problem("Entity set 'QlksdbContext.LoaiPhongs'  is null.");
        }

        // GET: LoaiPhong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LoaiPhongs == null)
            {
                return NotFound();
            }

            var loaiPhongModel = await _context.LoaiPhongs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiPhongModel == null)
            {
                return NotFound();
            }

            return View(loaiPhongModel);
        }

        // GET: LoaiPhong/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiPhong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaLp,TenLp,DonGia")] LoaiPhongModel loaiPhongModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiPhongModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiPhongModel);
        }

        // GET: LoaiPhong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LoaiPhongs == null)
            {
                return NotFound();
            }

            var loaiPhongModel = await _context.LoaiPhongs.FindAsync(id);
            if (loaiPhongModel == null)
            {
                return NotFound();
            }
            return View(loaiPhongModel);
        }

        // POST: LoaiPhong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaLp,TenLp,DonGia")] LoaiPhongModel loaiPhongModel)
        {
            if (id != loaiPhongModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiPhongModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiPhongModelExists(loaiPhongModel.Id))
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
            return View(loaiPhongModel);
        }

        // GET: LoaiPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LoaiPhongs == null)
            {
                return NotFound();
            }

            var loaiPhongModel = await _context.LoaiPhongs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiPhongModel == null)
            {
                return NotFound();
            }

            return View(loaiPhongModel);
        }

        // POST: LoaiPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LoaiPhongs == null)
            {
                return Problem("Entity set 'QlksdbContext.LoaiPhongs'  is null.");
            }
            var loaiPhongModel = await _context.LoaiPhongs.FindAsync(id);
            if (loaiPhongModel != null)
            {
                _context.LoaiPhongs.Remove(loaiPhongModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiPhongModelExists(int id)
        {
          return (_context.LoaiPhongs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
