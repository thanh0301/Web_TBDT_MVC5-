using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class UserController : Controller
    {
        //Tạo 1 đối tượng chứa toàn bộ CSDL từ dbQLBANLKDT
     
        db2DataContext data = new db2DataContext();
        // GET: User
        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult login()
        {
            if(Session["Taikhoan"] != null)
            {
                return RedirectToAction("Index", "Index");
            }
            return View();
        }
        public ActionResult signup()
        {
            //code sql 

            return View();
        }

        public ActionResult logout()
        {
            Session["Taikhoan"] = null;

            return RedirectToAction("login", "User");
        }

        [HttpPost]
        public ActionResult login(FormCollection collection)
        {
            //Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["name"];
            var matkhau = collection["pass"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Vui lòng nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Vui lòng nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (kh)
                TAIKHOAN tk = data.TAIKHOANs.SingleOrDefault(n => n.Taikhoan1 == tendn && n.Matkhau == EncodePassword(matkhau));

                if (tk != null)
                {
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = tk;
                    return RedirectToAction("Index", "Index");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        [HttpPost]
        public ActionResult signup(FormCollection collection, KHACHHANG kh)
        {
            //Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["name"];
            var hoten = collection["hoten"];
            var matkhau = collection["pass"];
            var matkhaunhaplai = collection["rpass"];
            var sdt = collection["sdt"];
            var email = collection["email"];
            var diachi = collection["diachi"];

            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Vui lòng nhập tên đăng nhập";
            }else if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi2"] = "Vui lòng nhập tên khách hàng";
            }
            else if(String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Vui lòng nhập mật khẩu";
            }else if (String.IsNullOrEmpty(matkhaunhaplai) || matkhaunhaplai != matkhau)
            {
                ViewData["Loi4"] = "Vui lòng nhập lại mật khẩu";
            }
            else if(String.IsNullOrEmpty(sdt))
            {
                ViewData["Loi5"] = "Số điện thoại không được bỏ trống";
            }else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi6"] = "Email không được bỏ trống";
            }
            else
            {
                TAIKHOAN newTK = new TAIKHOAN();
                newTK.Taikhoan1 = tendn;
                newTK.Matkhau = EncodePassword(matkhau); //Mã hóa MK

                kh.TAIKHOANs.Add(newTK);

                kh.HoTen = hoten;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = sdt;

                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("login");
            }
            return this.signup();
        }
        public ActionResult loginPartial()
        {
            
            if(Session["Taikhoan"] != null)
            {
                TAIKHOAN taikhoan = (TAIKHOAN)Session["Taikhoan"];
                ViewBag.TenDN = taikhoan.Taikhoan1;
            }
            else
            {
                ViewBag.TenDN = "login";
            }

            return PartialView();
        }

    }
}