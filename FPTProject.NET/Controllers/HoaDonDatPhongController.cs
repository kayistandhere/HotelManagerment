using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Hotel.Base;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    public class HoaDonDatPhongController : BaseController
    {
        private readonly INotyfService _nofty;
        public HoaDonDatPhongController(QlksdbContext context, INotyfService nofty) : base(context)
        {
            _nofty = nofty;
        }

        // GET: HoaDonDatPhong
        public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
        {
            if (base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("QuanLy"))
            {
                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewData["CurrentFilter"] = searchString;
                var HoaDonDatPhongs = _context.HoaDonDatPhongs.Include(hd => hd.MaDpNavigation)
                                                        .Include(hd => hd.MaNvNavigation).Include(hd => hd.MaDpNavigation.MaPNavigation)
                                                        .Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
                                                        .Include(hd => hd.MaDpNavigation.MaKhNavigation)
                                                        .OrderByDescending(hd => hd.NgayHd);
                if (!String.IsNullOrEmpty(searchString))
                {
                    HoaDonDatPhongs = HoaDonDatPhongs
                        .Where(hd => hd.MaHddp.Contains(searchString) 
                                    || hd.MaDp.Contains(searchString) 
                                    || hd.MaNv.Contains(searchString) 
                                    || hd.NgayHd.ToString().Contains(searchString) 
                                    || hd.MaDpNavigation.MaKh.Contains(searchString))
                                    .OrderByDescending(hd => hd.NgayHd);
                }
                int pageSize = 12;
                return View(await PaginatedList<HoaDonDatPhongModel>.CreateAsync(HoaDonDatPhongs.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            _nofty.Error("Bạn không đủ quyền truy cập!");
            return base.ChuyenHuong();
        }

        // GET: HoaDonDatPhong/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("QuanLy"))
            {
                if (id == null || _context.HoaDonDatPhongs == null)
                {
                    return NotFound();
                }

                var hoaDonDatPhongModel = await _context.HoaDonDatPhongs
                    .Include(h => h.MaDpNavigation)
                    .Include(h => h.MaNvNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (hoaDonDatPhongModel == null)
                {
                    return NotFound();
                }

                return View(hoaDonDatPhongModel);
            }
            return base.ChuyenHuong();
        }

        // GET: HoaDonDatPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
                if (id == null || _context.HoaDonDatPhongs == null)
                {
                    return NotFound();
                }

                var hoaDonDatPhongModel = await _context.HoaDonDatPhongs
                    .Include(h => h.MaDpNavigation)
                    .Include(h => h.MaNvNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (hoaDonDatPhongModel == null)
                {
                    return NotFound();
                }

                return View(hoaDonDatPhongModel);
            }
            return base.ChuyenHuong();
        }

        // POST: HoaDonDatPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
                if (_context.HoaDonDatPhongs == null)
                {
                    return Problem("Entity set 'QlksdbContext.HoaDonDatPhongs'  is null.");
                }
                var hoaDonDatPhongModel = await _context.HoaDonDatPhongs.FindAsync(id);
                if (hoaDonDatPhongModel != null)
                {
                    _context.HoaDonDatPhongs.Remove(hoaDonDatPhongModel);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return base.ChuyenHuong();
        }
    }
}
