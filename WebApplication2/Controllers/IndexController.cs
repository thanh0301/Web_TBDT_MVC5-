using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.MultipleModelInOneView;

using PagedList;


namespace WebApplication2.Controllers
{
    public class IndexController : Controller
    {

        //// GET: Index

        //====================================
        //Tạo 1 đối tượng chứa toàn bộ CSDL từ dbQLBANLKDT
        db2DataContext data = new db2DataContext();


        private List<SANPHAM> LaySanPhamMoi(int count)
        {
            //Sắp xếp giảm dần theo ngày cập nhật, lấy "count" dòng đầu
            return data.SANPHAMs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }

        private List<SANPHAM> LaySanPhamTheoLoai(int id, int count)
        {
            //Sắp xếp giảm dần theo ngày cập nhật, lấy "count" dòng đầu
            return data.SANPHAMs.Where(a => a.LOAISANPHAM.MaLoai == id).Take(count).ToList();
        }

        private List<LOAISANPHAM> LayTatCaLoaiSP()
        {
            return data.LOAISANPHAMs.ToList();
        }

        private List<SANPHAM> LaySPTimKiem(string searchString)
        {
            List<SANPHAM> sanphams = data.SANPHAMs.OrderByDescending(n => n.Ngaycapnhat).ToList();
            if(!string.IsNullOrEmpty(searchString))
            {
                sanphams = (sanphams.Where(n => n.TenSP.Contains(searchString) || n.LOAISANPHAM.TenLoai.Contains(searchString))).ToList();
            }

            return sanphams;
        }

        public ActionResult Index()
        {
            //Phải fix cái này lại nghe =========================== Để từ từ fix
            List<SANPHAM> sanphams = new List<SANPHAM>();
            //Lấy 5 sản phẩm mới nhất
            sanphams = LaySanPhamMoi(5);

            //Lấy Ảnh (Đã Optimize)
            List<ANHPHU> anhphus = new List<ANHPHU>();
            foreach (SANPHAM sp in sanphams)
            {
                //var tempAnh = from ap in data.ANHPHUs where ap.MaSP == sp.MaSP select ap;
                var tempAnh = data.ANHPHUs.First(n => n.MaSP == sp.MaSP);

                anhphus.Add(tempAnh);
            }

            ViewData["anhbia"] = anhphus;


            return View(sanphams);
        }

        //chi tiet san pham
        public ActionResult Details(int id)
        {
            Detail_Mota detail_Mota = new Detail_Mota();

            var sanPham = from sp in data.SANPHAMs
                          where sp.MaSP == id
                          select sp;
            detail_Mota.SANPHAM = sanPham.Single();

            String[] separator = { "<br>" }; //Chữ để tách
            detail_Mota.MotaStrings = detail_Mota.SANPHAM.Mota.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            //Lấy Ảnh (Đã Optimize)
            List<ANHPHU> anhphus = data.ANHPHUs.Where(a => a.MaSP == id).ToList();

            ViewData["anhbia"] = anhphus;

            return View(detail_Mota);
        }

        public ActionResult ListSPDanhMuc(int id , int ? page)
        {
            //Page List
            int pageSize = 8;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            

            var sanPham = from loaiSp in data.LOAISANPHAMs
                          from sp in data.SANPHAMs
                          where loaiSp.MaDM == id && sp.MaLoai == loaiSp.MaLoai
                          select sp;

            //Lấy Ảnh (Đã Optimize)
            List<ANHPHU> anhphus = new List<ANHPHU>();
            foreach(SANPHAM sp in sanPham.ToPagedList(pageNum, pageSize).ToList())
            {
                //var tempAnh = from ap in data.ANHPHUs where ap.MaSP == sp.MaSP select ap;
                var tempAnh = data.ANHPHUs.First(n => n.MaSP == sp.MaSP);

                anhphus.Add(tempAnh);
            }

            ViewData["anhbia"] = anhphus;

            //Lấy Title
            DANHMUC tempDM = data.DANHMUCs.SingleOrDefault(n => n.MaDM == id);
            ViewBag.Name = tempDM.TenDanhMuc;

            return View(sanPham.ToPagedList(pageNum, pageSize));
        }

        public ActionResult ListSPLoai(int id, int? page)
        {
            //Page List
            int pageSize = 8;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            var sanPham = from loaiSp in data.LOAISANPHAMs
                          from sp in data.SANPHAMs
                          where loaiSp.MaLoai == id && sp.MaLoai == loaiSp.MaLoai
                          select sp;

            //Lấy Ảnh (Đã Optimize)
            List<ANHPHU> anhphus = new List<ANHPHU>();
            foreach (SANPHAM sp in sanPham.ToPagedList(pageNum, pageSize).ToList())
            {
                //var tempAnh = from ap in data.ANHPHUs where ap.MaSP == sp.MaSP select ap;
                var tempAnh = data.ANHPHUs.First(n => n.MaSP == sp.MaSP);

                anhphus.Add(tempAnh);
            }

            ViewData["anhbia"] = anhphus;

            //Lấy Title
            LOAISANPHAM tempLoai = data.LOAISANPHAMs.SingleOrDefault(n => n.MaLoai == id);
            ViewBag.Name = tempLoai.DANHMUC.TenDanhMuc;
            ViewBag.NameLoai = tempLoai.TenLoai;

            return View(sanPham.ToPagedList(pageNum, pageSize));
        }


        //partial view
        public ActionResult Chude()

        {
            ModelDanhMucs modelDanhMucs = new ModelDanhMucs();

            modelDanhMucs.DANHMUCs = from cd in data.DANHMUCs select cd;
            modelDanhMucs.LOAISANPHAMs = from lsp in data.LOAISANPHAMs select lsp;



            return PartialView(modelDanhMucs);

        }

        public ActionResult ChudeonMB()

        {

            ModelDanhMucs modelDanhMucsMb = new ModelDanhMucs();

            modelDanhMucsMb.DANHMUCs = from cd in data.DANHMUCs select cd;
            modelDanhMucsMb.LOAISANPHAMs = from lsp in data.LOAISANPHAMs select lsp;



            return PartialView(modelDanhMucsMb);

        }

        //Partial View Các Sản Phẩm Cùng Loại
        public ActionResult subProduct(int id, int count)
        {
            var sanphams = LaySanPhamTheoLoai(id, count);

            var rnd = new Random();
            var sanphamsRandom = sanphams.OrderBy(item => rnd.Next());

            //Lấy Ảnh (Đã Optimize)
            List<ANHPHU> anhphus = new List<ANHPHU>();
            foreach (SANPHAM sp in sanphamsRandom)
            {
                //var tempAnh = from ap in data.ANHPHUs where ap.MaSP == sp.MaSP select ap;
                var tempAnh = data.ANHPHUs.First(n => n.MaSP == sp.MaSP);

                anhphus.Add(tempAnh);
            }

            ViewData["anhbia"] = anhphus;

            return PartialView(sanphamsRandom);
        }

        //Partial View Kênh Youtube
        public ActionResult kenhYoutube(int count)
        {
            var videoYt = data.Videos.Take(count).ToList();

            var rnd = new Random();
            var kenhytRandom = videoYt.OrderBy(item => rnd.Next());


            return PartialView(kenhytRandom);
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

            //Lấy Ảnh (Đã Optimize)
            List<ANHPHU> anhphus = new List<ANHPHU>();
            foreach (SANPHAM sp in sanPham.ToPagedList(pageNum, pageSize).ToList())
            {
                //var tempAnh = from ap in data.ANHPHUs where ap.MaSP == sp.MaSP select ap;
                var tempAnh = data.ANHPHUs.First(n => n.MaSP == sp.MaSP);

                anhphus.Add(tempAnh);
            }

            ViewData["anhbia"] = anhphus;

            //Kiểm tra xem có tìm thấy sản phẩm nào không
            if(sanPham.Count() > 0)
            {
                ViewBag.KetQua = "Kết quả tìm kiếm cho từ khóa: ";
            }
            else
            {
                ViewBag.KetQua = "Không tìm thấy kết quả tìm kiếm cho từ khóa: ";
            }

            ViewBag.TuKhoa =  searchString;

            return View(sanPham.ToPagedList(pageNum, pageSize));

        }
        public ActionResult Video(int ? page)
        {
            var listVideo = data.Videos.OrderByDescending(n => n.NgayCapNhat).ToList();

            //Page List
            int pageSize = 8;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            return View(listVideo.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult ChinhSachVanChuyen()
        {
            return View();
        }
        public ActionResult ChinhSachBaoHanh()
        {
            return View();
        }
    }

}