using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;

//************************* M+ (Model for All ) **********************************
public partial class XL_UNG_DUNG
{
    static XL_UNG_DUNG Ung_dung = null;

    XL_DU_LIEU Du_lieu_Ung_dung = null;
    List<XL_NGUOI_DUNG> Danh_sach_Nguoi_dung = new List<XL_NGUOI_DUNG>();

    public static XL_UNG_DUNG Khoi_dong_Ung_dung()
    {
        Ung_dung = new XL_UNG_DUNG(); // Không caching 
        Ung_dung.Khoi_dong_Du_lieu_Ung_dung();
        return Ung_dung;
    }
    void Khoi_dong_Du_lieu_Ung_dung()
    {
        var Nguoi_dung_Dang_nhap = (XL_NGUOI_DUNG)HttpContext.Current.Session["Nguoi_dung_Dang_nhap"];
        var Du_lieu_tu_Dich_vu = XL_DU_LIEU.Doc_Du_lieu();
        Du_lieu_Ung_dung = Du_lieu_tu_Dich_vu;
        //Bổ sung Thông tin cần thiết cho Tất cả người dùng 
        //===> khi xử lý Chức năng của Người dùng đăng nhập không cần đến Dữ liệu của Ứng dụng 
        //Du_lieu_Ung_dung.Danh_sach_Ban.ForEach(Ban =>
        //{
        //    Ban.Danh_sach_Mon_an_Cho_nau = Tao_Danh_Sach_Mon_an_Cho_nau_cua_Ban(Ban, Du_lieu_Ung_dung.Danh_sach_Mon_an);
        //});
        Danh_sach_Nguoi_dung = Du_lieu_Ung_dung.Danh_sach_Nguoi_dung.FindAll(x=>x.Nhom_Nguoi_dung.Ma_so == "DAU_BEP");
        Danh_sach_Nguoi_dung.ForEach(Nguoi_dung =>
        {
            Du_lieu_Ung_dung.Danh_sach_Ban.ForEach(Ban => {
                var Ban_moi = new XL_BAN();
                Ban_moi.Ma_so = Ban.Ma_so;
                Ban_moi.Ten = Ban.Ten;
                Ban_moi.Trang_thai = Ban.Trang_thai;
                Nguoi_dung.Danh_sach_Ban.Add(Ban_moi);
            });
            Nguoi_dung.Danh_sach_Mon_an = Du_lieu_Ung_dung.Danh_sach_Mon_an.FindAll(Mon_an => Nguoi_dung.Danh_sach_Loai_Mon_an.Any(Loai_Mon_an=>Loai_Mon_an.Ma_so == Mon_an.Loai_Mon_an.Ma_so));
            Nguoi_dung.Danh_sach_Ban.ForEach(x =>
            {
                x.Danh_sach_Mon_an_Cho_nau = Tao_Danh_Sach_Mon_an_Cho_nau_cua_Ban(x, Nguoi_dung.Danh_sach_Mon_an);
            });
            if (Nguoi_dung_Dang_nhap != null && Nguoi_dung_Dang_nhap.Ma_so == Nguoi_dung.Ma_so)
            {
                HttpContext.Current.Session["Nguoi_dung_Dang_nhap"] = Nguoi_dung;
            }
        });
    }
    //============= Xử lý Chức năng của Người dùng đăng nhập ==============
    //Lưu ý Quan trọng : Tất cả thông tin xử lý phải dựa vào thông tin của chính Người dùng đăng nhập 
    public XL_NGUOI_DUNG Dang_nhap(string Ten_Dang_nhap, string Mat_khau)
    {
        var Nguoi_dung = Danh_sach_Nguoi_dung.FirstOrDefault(
                                x => x.Ten_Dang_nhap == Ten_Dang_nhap
                                      && x.Mat_khau == Mat_khau
                                      && x.Nhom_Nguoi_dung.Ma_so == "DAU_BEP");

        if (Nguoi_dung != null)
        {   //Khởi động  Thông tin Online  
            
            Nguoi_dung.Danh_sach_Ban_Xem = Nguoi_dung.Danh_sach_Ban;
            HttpContext.Current.Session["Nguoi_dung_Dang_nhap"] = Nguoi_dung;
        }

        return Nguoi_dung;
    }
    public string Khoi_dong_Man_hinh_chinh()
    {
        var Nguoi_dung_Dang_nhap = (XL_NGUOI_DUNG)HttpContext.Current.Session["Nguoi_dung_Dang_nhap"];
        // Xử lý 
        //Nguoi_dung_Dang_nhap.Danh_sach_Ban = Ung_dung.Du_lieu_Ung_dung.Danh_sach_Ban;
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Nguoi_dung_Dang_nhap.Danh_sach_Ban;
        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Tra_cuu(string Chuoi_Tra_cuu)
    {
        var Nguoi_dung_Dang_nhap = (XL_NGUOI_DUNG)HttpContext.Current.Session["Nguoi_dung_Dang_nhap"];
        // Xử lý 
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Tra_cuu_Ban(Chuoi_Tra_cuu, Nguoi_dung_Dang_nhap.Danh_sach_Ban);
        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }
    public string Check_Da_Nau(string Ma_so_Ban,string Ma_so_Mon_an, string Ma_so_Goi_mon)
    {
        var Nguoi_dung_Dang_nhap = (XL_NGUOI_DUNG)HttpContext.Current.Session["Nguoi_dung_Dang_nhap"];
        var Kq_Ghi = XL_DU_LIEU.Ghi_Trang_thai_Da_Nau(Ma_so_Ban, Ma_so_Mon_an, Ma_so_Goi_mon, Nguoi_dung_Dang_nhap);
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Xem(XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        var Chuoi_HTML = $"<div>" +
                //$"{ Tao_Chuoi_HTML_Nguoi_dung_Dang_nhap(Nguoi_dung_Dang_nhap)}" +
                $"{ Tao_Chuoi_HTML_Danh_sach_Cho_nau(Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem,Nguoi_dung_Dang_nhap.Danh_sach_Mon_an)}" +
            $"</div>";
        return Chuoi_HTML;
    }
}

//************************* View-Layers/Prsenetaition VL/PL **********************************
public partial class XL_UNG_DUNG
{
    public string Dia_chi_Media = $"{XL_DU_LIEU.Dia_chi_Dich_vu}/Media";
    public CultureInfo Dinh_dang_VN = CultureInfo.GetCultureInfo("vi-VN");

    public string Tao_Chuoi_HTML_Nguoi_dung_Dang_nhap(XL_NGUOI_DUNG Nguoi_dung)
    {
        var Chuoi_Hinh = $"<img src='{Dia_chi_Media}/{Nguoi_dung.Ma_so}.png' " +
                 " class='AVATAR' />";
        var Chuoi_Thong_tin = $"{Nguoi_dung.Nhom_Nguoi_dung.Ten } {Nguoi_dung.Ho_ten}";

        var Chuoi_HTML = $"<div class='col-md-5 NGUOI_DUNG'>" +
                        $"{Chuoi_Thong_tin}"+
                           $"{Chuoi_Hinh}" +

                          $"</div>";
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Cho_nau(List<XL_BAN> Danh_sach_Ban, List<XL_MON_AN> Danh_sach_Mon_an)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach_Ban.ForEach(Ban =>
        {
            
            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>{Ban.Ten}</strong>";
            var Chuoi_Mon_an_Chua_nau = $"<strong class='col-md-12 text-left' style='color:red;'>[CHỜ NẤU]</strong>";
            var Chuoi_Mon_an_Da_nau = $"<strong class='col-md-12 text-left' style='color:yellow;'>[ĐÃ NẤU]</strong>";
            if (Ban.Trang_thai == "TRONG")
            {
                Chuoi_Thong_tin += $"<br/><p style='color:green;'>TRỐNG</p>" +
                                                  $"</div>";
            }
                
            else
            {
                Chuoi_Thong_tin += $"<br/><p style='color:orange;'>CÓ KHÁCH</p>" +
                                                  $"</div>";
                Chuoi_Thong_tin += $"<div class='row'>";
                Ban.Danh_sach_Mon_an_Cho_nau.ForEach(Mon_an =>
                {
                    Tao_chuoi_Mon_an_cua_Ban(Mon_an, Ban, ref Chuoi_Mon_an_Chua_nau, ref Chuoi_Mon_an_Da_nau);

                });
                Chuoi_Thong_tin += Chuoi_Mon_an_Chua_nau + Chuoi_Mon_an_Da_nau;
                Chuoi_Thong_tin += $"</div>";
            }
            var Chuoi_HTML = $"<div class='KHUNG col-xs-12 col-sm-6 col-md-4 col-lg-4'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Thong_tin}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public void Tao_chuoi_Mon_an_cua_Ban(XL_MON_AN Mon_an, XL_BAN Ban, ref string Chuoi_Mon_an_Chua_nau, ref string Chuoi_Mon_an_Da_nau)
    {
        var Chuoi_HTML = "";
        foreach(XL_GOI_MON Goi_mon in Mon_an.Danh_sach_Goi_mon)
        {
            var Chuoi_Goi_mon = $"<div class='col-md-7'>{Mon_an.Ten} x{Mon_an.Danh_sach_Goi_mon[0].So_luong} ";
            if (Goi_mon.Trang_thai == "CHO_NAU")
            {
                var Chuoi_Xu_ly_Click = $"Th_Ma_so_Ban.value='{Ban.Ma_so}';Th_Ma_so_Mon_an.value='{Mon_an.Ma_so}';Th_Ma_so_Goi_mon.value='{Goi_mon.Ma_so}';CHECK_DA_NAU.submit() ";
                //Chuoi_Goi_mon += $"<span style='color:red;'>[CHỜ NẤU]</span>";
                Chuoi_Goi_mon += $"</div><div class='col-md-2'>{Goi_mon.Thoi_diem_Goi.ToString("HH:mm")}</div> " +
                $"<div class='col-md-3'><button type='button' class='btn btn-outline-success btn-sm' onclick=\"" +
                $"{Chuoi_Xu_ly_Click}" + $"\">Xong</button></div>";
                Chuoi_Mon_an_Chua_nau += Chuoi_Goi_mon;
            }
            else if (Goi_mon.Trang_thai == "DA_NAU")
            {
                //Chuoi_Goi_mon += $"<span style='color:yellow;'>[ĐÃ NẤU]</span>";
                Chuoi_Goi_mon += "</div>";
                Chuoi_Goi_mon += $"<div class='col-md-2'>{Goi_mon.Thoi_diem_Nau_xong.ToString("HH:mm")}</div>";
                Chuoi_Mon_an_Da_nau += Chuoi_Goi_mon;
            }
            Chuoi_HTML += Chuoi_Goi_mon;
        }
    }
}

//************************* Business-Layers BL **********************************
public partial class XL_UNG_DUNG
{
    public XL_DAU_BEP Dang_nhap_Dau_bep(string Chuoi_Ten_Dang_nhap, string Chuoi_Mat_khau, List<XL_DAU_BEP> Danh_sach_Dau_bep)
    {
        var Dau_bep_Dang_nhap = Danh_sach_Dau_bep.FirstOrDefault(Dau_bep => Dau_bep.Ten_Dang_nhap == Chuoi_Ten_Dang_nhap && Dau_bep.Mat_khau == Chuoi_Mat_khau);
        return Dau_bep_Dang_nhap;
    }

    public object Nguoi_dung_Dang_nhap()
    {
        return HttpContext.Current.Session["Nguoi_dung"];
    }


    public List<XL_BAN> Tra_cuu_Ban(string Chuoi_Tra_cuu, List<XL_BAN> Danh_sach)
    {
        Danh_sach = Danh_sach.FindAll(Ban =>
            Ban.Ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper()) || Ban.Ma_so.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
        );
        return Danh_sach;
    }
    public XL_BAN Tim_Ban(string Chuoi_Tra_cuu, List<XL_BAN> Danh_sach)
    {
        var Ban_Tim = Danh_sach.FirstOrDefault(Ban =>
            Ban.Ma_so == Chuoi_Tra_cuu);
        return Ban_Tim;
    }
    public XL_MON_AN Tim_Mon_an(string Chuoi_Tra_cuu, List<XL_MON_AN> Danh_sach)
    {
        var Mon_an_Tim = Danh_sach.FirstOrDefault(Mon_an =>
            Mon_an.Ma_so == Chuoi_Tra_cuu);
        return Mon_an_Tim;
    }

    public List<XL_MON_AN> Tao_Danh_Sach_Mon_an_Cho_nau_cua_Ban(XL_BAN Ban, List<XL_MON_AN> Danh_sach_Mon_an)
    {
        //var Kq_ = Danh_sach_Mon_an.FindAll(Mon_an => Mon_an.Danh_sach_Goi_mon.Any(Goi_mon => Goi_mon.Trang_thai != "DA_THANH_TOAN" && 
        //Goi_mon.Trang_thai != "HUY" && Goi_mon.Ban.Ma_so == Ban.Ma_so));
        var Kq = new List<XL_MON_AN>();
        Danh_sach_Mon_an.ForEach(Mon_an =>
        {
            var Mon_an_Goi = new XL_MON_AN();
            Mon_an_Goi.Ten = Mon_an.Ten;
            Mon_an_Goi.Ma_so = Mon_an.Ma_so;
            Mon_an.Danh_sach_Goi_mon.ForEach(Goi_mon=>{
                if(Goi_mon.Ban.Ma_so == Ban.Ma_so && Goi_mon.Trang_thai != "HUY" && Goi_mon.Trang_thai != "DA_PHUC_VU" && Goi_mon.Trang_thai != "DA_THANH_TOAN")
                {
                    
                    Mon_an_Goi.Danh_sach_Goi_mon.Add(Goi_mon);
                    
                }

            });
            if(Mon_an_Goi.Danh_sach_Goi_mon.Count != 0)
                Kq.Add(Mon_an_Goi);
        });
        return Kq;
    }

    
}

//************************* Data-Layers DL **********************************
public partial class XL_DU_LIEU
{
    public static string Dia_chi_Dich_vu = "http://localhost:50963";
    public static string Dia_chi_Dich_vu_Nhan_vien_Phuc_vu = $"{Dia_chi_Dich_vu}/1-Dich_vu_Giao_tiep/DV_Dau_bep.cshtml";

    public static XL_DU_LIEU Doc_Du_lieu()
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = "Ma_so_Xu_ly=KHOI_DONG_DU_LIEU_DAU_BEP";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Chuoi_JSON = Xu_ly.DownloadString(Dia_chi_Xu_ly);
        var Du_lieu = JsonConvert.DeserializeObject<XL_DU_LIEU>(Chuoi_JSON);

        return Du_lieu;
    }

    public static string Ghi_Trang_thai_Da_Nau(string Ma_so_Ban, string Ma_so_Mon_an, string Ma_so_Goi_mon, XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        var Kq = "";
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;
        var Tham_so = $"Ma_so_Xu_ly=GHI_TRANG_THAI_DA_NAU&Ma_so_Mon_an={Ma_so_Mon_an}&Ma_so_Goi_mon={Ma_so_Goi_mon}";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        try
        {
            Kq = Xu_ly.DownloadString(Dia_chi_Xu_ly);
        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
        }
        if (Kq == "OK\r\n")
        {
            var Ban = Nguoi_dung_Dang_nhap.Danh_sach_Ban.FirstOrDefault(x=>x.Ma_so == Ma_so_Ban);
            var Mon_an = Ban.Danh_sach_Mon_an_Cho_nau.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
            var Goi_mon_Chon = Mon_an.Danh_sach_Goi_mon.FirstOrDefault(x => x.Ma_so == Ma_so_Goi_mon);
            Goi_mon_Chon.Trang_thai = "DA_NAU";
        }
        return Kq;
    }
}