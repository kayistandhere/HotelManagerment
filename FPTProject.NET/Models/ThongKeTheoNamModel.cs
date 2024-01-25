using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ThongKeTheoNamModel
    {
        public int TongSoKHTrongNam { get; set; }
        public int TongSoPhongTrongNam { get; set; }
        public decimal? TongDoanhThuTrongNam { get; set; }
        public int TongSoNgayDatDoanhThuTheoNam { get; set; }
    }
}