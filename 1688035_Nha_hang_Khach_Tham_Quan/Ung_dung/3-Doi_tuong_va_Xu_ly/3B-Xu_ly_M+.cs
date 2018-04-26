using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Globalization;
using System.Net;

//************************* M+ (Model for All ) **********************************
public partial class XL_UNG_DUNG
{

    static XL_UNG_DUNG Ung_dung = null;

    XL_DU_LIEU Du_lieu_Ung_dung = null;
    List<XL_KHACH_THAM_QUAN> Danh_sach_Nguoi_dung = new List<XL_KHACH_THAM_QUAN>();

    public static XL_UNG_DUNG Khoi_dong_Ung_dung()
    {
        Ung_dung = new XL_UNG_DUNG();
        Ung_dung.Khoi_dong_Du_lieu_Ung_dung();
        return Ung_dung;
    }
    void Khoi_dong_Du_lieu_Ung_dung()
    {
        var Du_lieu_tu_Dich_vu = XL_DU_LIEU.Doc_Du_lieu();
        Du_lieu_Ung_dung = Du_lieu_tu_Dich_vu;

    }
    public XL_KHACH_THAM_QUAN Khoi_dong_Khach_Tham_quan()
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        if (Khach_Tham_quan == null)
        {
            Khach_Tham_quan = new XL_KHACH_THAM_QUAN();
            Khach_Tham_quan.Danh_sach_Mon_an = Ung_dung.Du_lieu_Ung_dung.Danh_sach_Mon_an;
            Khach_Tham_quan.Danh_sach_Loai_Mon_an = Ung_dung.Du_lieu_Ung_dung.Nha_hang.Danh_sach_Loai_Mon_an;
            HttpContext.Current.Session["Khach_Tham_quan"] = Khach_Tham_quan;
        }

        return Khach_Tham_quan;
    }
    // Xử lý Chức năng của Khách Tham quan :
    public string Khoi_dong_MH_Chinh()
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];

        Khach_Tham_quan.Danh_sach_Mon_an_Xem = Khach_Tham_quan.Danh_sach_Mon_an;
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem();
        return Chuoi_HTML;
    }
    public string Dang_nhap(string Ten_Dang_nhap, string Mat_khau)
    {
        var Chuoi_HTML = $"<div class='alert alert-warning'>Đăng nhập không hợp lệ</div>";
        var Nguoi_dung = Du_lieu_Ung_dung.Danh_sach_Nguoi_dung.FirstOrDefault(
              x => x.Ten_Dang_nhap == Ten_Dang_nhap && x.Mat_khau == Mat_khau);
        if (Nguoi_dung != null)
        {
            var Lan_Dang_nhap = new XL_LAN_DANG_NHAP();
            Nguoi_dung.Danh_sach_Lan_Dang_nhap.Add(Lan_Dang_nhap);
            var Tham_so = $"Th_Ma_so_Chuc_nang=DANG_NHAP&&Th_Ten_Dang_nhap={Ten_Dang_nhap}&&Th_Mat_khau={Mat_khau}";
            if (Nguoi_dung.Nhom_Nguoi_dung.Ma_so == "NHAN_VIEN_PHUC_VU" || Nguoi_dung.Nhom_Nguoi_dung.Ma_so == "DAU_BEP" || Nguoi_dung.Nhom_Nguoi_dung.Ma_so == "QUAN_LY_NHA_HANG")
            {

                var Dia_chi_Xu_ly = $"{Nguoi_dung.Nhom_Nguoi_dung.Dia_chi_Dang_nhap}?{Tham_so}";
                HttpContext.Current.Response.Redirect(Dia_chi_Xu_ly);
            }
            else
            {
                Chuoi_HTML = $"<div class='alert alert-warning'>Chưa thực hiện</div>"; ;
            }


        }
        Chuoi_HTML += Khoi_dong_MH_Chinh();
        return Chuoi_HTML;
    }
    public string Tra_cuu(string Chuoi_Tra_cuu)
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        Khach_Tham_quan.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Chuoi_Tra_cuu, Khach_Tham_quan.Danh_sach_Mon_an);
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem();
        return Chuoi_HTML;
    }

    public string Chon_Loai_Mon_an(string Ma_so_Loai_Mon_an)
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        // Xử lý 
        Khach_Tham_quan.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Ma_so_Loai_Mon_an, Khach_Tham_quan.Danh_sach_Mon_an);
        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem();
        return Chuoi_HTML;
    }
    public string Chuyen_sang_Dat_Cho()
    {
        var Chuoi_HTML = "<iframe class='KHUNG_CHUC_NANG' src='MH_Dat_Cho.cshtml' style='height: 99vh;width: 100vw;border: none;padding-top:50px '  ></iframe>";
        return Chuoi_HTML;
    }
    public string Tao_Chuoi_HTML_Xem()
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        var Chuoi_HTML = $"<section id='mu-restaurant-menu'><div class='container'><div class='row'><div class='col-md-12'>" +
            $"<div class='mu-restaurant-menu-area'><div class='mu-title'><span class='mu-subtitle'>Discover</span><h2>OUR MENU</h2></div>" +
            $"<div class='mu-restaurant-menu-content'>" + $"{ Tao_Chuoi_HTML_Danh_sach_Nhom_Mon_an_Xem()}" +$"{Tao_Chuoi_HTML_Danh_sach_Mon_an()}"+

             $"</div></div></div></div></div></div></section>";
        return Chuoi_HTML;
    }
    public string Dat_cho()
    {
        var Chuoi_HTML = "";
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        var Kq_Ghi = XL_DU_LIEU.Ghi_Dat_Cho_moi(Khach_Tham_quan.Phieu_Dat_Ban, XL_DU_LIEU.Dia_chi_Dich_vu_Khach_Tham_quan);
        if (Kq_Ghi == "OK")
        {
            Kq_Ghi = XL_DU_LIEU.Ghi_Dat_Cho_moi(Khach_Tham_quan.Phieu_Dat_Ban, XL_DU_LIEU.Dia_chi_Phan_he_Quan_ly_Nha_hang);
            if(Kq_Ghi == "OK")
            {
                Chuoi_HTML += $"<div class='alert alert-success'>Bạn đã đặt chỗ thành công</div>";
            }
            else
            {
                Chuoi_HTML += $"<div class='alert alert-warning'>Đã có lỗi xảy ra, vui lòng nhập lại thông tin</div>";
            }
        }
        else
        {
            Chuoi_HTML += $"<div class='alert alert-warning'>Đã có lỗi xảy ra, vui lòng nhập lại thông tin</div>";
        }
        return Chuoi_HTML;
    }

    //-------------------------------------------Dich vu Giao tiep -------------------------------------
    public string Cap_nhat_Gia_ban(string Ma_so_Mon_an, string Gia_moi)
    {
        var Kq = "";

        var Mon_an = Du_lieu_Ung_dung.Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
        Mon_an.Don_gia_Ban = long.Parse(Gia_moi);

        return Kq;
    }
}

//************************* View-Layers/Prsenetaition VL/PL **********************************
public partial class XL_UNG_DUNG
{
    public string Dia_chi_Media = $"{XL_DU_LIEU.Dia_chi_Dich_vu}/Media";
    public CultureInfo Dinh_dang_VN = CultureInfo.GetCultureInfo("vi-VN");

    public string Tao_Chuoi_HTML_Danh_sach_Mon_an()
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        var Danh_sach = Khach_Tham_quan.Danh_sach_Mon_an_Xem;
        var Chuoi_HTML_Danh_sach = "<div class='DANH_SACH'><div class='row'>";

        Danh_sach.ForEach(Mon_an =>
        {
            var Chuoi_Hinh = $"<div class='KHUNG_HINH mx-auto'>" +
                                $"<img src='{Dia_chi_Media}/{Mon_an.Ma_so}.jpg' class='img-thumbnail HINH'/>" +
                             "</div>";

            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>{Mon_an.Ten}</strong>" +
                                      $"<br />Đơn giá: { Mon_an.Don_gia_Ban.ToString("c0", Dinh_dang_VN) }" +
                                  $"</div>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-12 col-sm-6 col-md-4 col-lg-4'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Hinh}" +
                                     $"{Chuoi_Thong_tin}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div></div>";

        return Chuoi_HTML_Danh_sach;
    }
    public string Tao_Chuoi_HTML_Danh_sach_Nhom_Mon_an_Xem()
    {
        var Khach_Tham_quan = (XL_KHACH_THAM_QUAN)HttpContext.Current.Session["Khach_Tham_quan"];
        var Danh_sach = Khach_Tham_quan.Danh_sach_Loai_Mon_an;
        var Chuoi_HTML_Danh_sach = "<div class='text-center'>";
        var Chuoi_Chuc_nang_Chon_Mon_Khai_vi = "";
        var Chuoi_Chuc_nang_Chon_Nuoc_giai_khat = "";
        var Chuoi_Chuc_nang_Chon_Mon_chinh = $"<div class='dropdown' style='display: inline-block;'>" +
                $"<button class='NUT_CHON_LOAI_MON_AN btn btn-default dropdown-toggle' type='button'  style='border-radius: 0; id='dropdownMenu1' data-toggle='dropdown' aria-haspopup='true' aria-expanded='true'><strong>MÓN CHÍNH </strong><span class='caret'></span></button>" +
                $"<ul class='dropdown-menu' aria-labelledby='dropdownMenu1'>";
        Danh_sach.ForEach(Loai_Mon_an =>
        {
            var Chuoi_Xu_ly_Click = $"Th_Ma_so_Loai_Mon_an.value='{Loai_Mon_an.Ma_so}';CHON_LOAI_MON_AN.submit() ";
            if(Loai_Mon_an.Ma_so == "MON_KHAI_VI" )
            {
                Chuoi_Chuc_nang_Chon_Mon_Khai_vi = $"<button type='button' class='NUT_CHON_LOAI_MON_AN btn btn-default' style='border-radius: 0;margin-left:20px;margin-right:20px' onclick=\"" + $"{Chuoi_Xu_ly_Click}" + "\">" +
                            $"<strong>{Loai_Mon_an.Ten}</strong>" +
                         $"</button>";
            }
            else if(Loai_Mon_an.Ma_so == "NUOC_GIAI_KHAT")
            {
                Chuoi_Chuc_nang_Chon_Nuoc_giai_khat = $"<button type='button' class='NUT_CHON_LOAI_MON_AN btn btn-default' style='border-radius: 0;margin-left:20px;margin-right:20px'onclick=\"" + $"{Chuoi_Xu_ly_Click}" + "\">" +
                            $"<strong>{Loai_Mon_an.Ten}</strong>" +
                         $"</button>";
            }
            else
            {
                Chuoi_Chuc_nang_Chon_Mon_chinh += $"<li><a href='#' onclick=\"" + $"{Chuoi_Xu_ly_Click}" + "\">" + $"<strong>{Loai_Mon_an.Ten}</strong>" +"</a></li>";
                
            }
        });
        Chuoi_Chuc_nang_Chon_Mon_chinh += $"</ul></div>";
        Chuoi_HTML_Danh_sach += Chuoi_Chuc_nang_Chon_Mon_Khai_vi + Chuoi_Chuc_nang_Chon_Mon_chinh + Chuoi_Chuc_nang_Chon_Nuoc_giai_khat;
        Chuoi_HTML_Danh_sach += "</div>";
        return Chuoi_HTML_Danh_sach;
    }

}

//************************* Business-Layers BL **********************************
public partial class XL_UNG_DUNG
{
    public List<XL_MON_AN> Tra_cuu_Mon_an(string Chuoi_Tra_cuu, List<XL_MON_AN> Danh_sach)
    {
        Danh_sach = Danh_sach.FindAll(Mon_an => 
            Mon_an.Ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper()) || Mon_an.Ma_so.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper()) || 
            Mon_an.Loai_Mon_an.Ma_so == Chuoi_Tra_cuu
        );
        return Danh_sach;
    }

}

//************************* Data-Layers DL **********************************
public partial class XL_DU_LIEU
{
    public static string Dia_chi_Dich_vu = "http://localhost:50963";
    public static string Dia_chi_Dich_vu_Khach_Tham_quan = $"{Dia_chi_Dich_vu}/1-Dich_vu_Giao_tiep/DV_Khach_Tham_quan.cshtml";
    public static string Dia_chi_Phan_he_Quan_ly_Nha_hang = $"http://localhost:52077/1-Dich_vu_Giao_tiep/DV_Chinh.cshtml";

    public static XL_DU_LIEU Doc_Du_lieu()
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = "Ma_so_Xu_ly=KHOI_DONG_DU_LIEU_KHACH_THAM_QUAN";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Khach_Tham_quan}?{Tham_so}";
        var Chuoi_JSON = Xu_ly.DownloadString(Dia_chi_Xu_ly);
        var Du_lieu = Json.Decode<XL_DU_LIEU>(Chuoi_JSON);

        return Du_lieu;
    }

    public static string Ghi_Dat_Cho_moi(XL_PHIEU_DAT_BAN Phieu_Dat_Cho, string Dia_chi)
    {
        var Kq = "";
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;
        var Tham_so = $"Ma_so_Xu_ly=GHI_PHIEU_DAT_BAN_MOI";
        var Dia_chi_Xu_ly = $"{Dia_chi}?{Tham_so}";
        var Chuoi_JSON = Json.Encode(Phieu_Dat_Cho);
        try
        {
            Kq = Xu_ly.UploadString(Dia_chi_Xu_ly, Chuoi_JSON).Trim();
        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
        }
        return Kq;
    }
}