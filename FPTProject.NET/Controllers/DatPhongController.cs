using AspNetCoreHero.ToastNotification.Abstractions;
using Hotel.Base;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SelectPdf;

namespace Hotel.Controllers
{
    public class DatPhongController : BaseController
    {
        private readonly QlksdbContext _context;
        private readonly INotyfService _notyf;

        public DatPhongController(QlksdbContext context, INotyfService notyf)
            : base(context)
        {
            _context = context;
            _notyf = notyf;
        }

        [HttpGet]
        public IActionResult Index(int? check_nav)
        {
            ViewData["check_nav"] = check_nav;
            var listDP = new List<DatPhongModel>();
            var listMaDpInHD = _context.HoaDonDatPhongs.Select(hddp => hddp.MaDp).ToList();
            if (check_nav == 0)
            {
                ViewData["check_nav"] = 0;
                listDP = _context.DatPhongs
                    .Include(dp => dp.MaPNavigation)
                    .OrderByDescending(dp => dp.Id)
                    .Where(dp => listMaDpInHD.Contains(dp.MaDp))
                    .ToList();
            }
            else
            {
                ViewData["check_nav"] = 1;
                listDP = _context.DatPhongs
                    .Include(dp => dp.MaPNavigation)
                    .OrderByDescending(d => d.Id)
                    .Where(dp => !listMaDpInHD.Contains(dp.MaDp))
                    .ToList();
            }
            return View(listDP);
        }

        [HttpGet]
        public IActionResult DanhSachPhongDaDat()
        {
            var listDP = new List<DatPhongModel>();
            var listMaDpInHD = _context.HoaDonDatPhongs.Select(hddp => hddp.MaDp).ToList();
            listDP = _context.DatPhongs
                .Include(dp => dp.MaPNavigation)
                .OrderByDescending(dp => dp.Id)
                .Where(dp => listMaDpInHD.Contains(dp.MaDp))
                .ToList();
            return View(listDP);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }

            var datphong = await _context.DatPhongs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaPNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (datphong == null)
            {
                return NotFound();
            }

            return View(datphong);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var idP = Int32.Parse(TempData["idP"].ToString());
            var phong = _context.Phongs.FirstOrDefault(p => p.Id == idP);
            ViewBag.MaP = phong.MaP;
            ViewBag.SoPhong = phong.SoPhong;
            ViewBag.IdP = idP;
            taoMaDp();
            taoMaKH();
            return View();
        }

        [HttpGet]
        public IActionResult GetDataPhong(int? idP)
        {
            TempData["idP"] = idP;
            return RedirectToAction(nameof(Create));
        }

        // POST: DatPhong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(
            int? idP,
            DatPhongModel datphong,
            KhachHangModel khachhang
        )
        {
            var phong = _context.Phongs
                .Include(p => p.MaLpNavigation)
                .FirstOrDefault(p => p.Id == idP);
            datphong.TongTien =
                phong.MaLpNavigation.DonGia
                * (decimal)Math.Ceiling((datphong.NgayKetThuc - datphong.NgayBatDau).TotalDays);
            if (ModelState.IsValid)
            {
                if (KiemTraThoiGianDatPhong(datphong))
                {
                    _context.Add(khachhang);
                    await _context.SaveChangesAsync();
                    _context.Add(datphong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DanhSachPhongDatPhong", "Phong");
                }
                else
                {
                    taoMaDp();
                    taoMaKH();
                    ViewData["Data_Phong"] = new
                    {
                        MaP = phong.MaP,
                        SoPhong = phong.SoPhong,
                        IdP = idP
                    };
                    _notyf.Error("Phòng này đã được đặt");
                    return View(datphong);
                }
            }
            else
            {
                ViewBag.MaP = phong.MaP;
                ViewBag.SoPhong = phong.SoPhong;
                ViewBag.IdP = idP;

                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToList();
                foreach (var error in errors)
                {
                    if (error.Key.Contains("TenKh"))
                    {
                        ViewData["Error_TenKh"] = error.Errors;
                    }
                    else if (error.Key.Contains("CCcd"))
                    {
                        ViewData["Error_CCcd"] = error.Errors;
                    }
                    else if (error.Key.Contains("SoDt"))
                    {
                        ViewData["Error_Sodt"] = error.Errors;
                    }
                }
            }
            taoMaDp();
            taoMaKH();
            return View(datphong);
        }

        // GET: DatPhong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //nv so 0, ql 1, admin 99, -1 bi khoa -> access denied
            // string tenDN = HttpContext.Request.Cookies["cookieName"];
            if (HttpContext.Request.Cookies["cookieName"] != null) { }
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }

            var datphong = await _context.DatPhongs.FindAsync(id);
            if (datphong == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", datphong.MaKh);
            ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "MaP", datphong.MaP);
            return View(datphong);
        }

        // POST: DatPhong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, DatPhongModel datphong)
        {
            if (id != datphong.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datphong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatphongExists(datphong.Id))
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
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", datphong.MaKh);
            ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "MaP", datphong.MaP);
            return View(datphong);
        }

        // GET: DatPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }

            var datphong = await _context.DatPhongs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaPNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (datphong == null)
            {
                return NotFound();
            }

            return View(datphong);
        }

        // POST: DatPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DatPhongs == null)
            {
                return Problem("Entity set 'QlksContext.DatPhongs'  is null.");
            }
            var datphong = await _context.DatPhongs.FindAsync(id);
            if (datphong != null)
            {
                _context.DatPhongs.Remove(datphong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatphongExists(int id)
        {
            return (_context.DatPhongs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> ThanhToanDatPhong(int? idDP)
        {
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("QuanLy"))
            {
                BaseClass bs = new BaseClass();
                string tenDN = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[0];
                var nhanVien = await _context.NhanViens
                    .Include(nv => nv.UserNameNavigation)
                    .Where(nv => nv.UserName == tenDN)
                    .FirstOrDefaultAsync();
                if (nhanVien == null)
                {
                    _notyf.Error("Vui lòng cập nhật đầy đủ thông tin nhân viên! Giao dịch bị hủy bỏ!");
                    return ChuyenHuong();
                }
                if (idDP == null || _context.DatPhongs == null)
                {
                    return NotFound();
                }
                var datPhong = await _context.DatPhongs
                    .Include(datPhong => datPhong.MaPNavigation)
                    .Include(datPhong => datPhong.MaKhNavigation)
                    .Include(datPhong => datPhong.MaPNavigation.MaLpNavigation)
                    .Include(datPhong => datPhong.DatDichVus)
                    .FirstOrDefaultAsync(dp => dp.Id == idDP);

                var datDV = await _context.DatDichVus
                    .Include(ddv => ddv.MaDvNavigation)
                    .Include(ddv => ddv.MaDpNavigation)
                    .Where(ddv => ddv.MaDp == datPhong.MaDp)
                    .ToListAsync();

                var hoaDon = await _context.HoaDonDatPhongs
                    .Include(hddp => hddp.MaNvNavigation)
                    .Include(hddp => hddp.MaDpNavigation)
                    .OrderBy(hddp => hddp.MaHddp)
                    .ToListAsync();
                if (datPhong == null)
                {
                    return NotFound();
                }
                List<object> list = new List<object>();
                list.Add(datPhong);
                list.Add(datDV);
                list.Add(hoaDon);
                list.Add(nhanVien);
                return View(list);
            }
            _notyf.Error("Bạn chưa đủ điều kiện để thực hiện chức năng này");
            return base.ChuyenHuong();
        }
        
        [HttpPost]
        public async Task<IActionResult> ThanhToanDatPhong(int? Id, HoaDonDatPhongModel hoaDon)
        {
            HoaDonDatPhongModel hoaDonDatPhong = new HoaDonDatPhongModel()
            {
                MaHddp = hoaDon.MaHddp,
                NgayHd = hoaDon.NgayHd,
                MaNv = hoaDon.MaNv,
                MaDp = hoaDon.MaDp
            };
            if (Id != null)
            {
                if (_context.DatPhongs != null && _context.Phongs != null)
                {
                    var dat = await _context.DatPhongs
                        .Where(dp => dp.MaDp == hoaDonDatPhong.MaDp)
                        .FirstOrDefaultAsync();
                    var datPhong = await _context.DatPhongs.Include(dp => dp.MaPNavigation).Include(dp => dp.MaPNavigation.MaLpNavigation).Where(dp => dp.Id == Id).FirstOrDefaultAsync();
                    if (datPhong != null)
                    {
                        // try
                        // {
                            _context.Add(hoaDonDatPhong);
                            await _context.SaveChangesAsync();
                            datPhong.NgayKetThuc = hoaDonDatPhong.NgayHd;
                            datPhong.TongTien = (decimal)(Math.Ceiling((hoaDonDatPhong.NgayHd - datPhong.NgayBatDau).TotalDays)) * datPhong.MaPNavigation.MaLpNavigation.DonGia.Value;
                            _context.Update(datPhong);
                            await _context.SaveChangesAsync();
                            var datDV = _context.DatDichVus.Include(ddv => ddv.MaDvNavigation)
                                .Where(d => d.MaDp == datPhong.MaDp)
                                .Where(d => d.NgayHd == null)
                                .ToList();
                            if (datDV != null && datDV.Count > 0)
                            {
                                foreach (var item in datDV)
                                {
                                    item.NgayHd = hoaDonDatPhong.NgayHd;
                                    item.TongTien = item.MaDvNavigation.DonGia * item.SoLuong;
                                    _context.Update(item);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            _notyf.Success("Thanh toán thành công!");
                            return RedirectToAction("Index", "HoaDonDatPhong");
                        // }
                        // catch (Exception ex)
                        // {
                        //     ModelState.AddModelError("", $"Thêm hóa đơn thất bại!\n {ex.Message}");
                        // }
                    }
                }
                else
                {
                    _notyf.Success("Thanh toán thất bại!");
                    return RedirectToAction("Index", "HoaDonDatPhong");
                } 
            }
            _notyf.Error("Không thể tạo hóa đơn thanh toán! Vui lòng kiểm tra lại!");
            return RedirectToAction("ThanhToanDatPhong", "DatPhong", new { @idDP = Id });
        }

        [HttpPost]
        public IActionResult GeneratePDF(string html)
        {
            // System.Console.WriteLine(html);
            html = html.Replace("StrTag", "<").Replace("EndTag", ">");
            var htmlToPdf = new HtmlToPdf();
            var pdfDocumnet = htmlToPdf.ConvertHtmlString(html);
            byte[] pdfFile = pdfDocumnet.Save();
            pdfDocumnet.Close();
            return File(pdfFile, "application/pdf", $"hoadon{DateTime.Now.ToString()}.pdf");
        }
    }
}