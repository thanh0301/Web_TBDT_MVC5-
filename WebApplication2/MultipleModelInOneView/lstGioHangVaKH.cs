using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.MultipleModelInOneView
{
    public class lstGioHangVaKH
    {
        public KHACHHANG  KhachHang { get; set; }
        public List<Giohang> ListGioHang { get; set; }
    }
}