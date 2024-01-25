using AspNetCoreHero.ToastNotification.Abstractions;
using Hotel.Base;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    public class ThongKeController : BaseController
    {
        private readonly INotyfService _noft;
        public ThongKeController(QlksdbContext context, INotyfService noft)
            : base(context)
        {
            _noft = noft;
        }
        public IActionResult Index()
        {
            if (base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
                if (
                _context.DatDichVus != null
                && _context.NhanViens != null
                && _context.TaiKhoans != null
                && _context.DatPhongs != null
            )
                {
                    BaseClass bs = new BaseClass();
                    ViewBag.TenDangNhap = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"])
                        .Split(";")[0];
                    ViewBag.Year = DateTime.Now.Year;
                    List<object> listThongKe = new List<object>();
                    List<int> listYearThanhToan = new List<int>();
                    List<int> listNgayThanhToan = new List<int>();
                    foreach (
                        var item in _context.HoaDonDatPhongs
                            .OrderByDescending(hd => hd.NgayHd.Year)
                            .ToList()
                    )
                    {
                        int year = item.NgayHd.Year;
                        if (!listYearThanhToan.Contains(year))
                        {
                            listYearThanhToan.Add(year);
                        }
                    }

                    var tongTienDichVu = _context.DatDichVus
                        .Where(ddv => ddv.NgayDatDichVu.Month == 2)
                        .Sum(ddv => ddv.SoLuong * ddv.MaDvNavigation.DonGia);
                    var kq = _context.DatPhongs
                        .Where(
                            dp => dp.NgayBatDau.Month == 2 && dp.NgayBatDau.Year == DateTime.Now.Year
                        )
                        .GroupBy(dp => dp.MaKh)
                        .Count();

                    ThongKeTheoNamModel thongKeTheoNamModel = new ThongKeTheoNamModel()
                    {
                        TongSoKHTrongNam = base.TongSoKHTrongNam(DateTime.Now.Year),
                        TongDoanhThuTrongNam = base.TongDoanhThuTrongNam(DateTime.Now.Year),
                        TongSoPhongTrongNam = base.TongSoPhongDuocSuDungTrongNam(DateTime.Now.Year),
                        TongSoNgayDatDoanhThuTheoNam = base.TongSoNgayDatDoanhThu(DateTime.Now.Year)
                    };
                    ViewData["thongKeTheoNamModel"] = thongKeTheoNamModel;

                    listThongKe.Add(listYearThanhToan); // 0
                    listThongKe.Add(base.listThongKeDoanhThuTheoLoaiPhong(DateTime.Now.Year)); //1
                    listThongKe.Add(base.listTopKhachHangTieuBieu(2023)); //2
                    listThongKe.Add(base.listTopNhanVienTieuBieu(2023)); //3
                    listThongKe.Add(base.listDoanhThuTheoThang(2023)); //4
                    listThongKe.Add(base.listTongSoNgayDatDoanhThuTrongNam(2023)); //5
                    return View(listThongKe);
                }
            }
            _noft.Error("Bạn không đủ quyền truy cập!");
            return base.ChuyenHuong();
        }

        [HttpPost]
        public IActionResult Index(int chooseyeardropdown)
        {
            BaseClass bs = new BaseClass();
            ViewBag.TenDangNhap = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[
                0
            ];
            List<object> listThongKe = new List<object>();
            List<int> listYearThanhToan = new List<int>();
            List<int> listNgayThanhToan = new List<int>();
            foreach (
                var item in _context.HoaDonDatPhongs
                    .OrderByDescending(hd => hd.NgayHd.Year)
                    .ToList()
            )
            {
                int year = item.NgayHd.Year;
                if (!listYearThanhToan.Contains(year))
                {
                    listYearThanhToan.Add(year);
                }
            }

            ThongKeTheoNamModel thongKeTheoNamModel = new ThongKeTheoNamModel()
            {
                TongSoKHTrongNam = base.TongSoKHTrongNam(chooseyeardropdown),
                TongDoanhThuTrongNam = base.TongDoanhThuTrongNam(chooseyeardropdown),
                TongSoPhongTrongNam = base.TongSoPhongDuocSuDungTrongNam(chooseyeardropdown),
                TongSoNgayDatDoanhThuTheoNam = base.TongSoNgayDatDoanhThu(chooseyeardropdown)
            };
            ViewData["thongKeTheoNamModel"] = thongKeTheoNamModel;
            ViewBag.Year = chooseyeardropdown;
            listThongKe.Add(listYearThanhToan); // 0
            listThongKe.Add(base.listThongKeDoanhThuTheoLoaiPhong(chooseyeardropdown)); //1
            listThongKe.Add(base.listTopKhachHangTieuBieu(chooseyeardropdown)); //2
            listThongKe.Add(base.listTopNhanVienTieuBieu(chooseyeardropdown)); //3
            listThongKe.Add(base.listDoanhThuTheoThang(chooseyeardropdown)); //4
            listThongKe.Add(base.listTongSoNgayDatDoanhThuTrongNam(chooseyeardropdown)); //5
            return View(listThongKe);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
