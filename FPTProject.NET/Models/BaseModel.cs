using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class BaseModel
    {
        public PhongModel phongs {get;set;}
        public TaiKhoanModel taikhoans { get; set; }
        
    }
}