using AspNetCoreHero.ToastNotification.Abstractions;
using Hotel.Base;
using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    public class TaiKhoanController : BaseController
    {
        private readonly INotyfService _nofty;
        private readonly IWebHostEnvironment _env;
        public TaiKhoanController(QlksdbContext context, INotyfService nofty, IWebHostEnvironment env)
            : base(context)
        {
            _nofty = nofty;
            _env = env;
        }

        // GET: TaiKhoan
        public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
        {
            if (base.KiemTraPhanQuyen("Admin"))
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
                var TaiKhoans = _context.TaiKhoans.Include(tk => tk.IdRoleNavigation).OrderByDescending(tk => tk.UserName);
                if (!String.IsNullOrEmpty(searchString))
                {
                    System.Console.WriteLine("Đã vào đây!");
                    TaiKhoans = TaiKhoans
                        .Where(p => p.UserName.Contains(searchString))
                        .OrderByDescending(tk => tk.UserName);
                }
                int pageSize = 12;
                return View(await PaginatedList<TaiKhoanModel>.CreateAsync(TaiKhoans.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            _nofty.Error("Bạn không đủ quyền truy cập!");
            return base.ChuyenHuong();
        }

        // GET: TaiKhoan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!base.KiemTraPhanQuyen("Admin"))
            {
                return base.ChuyenHuong();
            }
            if (id == null || _context.TaiKhoans == null)
            {
                return NotFound();
            }

            var taiKhoanModel = await _context.TaiKhoans
                .Include(t => t.IdRoleNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taiKhoanModel == null)
            {
                return NotFound();
            }

            return View(taiKhoanModel);
        }

        // GET: TaiKhoan/Create
        public IActionResult Create()
        {
            if (!base.KiemTraPhanQuyen("Admin"))
            {
                _nofty.Error("Không đủ quyền truy cập!");
                return base.ChuyenHuong();
            }
            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "RoleName");
            return View();
        }

        // POST: TaiKhoan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Id,UserName,UserPassword,IdRole,Email,PhoneNumber,EmailConfirm,TwoFactorEnabled,TimeLockOut"
            )]
                TaiKhoanModel taiKhoanModel
        )
        {
            if (ModelState.IsValid)
            {
                _context.Add(taiKhoanModel);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (_context.TaiKhoans.Where(tk => tk.UserName == taiKhoanModel.UserName).FirstOrDefault() != null)
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã được sử dụng!");
                    }
                    else if (_context.TaiKhoans.Where(tk => tk.Email == taiKhoanModel.Email).FirstOrDefault() != null)
                    {
                        ModelState.AddModelError("", "Email đã được sử dụng!");
                    }
                    else
                    {
                        ModelState.AddModelError("", $"{ex.Message}");
                    }
                    return View(taiKhoanModel);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRole"] = new SelectList(
                _context.Roles,
                "IdRole",
                "RoleName",
                taiKhoanModel.IdRole
            );
            return View(taiKhoanModel);
        }

        // GET: TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!base.KiemTraPhanQuyen("Admin"))
            {
                return base.ChuyenHuong();
            }
            if (id == null || _context.TaiKhoans == null)
            {
                return NotFound();
            }

            var taiKhoanModel = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoanModel == null)
            {
                return NotFound();
            }
            ViewData["IdRole"] = new SelectList(
                _context.Roles,
                "IdRole",
                "IdRole",
                taiKhoanModel.IdRole
            );
            return View(taiKhoanModel);
        }

        // POST: TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "Id,UserName,UserPassword,IdRole,Email,PhoneNumber,EmailConfirm,TwoFactorEnabled,TimeLockOut"
            )]
                TaiKhoanModel taiKhoanModel
        )
        {
            if (id != taiKhoanModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoanModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanModelExists(taiKhoanModel.Id))
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
            ViewData["IdRole"] = new SelectList(
                _context.Roles,
                "IdRole",
                "IdRole",
                taiKhoanModel.IdRole
            );
            return View(taiKhoanModel);
        }

        // GET: TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!base.KiemTraPhanQuyen("Admin"))
            {
                return base.ChuyenHuong();
            }
            if (id == null || _context.TaiKhoans == null)
            {
                return NotFound();
            }

            var taiKhoanModel = await _context.TaiKhoans
                .Include(t => t.IdRoleNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taiKhoanModel == null)
            {
                return NotFound();
            }

            return View(taiKhoanModel);
        }

        // POST: TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TaiKhoans == null)
            {
                return Problem("Entity set 'QlksdbContext.TaiKhoans'  is null.");
            }
            var taiKhoanModel = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoanModel != null)
            {
                _context.TaiKhoans.Remove(taiKhoanModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanModelExists(int id)
        {
            return (_context.TaiKhoans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Route("/dangnhap.html")]
        public IActionResult DangNhap()
        {
            if (HttpContext.Request.Cookies["DangNhap"] != null)
            {
                return base.ChuyenHuong();
            }
            return View();
        }
        [Route("/dangnhap.html")]
        [HttpPost]
        public async Task<IActionResult> DangNhap(TaiKhoanModel tkModel)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.TaiKhoans
                    .Where(
                        (TaiKhoanModel tk) =>
                            tk.UserName == tkModel.UserName
                            && tk.UserPassword == tkModel.UserPassword
                    )
                    .FirstOrDefaultAsync();
                if (check != null)
                {
                    if (check.TimeLockOut != null && DateTime.Now.CompareTo(check.TimeLockOut) <= 0)
                    {
                        ModelState.AddModelError("", $"Tài khoản của bạn sẽ được mở lại sau ({check.TimeLockOut})");
                        return View(tkModel);
                    }
                    else
                    {
                        if (check.IdRole == -1) // TH: tài khoản bị khóa
                        {
                            ModelState.AddModelError(
                                "name",
                                "Tài khoản của bạn đã bị khóa vui lòng liên hệ admin để được hỗ trợ!"
                            );
                            return View(tkModel);
                        }
                        else
                        {
                            string cookieValue = check.UserName + ";" + check.IdRole;
                            BaseClass bs = new BaseClass();
                            cookieValue = bs.hashString(cookieValue);
                            HttpContext.Response.Cookies.Append(
                                "DangNhap",
                                cookieValue,
                                new CookieOptions { Expires = DateTime.Now.AddMonths(1), Path = "/" }
                            );

                            HttpContext.Response.Cookies.Append(
                                "TokenLogOut",
                                cookieValue,
                                new CookieOptions { Expires = DateTime.Now.AddMonths(1), Path = "/" }
                            );

                            if (Request.Cookies["DangNhap"] != null)
                            {
                                ModelState.AddModelError("", $"Cookie is null. Vui lòng thử với trình duyệt khác!");
                                return View(tkModel);
                            }
                            else
                            {
                                check.CoutLoginFailed = 0;
                                check.TimeLockOut = null;
                                _context.Update(check);
                                try
                                {
                                    await _context.SaveChangesAsync();
                                }
                                catch (SqlException ex)
                                {
                                    ModelState.AddModelError("", $"Lỗi : {ex.Message}");
                                }
                                if (check.IdRole == 99) // tài khoản admin
                                {
                                    return RedirectToAction("Index", "TaiKhoan");
                                }
                                else if (check.IdRole == 0) // tài khoản nhân viên
                                {
                                    if (_context.NhanViens.Where(nv => nv.UserName == check.UserName).FirstOrDefault() == null)
                                    {
                                        _nofty.Warning("Bạn nên cập nhật profile để sử dụng dịch vụ!");
                                        return RedirectToAction(nameof(EditProfile));
                                    }
                                    // redirect về trang đặt phòng
                                    return RedirectToAction("Index", "DatPhong");
                                }
                                else if (check.IdRole == 1) // tài khoản quản lý
                                {
                                    if (_context.NhanViens.Where(nv => nv.UserName == check.UserName).FirstOrDefault() == null)
                                    {
                                        return RedirectToAction(nameof(EditProfile));
                                    }
                                    //redirect về trang quản lý nhân viên
                                    return RedirectToAction("Index", "NhanVien");
                                }
                            }
                        }
                    }
                }
                else
                {
                    // System.Console.WriteLine("bị null rồi!");
                    var test = _context.TaiKhoans.Where(tk => tk.UserName.Equals(tkModel.UserName)).FirstOrDefault();
                    if (test != null)
                    {
                        System.Console.WriteLine("Test : " + test.CoutLoginFailed);
                        if (test.CoutLoginFailed == null)
                        {
                            test.CoutLoginFailed = 1;
                        }
                        else
                        {
                            test.CoutLoginFailed += 1;
                        }
                        System.Console.WriteLine("Test : " + test.CoutLoginFailed);
                        _context.Update(test);
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (SqlException ex)
                        {

                        }
                        if (test.CoutLoginFailed == 5)
                        {
                            // tiến hành khóa tài khoản trong 30 phút
                            test.TimeLockOut = DateTime.Now.AddMinutes(30);
                            test.CoutLoginFailed = 0;
                            _context.Update(test);
                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                _nofty.Error($"Đã xảy lỗi truy vấn! {ex.Message}");
                            }
                            ModelState.AddModelError("", $"Tài khoản của bạn bị giới hạn đăng nhập đến {test.TimeLockOut.ToString()}");
                            return View(tkModel);
                        }
                        ModelState.AddModelError("", $"Mật khẩu không đúng!");
                        return View(tkModel);
                    }
                    ModelState.AddModelError("name", "Sai tên đăng nhập hoặc mật khẩu!");
                    return View(tkModel);
                }
            }
            return View();
        }

        public IActionResult DangXuat()
        {
            if (HttpContext.Request.Cookies["DangNhap"] != null)
            {
                // HttpContext.Response.Cookies.Append("DangNhap", "", new CookieOptions{
                //     Expires = DateTime.Now.AddDays(-1)
                // });
                foreach (var cookie in HttpContext.Request.Cookies)
                {
                    if (cookie.Key.Equals("DangNhap"))
                    {
                        Response.Cookies.Delete(cookie.Key);
                    }
                }
                return RedirectToAction(nameof(DangNhap));
            }
            _nofty.Error("Không tìm thấy cookie phù hợp!");
            return base.ChuyenHuong();
        }

        public IActionResult QuenMatKhau()
        {
            if (!base.KiemTraPhanQuyen(null))
            {
                return View();
            }
            _nofty.Information("Vui lòng đăng xuất trước để sử dụng dịch vụ!");
            return base.ChuyenHuong();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(string email)
        {
            if (_context.TaiKhoans != null)
            {
                var tk = await _context.TaiKhoans
                    .Where(tk => tk.Email == email)
                    .FirstOrDefaultAsync();
                string strCodeEmail = "";
                if (tk != null)
                {
                    TempData["PassModelResetPass"] = tk.UserName;
                    //tiến hành gửi email và và gửi mã code lên ViewData
                    BaseClass bs = new BaseClass();
                    strCodeEmail = bs.randEmailCode();
                    TempData["strCodeEmail"] = strCodeEmail;
                    if (bs.sendEmailGetCode(strCodeEmail, email) == true)
                    {
                        TempData["TokenQuenMatKhau"] = bs.randomToken();
                        _nofty.Success("Email đã được gửi thành công!");
                        return RedirectToAction(nameof(XacThucMail));
                    }
                    else
                    {
                        ModelState.AddModelError("name", "Gửi email thất bại!");
                        return View();
                    }
                }
                else
                {
                    // show thong bao email khong ton tai
                    // ModelState.AddModelError("", "Email không tồn tại!");
                    _nofty.Error("Email không tồn tại!");
                    TempData["PassModelResetPass"] = null;
                }
            }
            return View();
        }

        public IActionResult XacThucMail()
        {
            if (TempData["TokenQuenMatKhau"] == null)
            {
                _nofty.Error("Token không hợp lệ!");
                return RedirectToAction(nameof(QuenMatKhau));
            }
            else
            {
                if (!base.KiemTraPhanQuyen(null))
                {
                    TempData["TokenQuenMatKhau"] = null;
                    return View();
                }
                return base.ChuyenHuong();
            }
        }

        [HttpPost]
        public IActionResult XacThucMail(string code)
        {
            string? strCodeEmail = TempData["strCodeEmail"].ToString();
            if (TempData["strCodeEmail"] != null)
            {
                strCodeEmail = TempData["strCodeEmail"].ToString();
            }
            if (code.Equals(strCodeEmail))
            {
                BaseClass bs = new BaseClass();
                // sang trang đổi mật khẩu
                TempData["TokenQuenMatKhau"] = bs.randomToken();
                return RedirectToAction(nameof(DoiMatKhau));
            }
            ModelState.AddModelError("name", "Code không khớp!");
            //thông báo lỗi
            return View(code);
        }

        public IActionResult DoiMatKhau()
        {
            if (TempData["TokenQuenMatKhau"] == null)
            {
                _nofty.Error("Token không hợp lệ!");
                return RedirectToAction(nameof(QuenMatKhau));
            }
            if (!base.KiemTraPhanQuyen(null))
            {
                return View();
            }
            return base.ChuyenHuong();
        }

        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string pass, string repass)
        {
            if (pass == repass)
            {
                string? tenDN = "";
                if (TempData["PassModelResetPass"] != null)
                {
                    tenDN = TempData["PassModelResetPass"].ToString();
                    var tkResetPass = _context.TaiKhoans
                        .Where(tkModel => tkModel.UserName == tenDN)
                        .FirstOrDefault();
                    if (tkResetPass != null)
                    {
                        tkResetPass.UserPassword = pass;
                        _context.Update(tkResetPass);
                        await _context.SaveChangesAsync();
                        _nofty.Success("Đổi mật khẩu thành công!");
                        return RedirectToAction(nameof(DangNhap));
                        // Thong bao doi mat khau thanh cong!x`
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đổi mật khẩu thất bại!");
                        // Thong bao doi mat khau that bai
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Mật khẩu không khớp!");
                // thong bao mat khau khong khop
            }
            return View();
        }

        public IActionResult EditProfile()
        {
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("QuanLy"))
            {
                BaseClass bs = new BaseClass();
                string maNV = "NV-";
                string tenNV = "";
                string CCCD = "";
                var tenDN = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[0];
                ViewBag.tenDN = tenDN;
                if (_context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault() != null)
                {
                    var nhanVien = _context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault();
                    ViewBag.maNV = nhanVien.MaNv;
                    return View(nhanVien);
                }
                else
                {
                    var nhanVien = _context.NhanViens.OrderByDescending(nv => nv.MaNv).FirstOrDefault();
                    if (nhanVien != null)
                    {
                        maNV = nhanVien.MaNv.Trim();
                        int so = 0;
                        so = Int32.Parse(maNV.Split('-')[1].ToString());
                        int len = so.ToString().Length + 3;
                        string s = "";
                        maNV = "NV-";
                        for (int i = 0; i < 10 - len; i++) s += "0";
                        maNV += s + (so + 1).ToString();
                        ViewBag.maNV = maNV;
                    }
                    return View();
                }

            }
            _nofty.Error("Bạn không đủ quyền để truy cập!");
            return base.ChuyenHuong();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(NhanVienModel nvModel)
        {
            if (ModelState.IsValid)
            {
                BaseClass bs = new BaseClass();
                var tenDN = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[0];
                ViewBag.tenDN = tenDN;
                ViewBag.maNV = nvModel.MaNv;
                if (nvModel.fileUpload != null)
                {
                    var filePath = Path.Combine(_env.WebRootPath, "Images/NhanVien", nvModel.fileUpload.FileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    //lưu dữ liệu fileUpload và stream
                    nvModel.fileUpload.CopyTo(fileStream);
                    nvModel.HinhAnh = nvModel.fileUpload.FileName;
                    // System.Console.WriteLine(nvModel.HinhAnh + " - " + nvModel.TenNv);
                }
                if (_context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault() != null)
                {
                    var nhanVien = _context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault();
                    nhanVien.CCccd = nvModel.CCccd;
                    nhanVien.DiaChi = nvModel.DiaChi;
                    // nhanVien.Email = nvModel.Email;
                    nhanVien.CCccd = nvModel.CCccd;
                    nhanVien.HinhAnh = nvModel.HinhAnh;
                    System.Console.WriteLine(nhanVien.HinhAnh + " - " + nhanVien.TenNv);
                    _context.Update(nhanVien);
                    try{
                        await _context.SaveChangesAsync();
                    }catch{
                        _nofty.Error("Cập nhật thông tin thất bại!");
                        return View(nvModel);
                    }
                    _nofty.Success("Cập nhật thông tin thành công!");
                    return View(nvModel);
                }
                else
                {
                    _context.Add(nvModel);
                    try{
                        await _context.SaveChangesAsync();
                        _nofty.Success("Cập nhật thông tin thành công!");
                    }catch{
                        _nofty.Error("Cập nhật thông tin thất bại!");
                    }
                    return View(nvModel);
                }
            }
            _nofty.Error("Dữ liệu không hợp lệ!");
            return View(nvModel);
        }
    }
}
