using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Giohang
    {

        db2DataContext data = new db2DataContext();

        public int iMaSP { set; get; }
        public string sTenSP { set; get; }
        public string sAnhSP { set; get; }
        public Double dDonGia { set; get; }
        public int iSoLuong { set; get; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        public Giohang(int MaSP)
        {
            iMaSP = MaSP;
            SANPHAM sp = data.SANPHAMs.Single(n => n.MaSP == iMaSP);
            sTenSP = sp.TenSP;

            ANHPHU anhBia = data.ANHPHUs.First(n => n.MaSP == iMaSP);
            sAnhSP = anhBia.LinkAnh; //Nhớ Fix
            dDonGia = double.Parse(sp.Giaban.ToString());
            iSoLuong = 1;
        }

    }
}