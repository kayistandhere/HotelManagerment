using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QlksdbContext _context;

        public KhachHangController(QlksdbContext context)
        {
            _context = context;
        }

        // GET: KhachHang
        public async Task<IActionResult> Index(string sortOrder,
            string searchString,
            string currentFillter,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var KhachHang = (from s in _context.KhachHangs
                             select s);
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFillter;
            }
            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                KhachHang = KhachHang.Where(x => x.MaKh.Contains(searchString) || x.TenKh.Contains(searchString) || x.SoDt.Contains(searchString));

            }
            int pageSize = 12;
            return View(await PaginatedList<KhachHangModel>.CreateAsync(KhachHang.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
    
        // GET: KhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KhachHangs == null)
            {
                return NotFound();
            }

            var khachHangModel = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (khachHangModel == null)
            {
                return NotFound();
            }

            return View(khachHangModel);
        }

        // GET: KhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaKh,TenKh,HinhAnh,SoDt,CCcd,DiaChi")] KhachHangModel khachHangModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachHangModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khachHangModel);
        }

        // GET: KhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KhachHangs == null)
            {
                return NotFound();
            }

            var khachHangModel = await _context.KhachHangs.FindAsync(id);
            if (khachHangModel == null)
            {
                return NotFound();
            }
            return View(khachHangModel);
        }

        // POST: KhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaKh,TenKh,HinhAnh,SoDt,CCcd,DiaChi")] KhachHangModel khachHangModel)
        {
            if (id != khachHangModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachHangModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangModelExists(khachHangModel.Id))
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
            return View(khachHangModel);
        }

        // GET: KhachHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KhachHangs == null)
            {
                return NotFound();
            }

            var khachHangModel = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (khachHangModel == null)
            {
                return NotFound();
            }

            return View(khachHangModel);
        }

        // POST: KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KhachHangs == null)
            {
                return Problem("Entity set 'QlksdbContext.KhachHangs'  is null.");
            }
            var khachHangModel = await _context.KhachHangs.FindAsync(id);
            if (khachHangModel != null)
            {
                _context.KhachHangs.Remove(khachHangModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangModelExists(int id)
        {
            return (_context.KhachHangs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
