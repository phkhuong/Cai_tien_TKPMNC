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
    List<XL_NGUOI_DUNG> Danh_sach_Nguoi_dung = new List<XL_NGUOI_DUNG>();

    public static XL_UNG_DUNG Khoi_dong_Ung_dung()
    {
        Ung_dung = new XL_UNG_DUNG(); // Không caching 
        Ung_dung.Khoi_dong_Du_lieu_Ung_dung();

        return Ung_dung;
    }

    void Khoi_dong_Du_lieu_Ung_dung()
    {
        var Du_lieu_tu_Dich_vu = XL_DU_LIEU.Doc_Du_lieu();
        Du_lieu_Ung_dung = Du_lieu_tu_Dich_vu;

        var Nguoi_dung_Dang_nhap = (XL_NGUOI_DUNG)HttpContext.Current.Session["Nguoi_dung_Dang_nhap"];

        var Danh_sach_Loai_Mon_an = Du_lieu_Ung_dung.Nha_hang.Danh_sach_Loai_Mon_an;
        Danh_sach_Loai_Mon_an.ForEach(Loai_Mon_an => {
            Loai_Mon_an.So_luong_Mon = Du_lieu_Ung_dung.Danh_sach_Mon_an.Count(x => x.Loai_Mon_an.Ma_so == Loai_Mon_an.Ma_so);
        });

        //Bổ sung Thông tin cần thiết cho Tất cả người dùng 
        //===> khi xử lý Chức năng của Người dùng đăng nhập không cần đến Dữ liệu của Ứng dụng 
        Danh_sach_Nguoi_dung = Du_lieu_Ung_dung.Danh_sach_Nguoi_dung;
        Danh_sach_Nguoi_dung.ForEach(Nguoi_dung =>
        {
            if(Nguoi_dung.Nhom_Nguoi_dung.Ma_so == "QUAN_LY_NHA_HANG")
            {
                Nguoi_dung.Danh_sach_Nhan_vien_Phuc_vu = Danh_sach_Nguoi_dung.FindAll(x => x.Nhom_Nguoi_dung.Ma_so == "NHAN_VIEN_PHUC_VU");
                Nguoi_dung.Danh_sach_Loai_Mon_an = Danh_sach_Loai_Mon_an;
                Nguoi_dung.Danh_sach_Loai_Mon_an_Xem = Nguoi_dung.Danh_sach_Loai_Mon_an;
                Nguoi_dung.Danh_sach_Mon_an = Du_lieu_Ung_dung.Danh_sach_Mon_an;
                Nguoi_dung.Danh_sach_Mon_an_Xem = Nguoi_dung.Danh_sach_Mon_an;
                Nguoi_dung.Danh_sach_Ban = Du_lieu_Ung_dung.Danh_sach_Ban;
                Nguoi_dung.Danh_sach_Ban_Xem = Nguoi_dung.Danh_sach_Ban;
                Nguoi_dung.Danh_sach_Phieu_Dat_ban = Du_lieu_Ung_dung.Danh_sach_Phieu_Dat_ban;
                Nguoi_dung.Danh_sach_Phieu_Dat_ban_Xem = Nguoi_dung.Danh_sach_Phieu_Dat_ban;
                Nguoi_dung.Danh_sach_Y_kien = Du_lieu_Ung_dung.Danh_sach_Y_kien;
                Nguoi_dung.Danh_sach_Y_kien_Xem = Nguoi_dung.Danh_sach_Y_kien;

                if(Nguoi_dung_Dang_nhap != null && Nguoi_dung_Dang_nhap.Ma_so == Nguoi_dung.Ma_so)
                {
                    Nguoi_dung_Dang_nhap.Danh_sach_Nhan_vien_Phuc_vu = Nguoi_dung.Danh_sach_Nhan_vien_Phuc_vu;
                    Nguoi_dung_Dang_nhap.Danh_sach_Loai_Mon_an = Danh_sach_Loai_Mon_an;
                    Nguoi_dung_Dang_nhap.Danh_sach_Mon_an = Nguoi_dung.Danh_sach_Mon_an;
                    Nguoi_dung_Dang_nhap.Danh_sach_Ban = Nguoi_dung.Danh_sach_Ban;
                    Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban = Nguoi_dung.Danh_sach_Phieu_Dat_ban;
                    Nguoi_dung_Dang_nhap.Danh_sach_Y_kien = Nguoi_dung.Danh_sach_Y_kien;
                }
            }

            if (Nguoi_dung.Nhom_Nguoi_dung.Ma_so == "NHAN_VIEN_PHUC_VU")
            {
                var Danh_sach_Ban = new List<XL_BAN>();
                Du_lieu_Ung_dung.Danh_sach_Ban.ForEach(Ban => {
                    Nguoi_dung.Danh_sach_Ban.ForEach(Ban_Phan_cong => {
                        if(Ban_Phan_cong.Ma_so == Ban.Ma_so)
                        {
                            Danh_sach_Ban.Add(Ban);
                        }
                    });
                });

                Nguoi_dung.Danh_sach_Ban = Danh_sach_Ban;
            }
        });
    }

    //============= Xử lý Chức năng của Người dùng đăng nhập ==============
    //Lưu ý Quan trọng : Tất cả thông tin xử lý phải dựa vào thông tin của chính Người dùng đăng nhập 
    public XL_NGUOI_DUNG Dang_nhap(string Ten_Dang_nhap, string Mat_khau)
    {
        var Nguoi_dung = Danh_sach_Nguoi_dung.FirstOrDefault(x =>
            x.Ten_Dang_nhap == Ten_Dang_nhap
            && x.Mat_khau == Mat_khau
            && x.Nhom_Nguoi_dung.Ma_so == "QUAN_LY_NHA_HANG");

        if (Nguoi_dung != null)
        {
            Nguoi_dung.Danh_sach_Mon_an_Xem = Nguoi_dung.Danh_sach_Mon_an;
            Nguoi_dung.Danh_sach_Ban_Xem = Nguoi_dung.Danh_sach_Ban;
            HttpContext.Current.Session["Nguoi_dung_Dang_nhap"] = Nguoi_dung;
        }

        return Nguoi_dung;
    }

    public XL_NGUOI_DUNG Nguoi_dung_Dang_nhap()
    {
        var Nguoi_dung_Dang_nhap = (XL_NGUOI_DUNG)HttpContext.Current.Session["Nguoi_dung_Dang_nhap"];

        if (Nguoi_dung_Dang_nhap == null)
        {
            HttpContext.Current.Response.Write("<font color='red'>Người dùng không hợp lệ!</font>");
            HttpContext.Current.Response.End();
        }

        return Nguoi_dung_Dang_nhap;
    }

    // Thống kê
    public string Khoi_dong_Man_hinh_Thong_ke()
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        Tinh_Doanh_thu(Nguoi_dung_Dang_nhap);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Thong_ke(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    // Danh sách món ăn
    public string Khoi_dong_Man_hinh_Danh_sach_Mon_an()
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();
        // Xử lý 

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Chuoi_Tra_cuu_Mon_an()
    {
        if (HttpContext.Current.Session["Chuoi_Tra_cuu_Mon_an"] != null)
        {
            return (string)HttpContext.Current.Session["Chuoi_Tra_cuu_Mon_an"];
        }

        return "";
    }

    public string Tra_cuu_Mon_an(string Chuoi_Tra_cuu)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        HttpContext.Current.Session["Chuoi_Tra_cuu_Mon_an"] = Chuoi_Tra_cuu;
        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Chuoi_Tra_cuu, Nguoi_dung_Dang_nhap.Danh_sach_Mon_an);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Chon_Loai_Mon_an(string Ma_so_Loai_Mon_an)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        HttpContext.Current.Session["Chuoi_Tra_cuu_Mon_an"] = Ma_so_Loai_Mon_an;
        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Ma_so_Loai_Mon_an, Nguoi_dung_Dang_nhap.Danh_sach_Mon_an);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Cap_nhat_Gia_ban(string Ma_so_Mon_an, string Gia_moi)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        long Don_gia_Ban = 0;

        var Hop_le = long.TryParse(Gia_moi, out Don_gia_Ban) && Don_gia_Ban > 0;

        if(!Hop_le)
        {
            HttpContext.Current.Response.Write("<font color='red'>Đơn giá bán phải là số dương!</font>");
            HttpContext.Current.Response.End();
        }

        var Kq = XL_DU_LIEU.Cap_nhat_Gia_ban(Ma_so_Mon_an, Gia_moi);

        if(Kq == "OK")
        {
            var Mon_an = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
            Mon_an.Don_gia_Ban = Don_gia_Ban;
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Chuoi_Tra_cuu_Mon_an(), Nguoi_dung_Dang_nhap.Danh_sach_Mon_an);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    // Danh sách bàn
    public string Khoi_dong_Man_hinh_Danh_sach_Ban()
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();
        // Xử lý 

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Chuoi_Tra_cuu_Ban()
    {
        if (HttpContext.Current.Session["Chuoi_Tra_cuu_Ban"] != null)
        {
            return (string)HttpContext.Current.Session["Chuoi_Tra_cuu_Ban"];
        }

        return "";
    }

    public string Tra_cuu_Ban(string Chuoi_Tra_cuu)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        HttpContext.Current.Session["Chuoi_Tra_cuu_Ban"] = Chuoi_Tra_cuu;
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Tra_cuu_Ban(Chuoi_Tra_cuu, Nguoi_dung_Dang_nhap.Danh_sach_Ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    // Danh sách ý kiến
    public string Khoi_dong_Man_hinh_Danh_sach_Y_kien()
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();
        // Xử lý 

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Y_kien(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Chuoi_Tra_cuu_Y_kien()
    {
        if (HttpContext.Current.Session["Chuoi_Tra_cuu_Y_kien"] != null)
        {
            return (string)HttpContext.Current.Session["Chuoi_Tra_cuu_Y_kien"];
        }

        return "";
    }

    public string Tra_cuu_Y_kien(string Chuoi_Tra_cuu)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        HttpContext.Current.Session["Chuoi_Tra_cuu_Y_kien"] = Chuoi_Tra_cuu;
        Nguoi_dung_Dang_nhap.Danh_sach_Y_kien_Xem = Tra_cuu_Y_kien(Chuoi_Tra_cuu, Nguoi_dung_Dang_nhap.Danh_sach_Y_kien);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Y_kien(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    // Danh sách phiếu đặt bàn
    public string Khoi_dong_Man_hinh_Danh_sach_Phieu_Dat_ban()
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();
        // Xử lý 

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Phieu_Dat_ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Chuoi_Tra_cuu_Phieu_Dat_ban()
    {
        if (HttpContext.Current.Session["Chuoi_Tra_cuu_Phieu_Dat_ban"] != null)
        {
            return (string)HttpContext.Current.Session["Chuoi_Tra_cuu_Phieu_Dat_ban"];
        }

        return "";
    }

    public string Tra_cuu_Phieu_Dat_ban(string Chuoi_Tra_cuu)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        HttpContext.Current.Session["Chuoi_Tra_cuu_Phieu_Dat_ban"] = Chuoi_Tra_cuu;
        Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban_Xem = Tra_cuu_Phieu_Dat_ban(Chuoi_Tra_cuu, Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Phieu_Dat_ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Cap_nhat_Khach_den(string Ma_so_Phieu_Dat_ban)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý
        var Trang_Thai = "KHACH_DA_DEN";
        var Kq = XL_DU_LIEU.Cap_nhat_Trang_thai_Phieu_Dat_ban(Ma_so_Phieu_Dat_ban, Trang_Thai);

        if (Kq == "OK")
        {
            var Phieu_Dat_ban = Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban.FirstOrDefault(x => x.Ma_so == Ma_so_Phieu_Dat_ban);
            Phieu_Dat_ban.Trang_thai = Trang_Thai;
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban_Xem = Tra_cuu_Phieu_Dat_ban(Chuoi_Tra_cuu_Phieu_Dat_ban(), Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Phieu_Dat_ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Huy_Phieu_Dat_ban(string Ma_so_Phieu_Dat_ban)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý
        var Trang_Thai = "HUY";
        var Kq = XL_DU_LIEU.Cap_nhat_Trang_thai_Phieu_Dat_ban(Ma_so_Phieu_Dat_ban, Trang_Thai);

        if (Kq == "OK")
        {
            var Phieu_Dat_ban = Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban.FirstOrDefault(x => x.Ma_so == Ma_so_Phieu_Dat_ban);
            Phieu_Dat_ban.Trang_thai = Trang_Thai;
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban_Xem = Tra_cuu_Phieu_Dat_ban(Chuoi_Tra_cuu_Phieu_Dat_ban(), Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Phieu_Dat_ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public void Tinh_Doanh_thu(XL_NGUOI_DUNG Nguoi_dung)
    {
        Nguoi_dung.Doanh_thu_Ngay = 0;
        Nguoi_dung.Doanh_thu_Thang = 0;
        Nguoi_dung.Doanh_thu_Tong = 0;

        Nguoi_dung.Danh_sach_Ban.ForEach(Ban => {
            Ban.Danh_sach_Phieu_Tinh_tien.ForEach(Phieu_Tinh_tien => {
                // Tính doanh thu tháng
                if (Phieu_Tinh_tien.Ngay.Month == DateTime.Today.Month)
                {
                    Nguoi_dung.Doanh_thu_Thang += Phieu_Tinh_tien.Tong_tien;

                    // Tính doanh thu ngày
                    if (Phieu_Tinh_tien.Ngay.Day == DateTime.Today.Day)
                    {
                        Nguoi_dung.Doanh_thu_Ngay += Phieu_Tinh_tien.Tong_tien;
                    }
                }

                Nguoi_dung.Doanh_thu_Tong += Phieu_Tinh_tien.Tong_tien;
            });
        });
    }
}

//************************* View-Layers/Presentation Layers VL/PL **********************************
public partial class XL_UNG_DUNG
{
    public string Dia_chi_Media = $"{XL_DU_LIEU.Dia_chi_Dich_vu}/Media";
    public CultureInfo Dinh_dang_VN = CultureInfo.GetCultureInfo("vi-VN");

    public string Tao_Chuoi_HTML_Doanh_thu_Nha_hang(XL_NGUOI_DUNG Nguoi_dung)
    {
        var Chuoi_HTML = $"<div class='QUAN_TRONG' style='font-size:150%; text-align:center'>" +
                            $"Doanh thu ngày: {Nguoi_dung.Doanh_thu_Ngay.ToString("c0", Dinh_dang_VN)} - " +
                            $"Doanh thu tháng: {Nguoi_dung.Doanh_thu_Thang.ToString("c0", Dinh_dang_VN)} - " +
                            $"Tổng doanh thu: {Nguoi_dung.Doanh_thu_Tong.ToString("c0", Dinh_dang_VN)}" +
                         $"</div>";

        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Xem_Thong_ke(XL_NGUOI_DUNG Nguoi_dung)
    {
        var Chuoi_HTML_Thong_ke_Doanh_thu_Nhan_vien_Phuc_vu = Ung_dung.Tao_Chuoi_HTML_Thong_ke_Doanh_thu_Nhan_vien_Phuc_vu(Nguoi_dung.Danh_sach_Nhan_vien_Phuc_vu);
        var Chuoi_HTML_Thong_ke_Doanh_thu_Loai_Mon_an = Ung_dung.Tao_Chuoi_HTML_Thong_ke_Doanh_thu_Loai_Mon_an(Nguoi_dung.Danh_sach_Loai_Mon_an);

        var Chuoi_HTML = $"<div>" +
                            $"{Tao_Chuoi_HTML_Doanh_thu_Nha_hang(Nguoi_dung)}" +   
                        $"</div>" +
                        $"<div>" +
                            $"<h3>Doanh thu theo nhân viên phục vụ:</h3>" +
                            $"{Chuoi_HTML_Thong_ke_Doanh_thu_Nhan_vien_Phuc_vu}" +
                        $"</div>" +
                        $"<div>" +
                            $"<h3>Doanh thu theo loại món ăn:</h3>" +
                            $"{Chuoi_HTML_Thong_ke_Doanh_thu_Loai_Mon_an}" +
                        $"</div>";

        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        string Chuoi_Tra_cuu = Chuoi_Tra_cuu_Mon_an();

        var Chuoi_HTML = $"<div>" +
                            $"<div>" +
                                $"<div class='TIM_KIEM' style='margin-bottom: 5px'>" +
                                    $"<form method='post'>" +
                                        $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='TRA_CUU' />" +
                                        $"<input name='Th_Chuoi_Tra_cuu' value='{Chuoi_Tra_cuu}' class='form-control' placeholder='Tìm theo Loại món ăn, Tên món ăn'/>" +
                                    $"</form>" +
                                $"</div>" +
                                $"{ Tao_Chuoi_HTML_Danh_sach_Loai_Mon_an_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Loai_Mon_an_Xem, Nguoi_dung_Dang_nhap.Danh_sach_Mon_an.Count)}" +
                            $"</div>" +
                            $"{ Tao_Chuoi_HTML_Danh_sach_Mon_an_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem)}" +
                        $"</div>";
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Xem_Danh_sach_Ban(XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        string Chuoi_Tra_cuu = Chuoi_Tra_cuu_Ban();

        var Chuoi_HTML = $"<div>" +
                            $"<div class='TIM_KIEM' style='margin-bottom: 5px'>" +
                                $"<form method='post'>" +
                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='TRA_CUU' />" +
                                    $"<input name='Th_Chuoi_Tra_cuu' value='{Chuoi_Tra_cuu}' class='form-control' placeholder='Tìm theo Mã số bàn, Tên bàn'/>" +
                                $"</form>" +
                            $"</div>" +
                            $"{ Tao_Chuoi_HTML_Danh_sach_Ban_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem)}" +
                        $"</div>";
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Xem_Danh_sach_Y_kien(XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        string Chuoi_Tra_cuu = Chuoi_Tra_cuu_Y_kien();

        var Chuoi_HTML = $"<div>" +
                            $"<div class='TIM_KIEM' style='margin-bottom: 5px'>" +
                                $"<form method='post'>" +
                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='TRA_CUU' />" +
                                    $"<input name='Th_Chuoi_Tra_cuu' value='{Chuoi_Tra_cuu}' class='form-control' placeholder='Tìm theo Mã số bàn, Tên nhân viên'/>" +
                                $"</form>" +
                            $"</div>" +
                            $"{ Tao_Chuoi_HTML_Danh_sach_Y_kien_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Y_kien_Xem)}" +
                        $"</div>";
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Xem_Danh_sach_Phieu_Dat_ban(XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        string Chuoi_Tra_cuu = Chuoi_Tra_cuu_Phieu_Dat_ban();

        var Chuoi_HTML = $"<div>" +
                            $"<div class='TIM_KIEM' style='margin-bottom: 5px'>" +
                                $"<form method='post'>" +
                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='TRA_CUU' />" +
                                    $"<input name='Th_Chuoi_Tra_cuu' value='{Chuoi_Tra_cuu}' class='form-control' placeholder='Tìm theo Mã số phiếu đặt, Thông tin liên hệ của khách hàng'/>" +
                                $"</form>" +
                            $"</div>" +
                            $"{ Tao_Chuoi_HTML_Danh_sach_Phieu_Dat_ban_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Phieu_Dat_ban_Xem)}" +
                        $"</div>";
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Loai_Mon_an_Xem(List<XL_LOAI_MON_AN> Danh_sach, int Tong_so_Mon_an)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>" +
                                        $"<div style='display: inline-block; margin-right: 5px; margin-bottom: 5px'>" +
                                            $"<form method='post'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='CHON_LOAI_MON_AN' />" +
                                                $"<input name='Th_Ma_so_Loai_Mon_an' type='hidden' value='' />" +
                                                $"<button type='submit' class='btn btn-sm btn-info' >" +
                                                $"Tất cả ({Tong_so_Mon_an})" +
                                                $"</button>" +
                                            $"</form>" +
                                        $"</div>";

        Danh_sach.ForEach(Loai_Mon_an =>
        {
            var Chuoi_Chuc_nang_Chon = $"<form method='post'>" +
                                            $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='CHON_LOAI_MON_AN' />" +
                                            $"<input name='Th_Ma_so_Loai_Mon_an' type='hidden' value='{Loai_Mon_an.Ma_so}' />" +
                                            $"<button type='submit' class='btn btn-sm btn-primary' >" +
                                            $"{Loai_Mon_an.Ten} ({Loai_Mon_an.So_luong_Mon})" +
                                            $"</button>" +
                                        $"</form>";

            var Chuoi_Thong_tin = $"<div> " +
                                      $"{Chuoi_Chuc_nang_Chon}" +
                                  $"</div>";

            var Chuoi_HTML = $"<div style='display: inline-block; margin-right: 5px; margin-bottom: 5px'>" +
                                $"{Chuoi_Thong_tin}" +
                             $"</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";
        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Mon_an_Xem(List<XL_MON_AN> Danh_sach)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach.ForEach(Mon_an =>
        {
            var Chuoi_Hinh = $"<div class='KHUNG_HINH mx-auto'>" +
                                $"<img src='{Dia_chi_Media}/{Mon_an.Ma_so}.jpg' class='img-thumbnail HINH'/>" +
                             "</div>";

            var Chuoi_Chuc_nang_Cap_nhat = $"<form method='post'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='CAP_NHAT_GIA_BAN' />" +
                                                $"<input name='Th_Ma_so_Mon_an' type='hidden' value='{Mon_an.Ma_so}' />" +
                                                $"Cập nhật giá: <input type='text' name='Th_Gia_moi' style='width:70px' value='{Mon_an.Don_gia_Ban}'/>" +
                                            $"</form>";

            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>{Mon_an.Ten}</strong>" +
                                      $"<br /><strong>Doanh thu: { Mon_an.Doanh_thu.ToString("c0", Dinh_dang_VN) }</strong>" +
                                      $"<br />Đơn giá: { Mon_an.Don_gia_Ban.ToString("c0", Dinh_dang_VN) }" +
                                  $"</div>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-5 col-sm-4 col-md-3 col-lg-2'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Hinh}" +
                                     $"{Chuoi_Thong_tin}" +
                                     $"{Chuoi_Chuc_nang_Cap_nhat}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Ban_Xem(List<XL_BAN> Danh_sach_Ban)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach_Ban.ForEach(Ban =>
        {
            // Thông tin bàn
            var Nhan_vien_Phuc_vu = Nhan_vien_Phuc_vu_Ban(Ban);
            var Chuoi_Thong_tin = $"<div><strong>{Ban.Ten}</strong>" +
                                  $"<br/>Nhân viên phục vụ: {Nhan_vien_Phuc_vu.Ho_ten}";

            if (Ban.Trang_thai == "TRONG")
            {
                Chuoi_Thong_tin += $"<br/><p style='color:green;'>TRỐNG</p>";
            }
            else
            {
                Chuoi_Thong_tin += $"<br/><p style='color:orange;'>CÓ KHÁCH</p>";
            }

            Chuoi_Thong_tin += $"</div>";

            // Danh sách gọi món
            var Danh_sach_Goi_mon_cua_Ban = Danh_Sach_Goi_mon_cua_Ban(Ban, Du_lieu_Ung_dung.Danh_sach_Mon_an);
            long Tong_tien = 0;

            if (Danh_sach_Goi_mon_cua_Ban.Count != 0)
            {
                Chuoi_Thong_tin += $"<Ol>";
                Danh_sach_Goi_mon_cua_Ban.ForEach(Goi_mon =>
                {
                    Chuoi_Thong_tin += Tao_chuoi_HTML_Mon_an_cua_Ban(Goi_mon);
                    Tong_tien += Goi_mon.Thanh_tien;
                });

                Chuoi_Thong_tin += $"</Ol>";
            }

            // Thanh toán
            var Chuoi_Tong_tien = "";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-12 col-sm-12 col-md-6 col-lg-6'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Thong_tin}" +
                                     $"{Chuoi_Tong_tien}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_chuoi_HTML_Mon_an_cua_Ban(XL_GOI_MON Goi_mon)
    {
        var Chuoi_HTML = $"<li>{Goi_mon.Mon_an.Ten}: {Goi_mon.Don_gia.ToString("c0", Dinh_dang_VN)} x {Goi_mon.So_luong} ";
        var So_phut_Chua_phuc_vu = Math.Round(DateTime.Now.Subtract(Goi_mon.Thoi_diem_Goi).TotalMinutes);

        if (Goi_mon.Trang_thai == "CHO_NAU")
        {
            var Chuoi_Trang_thai = "CHỜ NẤU";

            if (Goi_mon.Mon_an.Loai_Mon_an.Ma_so == "NUOC_GIAI_KHAT")
            {
                Chuoi_Trang_thai = "CHỜ PHỤC VỤ";
            }

            Chuoi_HTML += $"<span style='color:red;'>[{Chuoi_Trang_thai}] [{So_phut_Chua_phuc_vu} phút]</span>";

        }
        else if (Goi_mon.Trang_thai == "DA_NAU")
        {
            Chuoi_HTML += $"<span style='color:red;'>[CHỜ PHỤC VỤ] [{So_phut_Chua_phuc_vu} phút]</span> ";
        }
        else if (Goi_mon.Trang_thai == "DA_PHUC_VU")
        {
            Chuoi_HTML += $"<span style='color:green;'>[ĐÃ PHỤC VỤ]</span>";
        }

        Chuoi_HTML += "</li>";

        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Y_kien_Xem(List<XL_Y_KIEN> Danh_sach)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach.Sort((x, y) => DateTime.Compare(y.Ngay, x.Ngay));

        Danh_sach.ForEach(Y_kien =>
        {
            var Chuoi_Danh_gia_Mon_an = Y_kien.Danh_gia_Mon_an == "HAI_LONG" ? "HÀI LÒNG" : "KHÔNG HÀI LÒNG";
            var Chuoi_Danh_gia_Phuc_vu = Y_kien.Danh_gia_Phuc_vu == "HAI_LONG" ? "HÀI LÒNG" : "KHÔNG HÀI LÒNG";

            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>Bàn: {Y_kien.Ma_so_Ban}</strong>" +
                                      $"<br />Ngày: {Y_kien.Ngay.ToLocalTime()}" +
                                      $"<br />Nhân viên phục vụ: {Y_kien.Nhan_vien_Phuc_vu.Ho_ten}" +
                                      $"<br />Đánh giá món ăn: {Chuoi_Danh_gia_Mon_an}" +
                                      $"<br />Đánh giá phục vụ: {Chuoi_Danh_gia_Phuc_vu}" +
                                      $"<br />Nội dung: { Y_kien.Noi_dung }" +
                                  $"</div>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-12 col-sm-6 col-md-4 col-lg-3'>" +
                                 $"<div class='THONG_TIN { (Y_kien.Danh_gia_Mon_an == "KHONG_HAI_LONG" || Y_kien.Danh_gia_Phuc_vu == "KHONG_HAI_LONG" ? "NGUY_HIEM" : "") } '>" +
                                     $"{Chuoi_Thong_tin}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Phieu_Dat_ban_Xem(List<XL_PHIEU_DAT_BAN> Danh_sach)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach.ForEach(Phieu_Dat_ban =>
        {
            if(Phieu_Dat_ban.Trang_thai != "HUY") {
                var Chuoi_Trang_thai = Phieu_Dat_ban.Trang_thai == "KHACH_DA_DEN" ? "KHÁCH ĐÃ ĐẾN" : "KHÁCH CHƯA ĐẾN";

                string Chuoi_Chuc_nang_Cap_nhat = "", Chuoi_Chuc_nang_Huy = "";

                if (Phieu_Dat_ban.Trang_thai != "KHACH_DA_DEN")
                {
                    Chuoi_Chuc_nang_Cap_nhat = $"<form method='post' style='display:inline'>" +
                                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='CAP_NHAT_KHACH_DEN' />" +
                                                    $"<input name='Th_Ma_so_Phieu_Dat_ban' type='hidden' value='{Phieu_Dat_ban.Ma_so}' />" +
                                                    $"<button type='submit' class='btn btn-sm btn-primary'>Khách đã đến</button>" +
                                                $"</form>";

                    Chuoi_Chuc_nang_Huy = $"<form method='post' style='display:inline'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='HUY_PHIEU_DAT' />" +
                                                $"<input name='Th_Ma_so_Phieu_Dat_ban' type='hidden' value='{Phieu_Dat_ban.Ma_so}' />" +
                                                $"<button type='submit' class='btn btn-sm btn-danger'>Hủy</button>" +
                                            $"</form>";
                }

                var Chuoi_Thong_tin = $"<div>" +
                                          $"<strong>Mã số: {Phieu_Dat_ban.Ma_so}</strong>" +
                                          $"<br />Ngày đặt: {Phieu_Dat_ban.Ngay_dat.ToLocalTime()}" +
                                          $"<br />Ngày đến: {Phieu_Dat_ban.Ngay_den.ToLocalTime()}" +
                                          $"<br />Số người lớn: {Phieu_Dat_ban.So_Nguoi_lon}" +
                                          $"<br />Số trẻ em: {Phieu_Dat_ban.So_Tre_em}" +
                                          $"<br />Thông tin liên hệ: {Phieu_Dat_ban.Ho_ten} - {Phieu_Dat_ban.Dien_thoai} - {Phieu_Dat_ban.Email}" +
                                          $"<br />Tình trạng: <strong>{Chuoi_Trang_thai}</strong>" +
                                      $"</div>";

                var Chuoi_HTML = $"<div class='KHUNG col-xs-12 col-sm-6 col-md-4 col-lg-3'>" +
                                     $"<div class='THONG_TIN { (Phieu_Dat_ban.Trang_thai == "KHACH_DA_DEN" ? "THANH_CONG" : "NGUY_HIEM") } '>" +
                                         $"{Chuoi_Thong_tin}" +
                                         $"<div style='text-align:center'>{Chuoi_Chuc_nang_Cap_nhat} {Chuoi_Chuc_nang_Huy}</div>" +
                                     $"</div>" +
                                 "</div>";

                Chuoi_HTML_Danh_sach += Chuoi_HTML;
            }
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Thong_ke_Doanh_thu_Nhan_vien_Phuc_vu(List<XL_NGUOI_DUNG> Danh_sach_Nhan_vien_Phuc_vu)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach_Nhan_vien_Phuc_vu.ForEach(Nhan_vien =>
        {
            Tinh_Doanh_thu(Nhan_vien);

            var Chuoi_Hinh = $"<div class='KHUNG_HINH mx-auto'>" +
                                $"<img src='{Dia_chi_Media}/{Nhan_vien.Ma_so}.png' class='img-thumbnail HINH'/>" +
                             "</div>";

            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>Họ tên: {Nhan_vien.Ho_ten}</strong>" +
                                      $"<br />Doanh thu ngày: { Nhan_vien.Doanh_thu_Ngay.ToString("c0", Dinh_dang_VN) }" +
                                      $"<br />Doanh thu tháng: { Nhan_vien.Doanh_thu_Thang.ToString("c0", Dinh_dang_VN) }" +
                                      $"<br />Tổng doanh thu: { Nhan_vien.Doanh_thu_Tong.ToString("c0", Dinh_dang_VN) }" +
                                  $"</div>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-5 col-sm-4 col-md-3 col-lg-2'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Hinh}" +
                                     $"{Chuoi_Thong_tin}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Thong_ke_Doanh_thu_Loai_Mon_an(List<XL_LOAI_MON_AN> Danh_sach_Loai_Mon_an)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach_Loai_Mon_an.ForEach(Loai_Mon_an =>
        {
            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>Tên: {Loai_Mon_an.Ten}</strong>" +
                                      $"<br />Doanh thu: { Loai_Mon_an.Doanh_thu.ToString("c0", Dinh_dang_VN) }" +
                                  $"</div>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-5 col-sm-4 col-md-3 col-lg-2'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Thong_tin}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

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
            Mon_an.Ma_so.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
            || Mon_an.Ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
            || Mon_an.Loai_Mon_an.Ma_so.ToUpper() == Chuoi_Tra_cuu.ToUpper()
            || Mon_an.Loai_Mon_an.Ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
        );

        return Danh_sach;
    }

    public List<XL_BAN> Tra_cuu_Ban(string Chuoi_Tra_cuu, List<XL_BAN> Danh_sach)
    {
        Danh_sach = Danh_sach.FindAll(Ban =>
            Ban.Ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper()) || Ban.Ma_so.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
        );

        return Danh_sach;
    }

    public XL_NGUOI_DUNG Nhan_vien_Phuc_vu_Ban(XL_BAN Ban)
    {
        var Nhan_vien_Phuc_vu_Ban = new XL_NGUOI_DUNG();

        Danh_sach_Nguoi_dung.ForEach(Nhan_vien => {
            if(Nhan_vien.Nhom_Nguoi_dung.Ma_so == "NHAN_VIEN_PHUC_VU")
            {
                Nhan_vien.Danh_sach_Ban.ForEach(Ban_Phan_cong => {
                    if (Ban_Phan_cong.Ma_so == Ban.Ma_so)
                    {
                        Nhan_vien_Phuc_vu_Ban = Nhan_vien;
                    }
                });
            }
        });

        return Nhan_vien_Phuc_vu_Ban;
    }

    public List<XL_GOI_MON> Danh_Sach_Goi_mon_cua_Ban(XL_BAN Ban, List<XL_MON_AN> Danh_sach_Mon_an)
    {
        var Danh_sach_Kq = new List<XL_GOI_MON>();

        Danh_sach_Mon_an.ForEach(Mon_an => {
            Mon_an.Danh_sach_Goi_mon.ForEach(Goi_mon => {
                // Chuyển sang múi giờ người dùng
                Goi_mon.Thoi_diem_Goi = Goi_mon.Thoi_diem_Goi.ToLocalTime();

                if(Goi_mon.Ban.Ma_so == Ban.Ma_so && Goi_mon.Trang_thai != "HUY" && Goi_mon.Trang_thai != "DA_THANH_TOAN")
                {
                    Goi_mon.Mon_an = Mon_an;
                    Danh_sach_Kq.Add(Goi_mon);
                }
            });
        });
 
        return Danh_sach_Kq;
    }

    public List<XL_Y_KIEN> Tra_cuu_Y_kien(string Chuoi_Tra_cuu, List<XL_Y_KIEN> Danh_sach)
    {
        Danh_sach = Danh_sach.FindAll(Y_kien =>
            Y_kien.Ma_so_Ban.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper()) || Y_kien.Nhan_vien_Phuc_vu.Ho_ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
        );
        return Danh_sach;
    }

    public List<XL_PHIEU_DAT_BAN> Tra_cuu_Phieu_Dat_ban(string Chuoi_Tra_cuu, List<XL_PHIEU_DAT_BAN> Danh_sach)
    {
        Danh_sach = Danh_sach.FindAll(Phieu_Dat_ban =>
            Phieu_Dat_ban.Ma_so.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper()) 
            || Phieu_Dat_ban.Ho_ten.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
            || Phieu_Dat_ban.Dien_thoai.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
            || Phieu_Dat_ban.Email.ToUpper().Contains(Chuoi_Tra_cuu.ToUpper())
        );
        return Danh_sach;
    }
}

//************************* Data-Layers DL **********************************
public partial class XL_DU_LIEU
{
    public static string Dia_chi_Dich_vu = "http://localhost:50963";
    static string Dia_chi_Dich_vu_Quan_ly_Nha_hang = $"{Dia_chi_Dich_vu}/1-Dich_vu_Giao_tiep/DV_Quan_ly_Nha_hang.cshtml";

    public static XL_DU_LIEU Doc_Du_lieu()
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = "Ma_so_Xu_ly=KHOI_DONG_DU_LIEU_QUAN_LY_NHA_HANG";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Quan_ly_Nha_hang}?{Tham_so}";
        var Chuoi_JSON = Xu_ly.DownloadString(Dia_chi_Xu_ly);
        var Du_lieu = Json.Decode<XL_DU_LIEU>(Chuoi_JSON);

        return Du_lieu;
    }

    public static string Cap_nhat_Gia_ban(string Ma_so_Mon_an, string Gia_moi)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = $"Ma_so_Xu_ly=CAP_NHAT_GIA_BAN&Ma_so_Mon_an={Ma_so_Mon_an}&Gia_moi={Gia_moi}";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Quan_ly_Nha_hang}?{Tham_so}";
        var Chuoi_Kq = Xu_ly.DownloadString(Dia_chi_Xu_ly);

        return Chuoi_Kq;
    }

    public static string Cap_nhat_Trang_thai_Phieu_Dat_ban(string Ma_so_Phieu_Dat_ban, string Trang_thai)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = $"Ma_so_Xu_ly=CAP_NHAT_TRANG_THAI_PHIEU_DAT_BAN&Ma_so_Phieu_Dat_ban={Ma_so_Phieu_Dat_ban}&Trang_thai={Trang_thai}";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Quan_ly_Nha_hang}?{Tham_so}";
        var Chuoi_Kq = Xu_ly.DownloadString(Dia_chi_Xu_ly);

        return Chuoi_Kq;
    }
}