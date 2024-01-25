using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;
using Hotel.Base;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Hotel.Controllers
{
    public class DatDichVuController : BaseController
    {
        private readonly QlksdbContext _context;
        private readonly INotyfService _notyf;

        public DatDichVuController(QlksdbContext context, INotyfService notyf)
            : base(context)
        {
            _context = context;
            _notyf = notyf;
        }

        public void taoMaDdv()
        {
            var datDv = _context.DatDichVus.OrderByDescending(dv => dv.Id).FirstOrDefault();
            if (datDv != null)
            {
                ViewData["MaDdv"] = "DDV" + (datDv.Id + 1).ToString();
            }
            else
            {
                ViewData["MaDdv"] = "DDV1";
            }
        }

        public IActionResult Index()
        {
            if (_context.DatDichVus == null)
            {
                return Problem("Chưa tồn tại phòng nào");
            }
            var datDichVu = _context.DatDichVus
                .Include(d => d.MaDpNavigation)
                .Include(d => d.MaDpNavigation.MaPNavigation)
                .Include(d => d.MaDvNavigation)
                .OrderByDescending(ddv => ddv.MaDdv)
                .Where(d => d.NgayHd == null)
                .ToList();
            return View(datDichVu);
        }
        [HttpGet]
        public IActionResult HoaDonDatDichVu()
        {
            if (_context.DatDichVus == null)
            {
                return Problem("Chưa tồn tại phòng nào");
            }
            var datDichVu = _context.DatDichVus
                .Include(d => d.MaDpNavigation)
                .Include(d => d.MaDpNavigation.MaPNavigation)
                .Include(d => d.MaDvNavigation)
                .OrderByDescending(ddv => ddv.MaDdv)
                .Where(d => d.NgayHd != null)
                .ToList();
            return View(datDichVu);
        }

        public IActionResult ChiTietDichVuCuaDatPhong(string? maDp)
        {
            var datP = _context.DatPhongs
                .Include(dp => dp.MaPNavigation)
                .FirstOrDefault(dp => dp.MaDp == maDp);
            ViewBag.SoPhong = datP.MaPNavigation.SoPhong;
            var listDdv = new List<DatDichVuModel>();
            if (_context.DatDichVus != null)
            {
                listDdv = _context.DatDichVus
                    .Include(ddv => ddv.MaDvNavigation)
                    .OrderByDescending(d => d.Id)
                    .Where(ddv => ddv.MaDp == maDp)
                    .ToList();
            }
            return View(listDdv);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpGet]
        public IActionResult Create(int? idDP)
        {
            TempData["idDP"] = idDP;
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("QuanLy"))
            {
                var datP = _context.DatPhongs.FirstOrDefault(p => p.Id == idDP);
                var phong = _context.Phongs.FirstOrDefault(dp => dp.MaP == datP.MaP);
                ViewData["MaDp"] = datP.MaDp;
                ViewData["SoPhong"] = phong.SoPhong;
                ViewData["MaDv"] = new SelectList(_context.DichVus, "MaDv", "TenDv");
                var tkDn = base.getTenDn();
                var nv = _context.NhanViens.Where(nv => nv.UserName == tkDn).FirstOrDefault();
                ViewData["MaNv"] = nv.MaNv;
                taoMaDdv();
                return View();
            }
            _notyf.Error("Không đủ quyền truy cập!");
            return base.ChuyenHuong();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? soP, DatDichVuModel datdichvu)
        {
            var dichVu = _context.DichVus.FirstOrDefault(v => v.MaDv == datdichvu.MaDv);
            var idDP = TempData["idDP"];
            if (ModelState.IsValid)
            {
                if (dichVu.SoLuong >= datdichvu.SoLuong)
                {
                    if (base.KiemTraThoiGianDatDichVu(datdichvu))
                    {
                        datdichvu.TongTien = datdichvu.SoLuong * dichVu.DonGia;
                        dichVu.SoLuong = dichVu.SoLuong - datdichvu.SoLuong;
                        _context.Update(dichVu);
                        await _context.SaveChangesAsync();
                        _context.Add(datdichvu);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "DatPhong");
                    }else{
                        _notyf.Error("Thời gian đặt dịch vụ vượt quá thời gian đặt phòng");
                    }
                }
                else
                {
                    _notyf.Error("Đặt số lượng quá lớn");
                }
            }
            else
            {
                ViewData["MaDp"] = datdichvu.MaDp;
                ViewData["SoPhong"] = soP;
                ViewData["MaDv"] = new SelectList(_context.DichVus, "MaDv", "TenDv");
                var tkDn = base.getTenDn();
                var nv = _context.NhanViens.Where(nv => nv.UserName == tkDn).FirstOrDefault();
                ViewData["MaNv"] = nv.MaNv;
                taoMaDdv();
                _notyf.Warning("Vui lòng điền đủ dữ liệu");
            }
            return RedirectToAction(nameof(Create), new { idDP = idDP });
        }
    }
}
