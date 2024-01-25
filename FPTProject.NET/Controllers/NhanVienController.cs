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
    public class NhanVienController : Controller
    {
        private readonly QlksdbContext _context;

        public NhanVienController(QlksdbContext context)
        {
            _context = context;
        }

        // GET: NhanVien
        public async Task<IActionResult> Index()
        {
            var qlksdbContext = _context.NhanViens.Include(n => n.UserNameNavigation);
            return View(await qlksdbContext.ToListAsync());
        }

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVienModel = await _context.NhanViens
                .Include(n => n.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nhanVienModel == null)
            {
                return NotFound();
            }

            return View(nhanVienModel);
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            ViewData["UserName"] = new SelectList(_context.TaiKhoans, "UserName", "UserName");
            return View();
        }

        // POST: NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaNv,TenNv,HinhAnh,SoDt,CCccd,DiaChi,Email,UserName")] NhanVienModel nhanVienModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVienModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserName"] = new SelectList(_context.TaiKhoans, "UserName", "UserName", nhanVienModel.UserName);
            return View(nhanVienModel);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVienModel = await _context.NhanViens.FindAsync(id);
            if (nhanVienModel == null)
            {
                return NotFound();
            }
            ViewData["UserName"] = new SelectList(_context.TaiKhoans, "UserName", "UserName", nhanVienModel.UserName);
            return View(nhanVienModel);
        }

        // POST: NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaNv,TenNv,HinhAnh,SoDt,CCccd,DiaChi,Email,UserName")] NhanVienModel nhanVienModel)
        {
            if (id != nhanVienModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVienModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienModelExists(nhanVienModel.Id))
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
            ViewData["UserName"] = new SelectList(_context.TaiKhoans, "UserName", "UserName", nhanVienModel.UserName);
            return View(nhanVienModel);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVienModel = await _context.NhanViens
                .Include(n => n.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nhanVienModel == null)
            {
                return NotFound();
            }

            return View(nhanVienModel);
        }
        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.NhanViens == null)
            {
                return Problem("Entity set 'QlksdbContext.NhanViens'  is null.");
            }
            var nhanVienModel = await _context.NhanViens.FindAsync(id);
            if (nhanVienModel != null)
            {
                _context.NhanViens.Remove(nhanVienModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienModelExists(int id)
        {
          return (_context.NhanViens?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
