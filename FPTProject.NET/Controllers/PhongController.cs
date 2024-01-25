using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;
using Hotel.Base;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Data.SqlClient;

namespace Hotel.Controllers
{
    public class PhongController : BaseController
    {
         private readonly INotyfService _notyf;
         private readonly IWebHostEnvironment _env;
        public PhongController(QlksdbContext context, INotyfService notyf, IWebHostEnvironment env)
            : base(context) 
        {
            _notyf = notyf;
            _env = env;
        }

        // GET: Phong
        public async Task<IActionResult> Index(
            string searchString,
            string currentFilter,
            int? pageNumber
        )
        {
            if (
                base.KiemTraPhanQuyen("Admin")
                || base.KiemTraPhanQuyen("NhanVien")
                || base.KiemTraPhanQuyen("QuanLy")
            )
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
                var phongs = (from p in _context.Phongs orderby p.SoPhong descending select p)
                    .Include(k => k.MaLpNavigation)
                    .Include(k => k.MaTvpNavigation);
                if (!String.IsNullOrEmpty(searchString))
                {
                    phongs = phongs
                        .Where(p => p.SoPhong.ToString().Contains(searchString))
                        .OrderByDescending(p => p.SoPhong)
                        .Include(k => k.MaLpNavigation)
                        .Include(k => k.MaTvpNavigation);
                }
                int pageSize = 12;
                return View(
                    await PaginatedList<PhongModel>.CreateAsync(
                        phongs.AsNoTracking(),
                        pageNumber ?? 1,
                        pageSize
                    )
                );
            }
            return base.ChuyenHuong();
        }

        // GET: Phong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Phongs == null)
            {
                return NotFound();
            }
            var phong = await _context.Phongs
                .Include(p => p.MaLpNavigation)
                .Include(p => p.MaTvpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phong == null)
            {
                return NotFound();
            }
            return View(phong);
        }

        // GET: Phong/Create
        public IActionResult Create()
        {
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp");
            ViewData["MaTvp"] = new SelectList(_context.TacVuPhongs, "MaTvp", "TenTvp");
            return View();
        }

        // POST: Phong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
                PhongModel phong
        )
        {
            phong.CreatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (phong.fileUpload != null)
                {
                    var filePath = Path.Combine(_env.WebRootPath, "Images/Phong", phong.fileUpload.FileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    //lưu dữ liệu fileUpload và stream
                    phong.fileUpload.CopyTo(fileStream);
                    phong.HinhAnh = phong.fileUpload.FileName;
                    // System.Console.WriteLine(phong.HinhAnh + " - " + phong.TenNv);
                }
                // ModelState.AddModelError("abc", "Loi");
                try{
                    _context.Add(phong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }catch(Exception ex)
                {
                    _notyf.Error($"Số phòng đã tồn tại! {ex.Message}");
                    return View(phong);
                }
            }
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp", phong.MaLp);
            ViewData["MaTvp"] = new SelectList(
                _context.TacVuPhongs,
                "MaTvp",
                "TenTvp",
                phong.MaTvp
            );

            return View(phong);
        }

        // GET: Phong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Phongs == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null)
            {
                return NotFound();
            }
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp", phong.MaLp);
            ViewData["MaTvp"] = new SelectList(
                _context.TacVuPhongs,
                "MaTvp",
                "TenTvp",
                phong.MaTvp
            );
            return View(phong);
        }

        // POST: Phong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,MaP,SoPhong,HinhAnh,TrangThaiPhong,MaLp,MaTvp,MoTa, LoaiGiuong")]
                PhongModel phong
        )
        {
            phong.UpdatedAt = DateTime.Now;
            if (id != phong.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.Id))
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
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp", phong.MaLp);
            ViewData["MaTvp"] = new SelectList(
                _context.TacVuPhongs,
                "MaTvp",
                "TenTvp",
                phong.MaTvp
            );
            return View(phong);
        }

        // POST: Phong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Phongs == null)
            {
                return Problem("Entity set 'QlksContext.Phongs'  is null.");
            }
            try
            {
                var phong = await _context.Phongs.FindAsync(id);
                ViewData["error"] = "Méo lỗi";
                if (phong != null)
                {
                    _context.Phongs.Remove(phong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewData["error"] = "Phòng đã đặt không thể xoá!";
            }
            return View();
        }

        [HttpGet]
        public IActionResult DanhSachPhongDatPhong()
        {
            var phong = new List<PhongModel>();
            if (_context.Phongs != null)
            {
                phong = _context.Phongs
                    .Where(p => p.TrangThaiPhong == 1)
                    .Include(p => p.MaLpNavigation)
                    .OrderByDescending(p => p.SoPhong)
                    .ToList();
            }
            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhSachPhongDatPhong(int? id)
        {
            //1 là rảnh, 0 là đã đặt, 2 là tất cả
            var listPhong = new List<PhongModel>();
            if (_context.Phongs != null)
            {
                listPhong = await _context.Phongs
                    .Where(p => p.TrangThaiPhong == 1)
                    .Include(p => p.MaLpNavigation)
                    .OrderByDescending(p => p.SoPhong)
                    .ToListAsync();
            }
            return View(listPhong);
        }

        private bool PhongExists(int id)
        {
            var a = (_context.Phongs?.Any(e => e.Id == id)).GetValueOrDefault();
            return (_context.Phongs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
