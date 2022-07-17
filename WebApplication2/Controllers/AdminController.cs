using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace WebApplication2.Controllers
{
    public class AdminController : Controller
    {

        //Tạo 1 đối tượng chứa toàn bộ CSDL từ dbQLBANLKDT
        db2DataContext data = new db2DataContext();

        private List<SANPHAM> LaySPTimKiem(string searchString)
        {
            List<SANPHAM> sanphams = data.SANPHAMs.OrderByDescending(n => n.Ngaycapnhat).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                sanphams = (sanphams.Where(n => n.TenSP.Contains(searchString) || n.LOAISANPHAM.TenLoai.Contains(searchString))).ToList();
            }

            return sanphams;
        }

        // GET: Admin
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["TaikhoanAdmin"] == null || Session["TaikhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("login", "Admin");
            }
            return RedirectToAction("Sanpham");
        }

        public ActionResult Sanpham(int? page)
        {
            if (Session["TaikhoanAdmin"] == null || Session["TaikhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("login", "Admin");
            }

            int pageNumber = (page ?? 1);
            int pageSize = 7;

            ViewData["anhbia"] = data.ANHPHUs.ToList();

            return View(data.SANPHAMs.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng tạo mới (ad)

                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["TaikhoanAdmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Themmoisp()
        {
            if (Session["TaikhoanAdmin"] == null || Session["TaikhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("login", "Admin");
            }

            //Đưa dữ liệu vào dropdownlist
            //Lấy ds từ table LoaiSanPham, sắp xếp tăng dần theo tên loại, chọn lấy giá trị MaLoai hien thi theo TenLoai
            ViewBag.MaLoai = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TenThuongHieu), "MaTH", "TenThuongHieu");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiSp(SANPHAM sp, HttpPostedFileBase fileupload)
        {
            ViewBag.MaLoai = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TenThuongHieu), "MaTH", "TenThuongHieu");
            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/img/Anhphu"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }

                    ANHPHU newAnh = new ANHPHU();
                    newAnh.LinkAnh = fileName;
                    sp.ANHPHUs.Add(newAnh);
                    //Lưu vào CSDL
                    data.SANPHAMs.InsertOnSubmit(sp);
                    data.SubmitChanges();

                }
                return RedirectToAction("Sanpham");
            }
        }

        //Chỉnh sửa sp
     
        public ActionResult Suasp(int id)
        {
            //Lấy ra sp cần sửa
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);

            //Lấy Ảnh
            ANHPHU anhbia = data.ANHPHUs.FirstOrDefault(n => n.MaSP == id);
            ViewBag.AnhBia = anhbia.LinkAnh;

            //Đưa dữ liệu vào dropdownlist
            //Lấy ds từ table LoaiSanPham, sắp xếp tăng dần theo tên loại, chọn lấy giá trị MaLoai hien thi theo TenLoai
            ViewBag.MaLoai = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TenThuongHieu), "MaTH", "TenThuongHieu");

            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }

        [HttpPost, ValidateInput(false), ActionName("Suasp")]
        public ActionResult Xacnhansua(int id, HttpPostedFileBase fileupload)
        {
            //Lấy ra sp cần sửa
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);

            //Lấy ds từ table LoaiSanPham, sắp xếp tăng dần theo tên loại, chọn lấy giá trị MaLoai hien thi theo TenLoai
            ViewBag.MaLoai = new SelectList(data.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TenThuongHieu), "MaTH", "TenThuongHieu");

            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/img/Anhphu"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }

                    ANHPHU newAnh = newAnh = data.ANHPHUs.FirstOrDefault(n => n.MaSP == sp.MaSP);
                    newAnh.LinkAnh = fileName;

                    UpdateModel(sp);
                    data.SubmitChanges();

                }



                return RedirectToAction("Sanpham", new { id = sp.MaSP });
            }
        }


        public ActionResult Chitietsp(int id)
        {
            //Lấy ra đối tượng sp theo mã
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;

            var anhbia = data.ANHPHUs.FirstOrDefault(n => n.MaSP == sanpham.MaSP);
            ViewBag.AnhBia = anhbia.LinkAnh.ToString();
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }

        
        public ActionResult Xoasp(int id)
        {
            //Lấy ra đối tượng sp cần xóa theo mã
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;

            ANHPHU anhbia = data.ANHPHUs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.AnhBia = anhbia.LinkAnh;
            if(sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        //Hàm xóa này còn vấn đề vì sản phẩm có liên kết với bảng ANHPHU !!!!!!!!!!!!!!!!
        [HttpPost, ActionName("Xoasp")]
        public ActionResult Xacnhanxoa(int id)
        {
            //Lấy ra sp cần xóa theo mã sp
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;

            var listAnhPhu = data.ANHPHUs.Where(n => n.MaSP == sp.MaSP);

            if(sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            foreach(ANHPHU item in listAnhPhu)
            {
                data.ANHPHUs.DeleteOnSubmit(item);
            }
            data.SANPHAMs.DeleteOnSubmit(sp);
            
            data.SubmitChanges();
            return RedirectToAction("Sanpham");
        }


        //Partial View Thanh Tìm Kiếm
        public ActionResult timKiem()
        {
            return PartialView();
        }
        public ActionResult TimKiemSP(string searchString, int? page)
        {
            searchString = searchString.ToUpper();

            //Page List
            int pageSize = 8;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            var sanPham = LaySPTimKiem(searchString);

            //Lấy Ảnh 
            ViewData["anhbia"] = data.ANHPHUs.ToList();


            //Kiểm tra xem có tìm thấy sản phẩm nào không
            if (sanPham.Count() > 0)
            {
                ViewBag.KetQua = "Kết quả tìm kiếm cho từ khóa: ";
            }
            else
            {
                ViewBag.KetQua = "Không tìm thấy kết quả tìm kiếm cho từ khóa: ";
            }

            ViewBag.TuKhoa = searchString;

            return View(sanPham.ToPagedList(pageNum, pageSize));

        }

        //===============Quản lí ảnh=================
        public ActionResult Quanlianh(int id)
        {
            if (Session["TaikhoanAdmin"] == null || Session["TaikhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("login", "Admin");
            }

            ViewBag.Tensp = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id).TenSP;

            return View(data.ANHPHUs.Where(n => n.MaSP == id).OrderBy(n => n.IDAnh));
        }

        public ActionResult Themanh(int id)
        {
            if (Session["TaikhoanAdmin"] == null || Session["TaikhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("login", "Admin");
            }

            //Lấy ra sp cần sửa
            SANPHAM sp = data.SANPHAMs.FirstOrDefault(n => n.MaSP == id);


            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);

        }
        [HttpPost, ValidateInput(false), ActionName("Themanh")]
        public ActionResult Xacnhanthem(int id, HttpPostedFileBase fileUpload)
        {
            //Lấy ra sp cần sửa
            SANPHAM sp = data.SANPHAMs.FirstOrDefault(n => n.MaSP == id);

            //Kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/img/Anhphu"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileUpload.SaveAs(path);
                    }

                    ANHPHU newAnh = new ANHPHU();
                    newAnh.LinkAnh = fileName;
                    sp.ANHPHUs.Add(newAnh);

                    UpdateModel(sp);
                    data.SubmitChanges();

                }

                return RedirectToAction("Quanlianh", "Admin", new { id = sp.MaSP });
            }
        }


        public ActionResult Xoaanh(int id, int maSP)
        {
            //Lấy ra đối tượng ảnh cần xóa theo mã
            ANHPHU anh = data.ANHPHUs.SingleOrDefault(n => n.IDAnh == id);

            if (anh == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            data.ANHPHUs.DeleteOnSubmit(anh);
            data.SubmitChanges();

            return RedirectToAction("Quanlianh",new { id  = maSP});
        }
        
    }

}