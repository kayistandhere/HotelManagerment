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
    public class DichVuController : Controller
    {
        private readonly QlksdbContext _context;

        public DichVuController(QlksdbContext context)
        {
            _context = context;
        }

        public void taoMaDv()
        {
            var dichVu = _context.DichVus.OrderByDescending(dv => dv.Id).FirstOrDefault();
            if (dichVu != null)
            {
                ViewData["MaDv"] = "DV" + (dichVu.Id + 1).ToString();
            }
            else
            {
                ViewData["MaDv"] = "DV1";
            }
        }

        public async Task<IActionResult> Index()
        {
            return _context.DichVus != null
                ? View(await _context.DichVus.ToListAsync())
                : Problem("Chưa có Dịch vụ nào được thêm!");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public IActionResult Create()
        {
            taoMaDv();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,MaDv,HinhAnh,TenDv,DonGia,SoLuong")] DichVuModel dichvu
        )
        {
            if (ModelState.IsValid)
            {
                System.Console.WriteLine("ok");
                _context.Add(dichvu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dichvu);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || _context.DichVus == null)
            {
                return NotFound();
            }
            var dichVu = _context.DichVus.Find(id);
            if (dichVu == null)
            {
                return NotFound();
            }
            return View(dichVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DichVuModel dichvu)
        {
            if (id != dichvu.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dichvu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dichvu);
        }

        public IActionResult Detail(int? id)
        {
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }
            var dichVu = _context.DichVus.Find(id);
            if (dichVu == null)
            {
                return NotFound();
            }
            return View(dichVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (_context.DatPhongs == null)
            {
                return Problem("Danh sách dich vu đã trống!");
            }
            var dichVu = await _context.DichVus.FirstOrDefaultAsync(dv => dv.Id == id);
            if (dichVu != null)
            {
                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
