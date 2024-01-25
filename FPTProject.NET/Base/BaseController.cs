using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelectPdf;

namespace Hotel.Base
{
    public class BaseController : Controller
    {
        protected readonly QlksdbContext _context;

        public BaseController(QlksdbContext context)
        {
            _context = context;
        }

        public bool KiemTraPhanQuyen(string roleName)
        {
            // bool result = false;
            int roleId = -99;
            string? strCookie =
                HttpContext.Request.Cookies["DangNhap"] == null
                    ? ""
                    : HttpContext.Request.Cookies["DangNhap"];
            if (!string.IsNullOrEmpty(strCookie) && _context.TaiKhoans != null)
            {
                BaseClass bs = new BaseClass();
                strCookie = bs.deCodeHash(strCookie);
                string tenDN = strCookie.Split(";")[0];
                Int32.TryParse(strCookie.Split(";")[1], out roleId);
                if (_context.TaiKhoans.Where(tk => tk.UserName == tenDN).FirstOrDefault() != null) // đã đăng nhập
                {
                    if (!string.IsNullOrEmpty(roleName)) // trường sử dụng phân quyền để truy cập
                    {
                        if (roleName.Equals("Admin") && roleId == 99)
                        {
                            return true;
                        }
                        else if (roleName.Equals("QuanLy") && roleId == 1)
                        {
                            return true;
                        }
                        else if (roleName.Equals("NhanVien") && roleId == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else // trường hợp xác nhận đăng nhập
                    {
                        return true;
                    }
                }
                else
                {
                    // cookie vẫn được lưu nhưng tài khoản trong database đã bị xóa -> tiến hành xóa cookie
                    xoaCookieDangNhap();
                    ModelState.AddModelError(
                        "",
                        "Tài khoản đã không còn tồn tại trên hệ thống! Cookie đã được xóa bỏ!"
                    );
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void xoaCookieDangNhap()
        {
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                if (cookie.Key.Equals("DangNhap"))
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }
        }
        public IActionResult ChuyenHuong()
        {
            if (HttpContext.Request.Cookies["DangNhap"] != null)
            {
                BaseClass bs = new BaseClass();
                int roleId = Int32.Parse(
                    bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[1]
                );
                System.Console.WriteLine(roleId);
                if (roleId == 99)
                {
                    return RedirectToAction("Index", "TaiKhoan");
                }
                else if (roleId == 1)
                {
                    return RedirectToAction("Index", "NhanVien");
                }
                else if (roleId == 0)
                {
                    return RedirectToAction("Index", "DatPhong");
                }
                return NotFound();
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        public void addModelError()
        {
            var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            ViewBag.Errors = errors;
        }

        public void taoMaDp()
        {
            var datP = _context.DatPhongs.OrderByDescending(dp => dp.Id).FirstOrDefault();
            if (datP != null)
            {
                ViewData["MaDp"] = "MDP" + (datP.Id + 1).ToString();
            }
            else
            {
                ViewData["MaDp"] = "MDP1";
            }
        }

        public void taoMaKH()
        {
            var kh = _context.KhachHangs.OrderByDescending(dp => dp.Id).FirstOrDefault();
            if (kh != null)
            {
                ViewData["MaKh"] = "MaKH" + (kh.Id + 1).ToString();
            }
            else
            {
                ViewData["MaKh"] = "MaKH1";
            }
        }

        public bool KiemTraThoiGianDatPhong(DatPhongModel datPhong)
        {
            bool check = true;
            var timeCheckIn = datPhong.NgayBatDau;
            var timeCheckOut = datPhong.NgayKetThuc;
            var datPhongs =
                from dp in _context.DatPhongs
                join hddp in _context.HoaDonDatPhongs on dp.MaDp equals hddp.MaDp into g
                from x in g.DefaultIfEmpty()
                select new { dp = dp, maHD = x.MaHddp };
            foreach (var item in datPhongs)
            {
                if (item.maHD == null && item.dp.MaP == datPhong.MaP)
                {
                    TimeSpan timeSpan1 = (datPhong.NgayBatDau - item.dp.NgayKetThuc);
                    TimeSpan timeSpan2 = (item.dp.NgayBatDau - datPhong.NgayKetThuc);
                    if (!(timeSpan1.TotalMinutes > 0 || timeSpan2.TotalMinutes > 0))
                    {
                        check = false;
                    }
                }
            }
            return check;
        }

        public bool KiemTraThoiGianDatDichVu(DatDichVuModel datDV)
        {
            bool check = true;
            var ngayDatDichVu = datDV.NgayDatDichVu;
            var datPhongs = _context.DatPhongs.FirstOrDefault(d => d.MaDp == datDV.MaDp);

            TimeSpan timeSpan1 = (ngayDatDichVu - datPhongs.NgayBatDau);
            TimeSpan timeSpan2 = (datPhongs.NgayKetThuc - ngayDatDichVu);
            if (timeSpan1.TotalMinutes <= 0 || timeSpan2.TotalMinutes <= 0)
            {
                check = false;
            }
            return check;
        }

        public string getTenDn()
        {
            BaseClass bs = new BaseClass();
            string tenDN = "";
            string? chuoiCookie = HttpContext.Request.Cookies["DangNhap"];
            if (chuoiCookie != null)
            {
                chuoiCookie = bs.deCodeHash(chuoiCookie);
            }
            if (chuoiCookie != null && chuoiCookie.Contains(";"))
            {
                tenDN = chuoiCookie.Split(";")[0];
            }
            return tenDN;
        }

        //Tâm thêm
        public List<ThongKeDoanhThuTheoThangModel> listDoanhThuTheoThang(int year)
        {
            var listResult = new List<ThongKeDoanhThuTheoThangModel>();

            if (_context.HoaDonDatPhongs != null)
            {
                listResult = _context.HoaDonDatPhongs
                    .Include(hd => hd.MaDpNavigation)
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(hd => hd.NgayHd.Month)
                    .AsEnumerable()
                    .Select(
                        group =>
                            new ThongKeDoanhThuTheoThangModel
                            {
                                month = group.Key,
                                total = group.Sum(
                                    g =>
                                        (decimal)(
                                            Math.Ceiling(
                                                (g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays
                                            )
                                        ) * g.MaDpNavigation.TongTien
                                )
                            }
                    )
                    .ToList();
            }
            return listResult;
        }

        public List<NhanVienModel> listTopNhanVienTieuBieu(int year)
        {
            var listResult = new List<NhanVienModel>();
            if (_context.NhanViens != null && _context.HoaDonDatPhongs != null)
            {
                var list = _context.HoaDonDatPhongs
                    .Include(hd => hd.MaDpNavigation)
                    .Include(hd => hd.MaDpNavigation.MaPNavigation)
                    .Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(hd => hd.MaNv)
                    .Select(group => new { MaNV = group.Key, Count = group.Count() })
                    .ToList();
                list = list.OrderByDescending(l => l.Count).Take(10).ToList();
                int dem = 0;
                foreach (var item in list)
                {
                    dem++;
                    listResult.Add(
                        _context.NhanViens
                            .Include(nv => nv.UserNameNavigation)
                            .Include(nv => nv.UserNameNavigation.IdRoleNavigation)
                            .Where(nv => nv.MaNv == item.MaNV)
                            .FirstOrDefault()
                    );
                    if (dem == 10)
                        break;
                }
            }
            return listResult;
        }

        public List<KhachHangModel> listTopKhachHangTieuBieu(int year)
        {
            var listResult = new List<KhachHangModel>();
            if (_context.DatPhongs != null && _context.KhachHangs != null)
            {
                var list = _context.DatPhongs
                    .Where(dp => dp.NgayBatDau.Year == year)
                    .GroupBy(dp => dp.MaKh)
                    .Select(group => new { MaKH = group.Key, Count = group.Count() })
                    .ToList();
                list = list.OrderByDescending(group => group.Count).ToList();
                int dem = 0;
                foreach (var item in list)
                {
                    dem++;
                    listResult.Add(
                        _context.KhachHangs.Where(kh => kh.MaKh == item.MaKH).FirstOrDefault()
                    );
                    if (dem == 3)
                        break;
                }
            }
            return listResult;
        }

        public List<ThongKeDoanhThuLoaiPhongModel> listThongKeDoanhThuTheoLoaiPhong(int year)
        {
            var listDoanhThuLoaiPhong = new List<ThongKeDoanhThuLoaiPhongModel>();
            if (_context.HoaDonDatPhongs != null && _context.LoaiPhongs != null)
            {
                listDoanhThuLoaiPhong = _context.HoaDonDatPhongs
                    .Include(hd => hd.MaDpNavigation)
                    .Include(hd => hd.MaDpNavigation.MaPNavigation)
                    .Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp)
                    .AsEnumerable()
                    .Select(
                        group =>
                            new ThongKeDoanhThuLoaiPhongModel
                            {
                                tenLP = group.Key,
                                total = group.Sum(
                                    g =>
                                        (decimal)(
                                            Math.Ceiling(
                                                (g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays
                                            )
                                        ) * g.MaDpNavigation.TongTien
                                )
                            }
                    )
                    .ToList();
            }
            if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Đơn")) == null)
            {
                listDoanhThuLoaiPhong.Add(
                    new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Đơn", total = 0 }
                );
            }
            if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Đôi")) == null)
            {
                listDoanhThuLoaiPhong.Add(
                    new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Đôi", total = 0 }
                );
            }
            if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Gia Đình")) == null)
            {
                listDoanhThuLoaiPhong.Add(
                    new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Gia Đình", total = 0 }
                );
            }
            if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Vip")) == null)
            {
                listDoanhThuLoaiPhong.Add(
                    new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Vip", total = 0 }
                );
            }
            return listDoanhThuLoaiPhong;
        }

        public List<ThongKeSoNgayDatDoanhThuNhanVienModel> listTongSoNgayDatDoanhThuTrongNam(
            int year
        )
        {
            var listResult = new List<ThongKeSoNgayDatDoanhThuNhanVienModel>();
            if (_context.HoaDonDatPhongs != null && _context.NhanViens != null)
            {
                listResult = _context.HoaDonDatPhongs
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(
                        hd =>
                            new
                            {
                                hd.MaNv,
                                hd.NgayHd.Day,
                                hd.NgayHd.Month
                            }
                    )
                    .AsEnumerable()
                    .Select(
                        group =>
                            new
                            {
                                MaNV = group.Key.MaNv,
                                Thang = group.Key.Month,
                                Ngay = group.Key.Day,
                                total = group.Sum(
                                    g =>
                                        (decimal)(
                                            Math.Ceiling(
                                                (g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays
                                            )
                                        ) * g.MaDpNavigation.TongTien
                                )
                            }
                    )
                    .GroupBy(group => new { group.MaNV, group.Thang })
                    .Select(
                        group =>
                            new
                            {
                                MaNV = group.Key.MaNV,
                                Thang = group.Key.Thang,
                                Ngay = group.Count(),
                                Total = group.Sum(l => l.total)
                            }
                    )
                    .GroupBy(group => group.MaNV)
                    .Select(
                        group =>
                            new ThongKeSoNgayDatDoanhThuNhanVienModel
                            {
                                MaNV = group.Key,
                                TongNgay = group.Sum(l => l.Ngay),
                                Total = group.Sum(l => l.Total)
                            }
                    )
                    .ToList();
            }
            return listResult;
        }

        public int TongSoKHTrongNam(int year)
        {
            int result = 0;
            if (_context.HoaDonDatPhongs != null && _context.KhachHangs != null)
            {
                result = _context.HoaDonDatPhongs
                    .Include(hd => hd.MaDpNavigation)
                    .Include(hd => hd.MaDpNavigation.MaKhNavigation)
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(hd => hd.MaDpNavigation.MaKh)
                    .Select(group => new { MaKH = group.Key, Count = group.Count() })
                    .Count();
            }
            return result;
        }

        public decimal TongDoanhThuTrongNam(int year)
        {
            decimal? result = 0;
            if (_context.HoaDonDatPhongs != null)
            {
                var i = _context.HoaDonDatPhongs
                    .Include(hd => hd.MaDpNavigation)
                    .Where(hd => hd.NgayHd.Year == year)
                    .AsEnumerable()
                    .Sum(
                        g =>
                            (decimal)(
                                Math.Ceiling((g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays)
                            ) * g.MaDpNavigation.TongTien
                            ?? 0
                    );
                var i2 = _context.DatDichVus
                    .Include(ddv => ddv.MaDvNavigation)
                    .Where(ddv => ddv.NgayDatDichVu.Year == year)
                    .Sum(ddv => ddv.SoLuong * ddv.MaDvNavigation.DonGia);
                result = i + i2;
            }

            return result.Value;
        }

        public int TongSoNgayDatDoanhThu(int year)
        {
            int result = 0;
            if (_context.HoaDonDatPhongs != null)
            {
                result = _context.HoaDonDatPhongs
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(hd => new { hd.NgayHd.Month, hd.NgayHd.Day })
                    .Select(
                        group =>
                            new
                            {
                                Thang = group.Key.Month,
                                Ngay = group.Key.Day,
                                Count = group.Count()
                            }
                    )
                    .GroupBy(group => group.Thang)
                    .Select(group => new { Thang = group.Key, Count = group.Count() })
                    .Sum(group => group.Count);
            }
            return result;
        }

        public int TongSoPhongDuocSuDungTrongNam(int year)
        {
            int result = 0;
            if (_context.HoaDonDatPhongs != null && _context.Phongs != null)
            {
                result = _context.HoaDonDatPhongs
                    .Include(hd => hd.MaDpNavigation)
                    .Include(hd => hd.MaDpNavigation.MaPNavigation)
                    .Where(hd => hd.NgayHd.Year == year)
                    .GroupBy(hd => hd.MaDpNavigation.MaPNavigation.SoPhong)
                    .Select(group => new { Phong = group.Key, Count = group.Count() })
                    .Count();
            }
            return result;
        }
    }
}
