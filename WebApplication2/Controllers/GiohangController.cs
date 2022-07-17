using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.MultipleModelInOneView;

namespace WebApplication2.Controllers
{
    public class GiohangController : Controller
    {
        db2DataContext data = new db2DataContext();
        //Lay gio hang
        public List<Giohang> Laygiohang()
        {
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if(listGiohang == null)
            {
                //Neu gio hang rong thi khoi tao Giohang
                listGiohang = new List<Giohang>();
                Session["Giohang"] = listGiohang;
            }
            return listGiohang;
        }

        //Them vao gio hang
        public ActionResult ThemGiohang(int iMaSP, string strURL)
        {
            //Lay ra Session gio hang
            List<Giohang> listGiohang = Laygiohang();
            //Kiểm tra sách này tồn tại trong Session["Giohang"] chưa ?
            Giohang sanpham = listGiohang.Find(n => n.iMaSP == iMaSP);
            if(sanpham == null)
            {
                sanpham = new Giohang(iMaSP);
                listGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong ++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> listGiohang = Session["GioHang"] as List<Giohang>;
            if(listGiohang != null)
            {
                iTongSoLuong = listGiohang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> listGiohang = Session["GioHang"] as List<Giohang>;

            if(listGiohang != null)
            {
                iTongTien = listGiohang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        //Xay dung trang Gio Hang
        public ActionResult GioHang()
        {
            List<Giohang> listGiohang = Laygiohang();
            if(listGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Index");
            }
            ViewBag.TongsoLuong = TongSoLuong();
            ViewBag.Tongtien = TongSoLuong();
            return View(listGiohang);
        }

        public ActionResult GiohangPartical()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> listGiohang = Laygiohang();
            Giohang sanpham = listGiohang.SingleOrDefault(n => n.iMaSP == iMaSP);
            if(sanpham != null)
            {
                listGiohang.RemoveAll(n => n.iMaSP == iMaSP);
                return RedirectToAction("GioHang");
            }
            if(listGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Index");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<Giohang> listGiohang = Laygiohang();
            Giohang sanpham = listGiohang.SingleOrDefault(n => n.iMaSP == iMaSP);

            if(sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatCaGiohang()
        {
            List<Giohang> listGiohang = Laygiohang();
            listGiohang.Clear();
            return RedirectToAction("Index", "Index");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if(Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("login", "User");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Index");
            }

            lstGioHangVaKH lstGioHangVaKH = new lstGioHangVaKH();

            //Lay gio hang tu Session
            //Lấy KHACHHANG thông qua TAIKHOAN
            TAIKHOAN tk = (TAIKHOAN)Session["Taikhoan"];
            KHACHHANG kh = data.KHACHHANGs.FirstOrDefault(n => n.MaKH == tk.MaKH);

            lstGioHangVaKH.KhachHang = kh;

            lstGioHangVaKH.ListGioHang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGioHangVaKH);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don Hang
            DONDATHANG ddh = new DONDATHANG();
            //Lấy KHACHHANG thông qua TAIKHOAN
            TAIKHOAN tk = (TAIKHOAN)Session["Taikhoan"];
            KHACHHANG kh = data.KHACHHANGs.FirstOrDefault(n => n.MaKH == tk.MaKH);

            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach(var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSP = item.iMaSP;
                ctdh.Soluong = item.iSoLuong;
                ctdh.Dongia = (decimal)item.dDonGia;
                data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}