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
            Nguoi_dung.Danh_sach_Loai_Mon_an = Danh_sach_Loai_Mon_an;
            Nguoi_dung.Danh_sach_Loai_Mon_an_Xem = Nguoi_dung.Danh_sach_Loai_Mon_an;
            Nguoi_dung.Danh_sach_Mon_an = Du_lieu_Ung_dung.Danh_sach_Mon_an;
            Nguoi_dung.Danh_sach_Ban = Du_lieu_Ung_dung.Danh_sach_Ban.FindAll(Ban => 
                Nguoi_dung.Danh_sach_Ban.Any(Ban_Phan_cong => Ban.Ma_so == Ban_Phan_cong.Ma_so)
            );
            Nguoi_dung.Danh_sach_Ban_Xem = Nguoi_dung.Danh_sach_Ban;

            Nguoi_dung.Danh_sach_Mon_an.ForEach(Mon_an => {
                Mon_an.Danh_sach_Goi_mon.ForEach(Goi_mon => {
                    // Chuyển sang múi giờ địa phương
                    Goi_mon.Thoi_diem_Goi = Goi_mon.Thoi_diem_Goi.ToLocalTime();
                    Goi_mon.Thoi_diem_Huy = Goi_mon.Thoi_diem_Huy.ToLocalTime();
                    Goi_mon.Thoi_diem_Nau_xong = Goi_mon.Thoi_diem_Nau_xong.ToLocalTime();
                    Goi_mon.Thoi_diem_Phuc_vu = Goi_mon.Thoi_diem_Phuc_vu.ToLocalTime();
                    Goi_mon.Thoi_diem_Thanh_toan = Goi_mon.Thoi_diem_Thanh_toan.ToLocalTime();

                    if (Nguoi_dung.Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Goi_mon.Ban.Ma_so) != null)
                    {
                        if (Goi_mon.Trang_thai != "HUY" && Goi_mon.Trang_thai != "DA_THANH_TOAN" && Goi_mon.Ban != null)
                        {
                            Goi_mon.Mon_an = new XL_MON_AN() { Ma_so = Mon_an.Ma_so, Ten = Mon_an.Ten, Loai_Mon_an = Mon_an.Loai_Mon_an };
                            Nguoi_dung.Danh_sach_Goi_mon.Add(Goi_mon);
                        }
                    }
                });
            });

            if (Nguoi_dung_Dang_nhap != null && Nguoi_dung_Dang_nhap.Ma_so == Nguoi_dung.Ma_so)
            {
                Nguoi_dung_Dang_nhap.Danh_sach_Loai_Mon_an = Nguoi_dung.Danh_sach_Loai_Mon_an;
                Nguoi_dung_Dang_nhap.Danh_sach_Mon_an = Nguoi_dung.Danh_sach_Mon_an;
                Nguoi_dung_Dang_nhap.Danh_sach_Ban = Nguoi_dung.Danh_sach_Ban;
                Nguoi_dung_Dang_nhap.Danh_sach_Mon_an = Nguoi_dung.Danh_sach_Mon_an;
                Nguoi_dung_Dang_nhap.Danh_sach_Goi_mon = Nguoi_dung.Danh_sach_Goi_mon;
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
            && x.Nhom_Nguoi_dung.Ma_so == "NHAN_VIEN_PHUC_VU");

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

        if(Nguoi_dung_Dang_nhap == null)
        {
            HttpContext.Current.Response.Write("Người dùng không hợp lệ");
            HttpContext.Current.Response.End();
        }

        return Nguoi_dung_Dang_nhap;
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

    public string Chon_Mon_an(string Ma_so_Mon_an)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        var Mon_an_Chon = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
        var Mon_an_Da_chon = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);

        if (Mon_an_Da_chon == null)
        {
            Mon_an_Chon.So_luong = 1;
            Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon.Add(Mon_an_Chon);
            Mon_an_Chon.Tien = Mon_an_Chon.So_luong * Mon_an_Chon.Don_gia_Ban;
        }
        else
        {
            Mon_an_Da_chon.So_luong += 1;
            Mon_an_Da_chon.Tien = Mon_an_Da_chon.So_luong * Mon_an_Da_chon.Don_gia_Ban;
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Chuoi_Tra_cuu_Mon_an(), Nguoi_dung_Dang_nhap.Danh_sach_Mon_an);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Giam_So_luong_Mon_an(string Ma_so_Mon_an)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        var Mon_an = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
        Mon_an.So_luong -= 1;
        Mon_an.Tien = Mon_an.So_luong * Mon_an.Don_gia_Ban;

        if (Mon_an.So_luong == 0)
        {
            Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon.Remove(Mon_an);
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem = Tra_cuu_Mon_an(Chuoi_Tra_cuu_Mon_an(), Nguoi_dung_Dang_nhap.Danh_sach_Mon_an);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Mon_an(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public void Dat_mon(string Ma_so_Ban_Dat)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý
        var Ban_Dat = Nguoi_dung_Dang_nhap.Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Ma_so_Ban_Dat);

        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon.ForEach(Mon_an_Chon => {
            XL_GOI_MON Dat_mon = new XL_GOI_MON();

            Dat_mon.Nhan_vien_Phuc_vu = new XL_NGUOI_DUNG() { Ma_so = Nguoi_dung_Dang_nhap.Ma_so, Ho_ten = Nguoi_dung_Dang_nhap.Ho_ten };
            Dat_mon.Ban = new XL_BAN() { Ma_so = Ban_Dat.Ma_so, Ten = Ban_Dat.Ten };
            Dat_mon.So_luong = Mon_an_Chon.So_luong;
            Dat_mon.Don_gia = Mon_an_Chon.Don_gia_Ban;
            Dat_mon.Thanh_tien = Mon_an_Chon.So_luong * Mon_an_Chon.Don_gia_Ban;
            Dat_mon.Trang_thai = "CHO_NAU";
            Dat_mon.Ma_so = "GM_" + DateTime.Now.Ticks;

            
            //var kq = XL_DU_LIEU.Ghi_Mon_an(Mon_an_Chon);
            var kq = XL_DU_LIEU.Ghi_Goi_mon_Moi(Dat_mon, Mon_an_Chon);
            // Cập nhật dữ liệu online
            if (kq == "OK")
            {
                //var Mon_an = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Mon_an_Chon.Ma_so);
                Mon_an_Chon.Danh_sach_Goi_mon.Add(Dat_mon);
                Dat_mon.Mon_an = new XL_MON_AN()
                {
                    Ma_so = Mon_an_Chon.Ma_so,
                    Ten = Mon_an_Chon.Ten,
                    Loai_Mon_an = new XL_LOAI_MON_AN { Ma_so = Mon_an_Chon.Loai_Mon_an.Ma_so, Ten = Mon_an_Chon.Loai_Mon_an.Ten }
                };
                Nguoi_dung_Dang_nhap.Danh_sach_Goi_mon.Add(Dat_mon);
            }
            else
            {
                HttpContext.Current.Response.Write("Lỗi hệ thống! Vui lòng thử lại!");
                HttpContext.Current.Response.End();
            }
        });

        Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon.Clear();

        HttpContext.Current.Response.Redirect("MH_Danh_sach_Ban.cshtml");
        HttpContext.Current.Response.End();
    }

    // Danh sách bàn
    public string Khoi_dong_Man_hinh_Danh_sach_Ban()
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý 
        Tinh_Doanh_thu(Nguoi_dung_Dang_nhap);

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

    public List<XL_GOI_MON> Danh_Sach_Goi_mon_cua_Ban(List<XL_GOI_MON> Danh_sach_Goi_mon, XL_BAN Ban)
    {
        var Danh_sach_Kq = Danh_sach_Goi_mon.FindAll(Goi_mon => 
            Goi_mon.Ban.Ma_so == Ban.Ma_so
            && Goi_mon.Trang_thai != "HUY"
            && Goi_mon.Trang_thai != "DA_THANH_TOAN"
        );

        return Danh_sach_Kq;
    }

    public string Bat_dau_Phuc_vu(string Ma_so_Ban)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý ghi
        var Ban = Nguoi_dung_Dang_nhap.Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Ma_so_Ban);
        
        //var Kq = XL_DU_LIEU.Ghi_Ban(Ban);
        var Kq = XL_DU_LIEU.Ghi_Cap_nhap_Tinh_trang_Ban(Ban, "CO_KHACH");
        if (Kq != "OK")
        {
            Ban.Trang_thai = "TRONG";
            HttpContext.Current.Response.Write("Lỗi hệ thống! Vui lòng thử lại!");
            HttpContext.Current.Response.End();
        }
        else
        {
            Ban.Trang_thai = "CO_KHACH";
        }
        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Tra_cuu_Ban(Chuoi_Tra_cuu_Ban(), Nguoi_dung_Dang_nhap.Danh_sach_Ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Phuc_vu_Mon_an(string Ma_so_Goi_mon)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý ghi
        var Mon_an = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an.FirstOrDefault(x => x.Danh_sach_Goi_mon.Any(y => y.Ma_so == Ma_so_Goi_mon));
        var Goi_mon_Phuc_vu = Mon_an.Danh_sach_Goi_mon.FirstOrDefault(x => x.Ma_so == Ma_so_Goi_mon);
        
        //var Kq = XL_DU_LIEU.Ghi_Mon_an(Mon_an);
        var Kq = XL_DU_LIEU.Ghi_Cap_nhap_Trang_thai_Mon_an(Goi_mon_Phuc_vu.Ban.Ma_so, Mon_an.Ma_so, Goi_mon_Phuc_vu.Ma_so, "DA_PHUC_VU");
        if (Kq != "OK")
        {
            HttpContext.Current.Response.Write("Lỗi hệ thống! Vui lòng thử lại!");
            HttpContext.Current.Response.End();
        }
        else
        {
            Goi_mon_Phuc_vu.Trang_thai = "DA_PHUC_VU";
            Goi_mon_Phuc_vu.Thoi_diem_Phuc_vu = DateTime.Now;
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Tra_cuu_Ban(Chuoi_Tra_cuu_Ban(), Nguoi_dung_Dang_nhap.Danh_sach_Ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Huy_Goi_mon(string Ma_so_Goi_mon)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý ghi
        var Mon_an = Nguoi_dung_Dang_nhap.Danh_sach_Mon_an.FirstOrDefault(x => x.Danh_sach_Goi_mon.Any(y => y.Ma_so == Ma_so_Goi_mon));
        var Goi_mon_Huy = Mon_an.Danh_sach_Goi_mon.FirstOrDefault(x => x.Ma_so == Ma_so_Goi_mon);
        
        //var Kq = XL_DU_LIEU.Ghi_Mon_an(Mon_an);
        var Kq = XL_DU_LIEU.Ghi_Cap_nhap_Trang_thai_Mon_an(Goi_mon_Huy.Ban.Ma_so, Mon_an.Ma_so, Ma_so_Goi_mon, "HUY");
        if(Kq != "OK")
        {
            HttpContext.Current.Response.Write("Lỗi hệ thống! Vui lòng thử lại!");
            HttpContext.Current.Response.End();
        }
        else
        {
            Goi_mon_Huy.Trang_thai = "HUY";
            Goi_mon_Huy.Thoi_diem_Huy = DateTime.Now;
        }
        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Tra_cuu_Ban(Chuoi_Tra_cuu_Ban(), Nguoi_dung_Dang_nhap.Danh_sach_Ban);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public string Thanh_toan(string Ma_so_Ban, XL_Y_KIEN Y_kien = null)
    {
        var Nguoi_dung_Dang_nhap = this.Nguoi_dung_Dang_nhap();

        // Xử lý ghi
        var Phieu_Tinh_tien = new XL_PHIEU_TINH_TIEN();
        Phieu_Tinh_tien.Ma_so = "PTT_" + DateTime.Now.Ticks;
        Phieu_Tinh_tien.Ngay = DateTime.Now;
        Phieu_Tinh_tien.Nhan_vien_Phuc_vu = new XL_NGUOI_DUNG() { Ma_so = Nguoi_dung_Dang_nhap.Ma_so, Ho_ten = Nguoi_dung_Dang_nhap.Ho_ten };

        Nguoi_dung_Dang_nhap.Danh_sach_Goi_mon.ForEach(Goi_mon => {
            if(Goi_mon.Ban.Ma_so == Ma_so_Ban && Goi_mon.Trang_thai == "DA_PHUC_VU")
            {
                Phieu_Tinh_tien.Danh_sach_Goi_mon.Add(Goi_mon.Ma_so);
                Phieu_Tinh_tien.Tong_tien += Goi_mon.Thanh_tien;
            }
        });

        var Kq = XL_DU_LIEU.Ghi_Phieu_Tinh_tien(Phieu_Tinh_tien, Ma_so_Ban, Y_kien);

        // Cập nhật dữ liệu online nếu ghi thành công
        if (Kq == "OK")
        {
            Nguoi_dung_Dang_nhap.Danh_sach_Goi_mon.ForEach(Goi_mon => {
                if (Goi_mon.Ban.Ma_so == Ma_so_Ban && Goi_mon.Trang_thai == "DA_PHUC_VU")
                {
                    Goi_mon.Trang_thai = "DA_THANH_TOAN";
                }
            });

            var Ban_Thanh_toan = Nguoi_dung_Dang_nhap.Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Ma_so_Ban);
            Ban_Thanh_toan.Trang_thai = "TRONG";
            Ban_Thanh_toan.Danh_sach_Phieu_Tinh_tien.Add(Phieu_Tinh_tien);

            if (Y_kien != null)
            {
                Y_kien.Nhan_vien_Phuc_vu = Nguoi_dung_Dang_nhap;
                Ban_Thanh_toan.Danh_sach_Y_kien.Add(Y_kien);
            }
        }
        else
        {
            HttpContext.Current.Response.Write("Lỗi hệ thống! Vui lòng thử lại!");
            HttpContext.Current.Response.End();
        }

        // Khử lỗi caching
        Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem = Tra_cuu_Ban(Chuoi_Tra_cuu_Ban(), Nguoi_dung_Dang_nhap.Danh_sach_Ban);
        Tinh_Doanh_thu(Nguoi_dung_Dang_nhap);

        // Tạo chuỗi HTML kết quả xem 
        var Chuoi_HTML = Tao_Chuoi_HTML_Xem_Danh_sach_Ban(Nguoi_dung_Dang_nhap);
        return Chuoi_HTML;
    }

    public void Tinh_Doanh_thu(XL_NGUOI_DUNG Nguoi_dung)
    {
        Nguoi_dung.Doanh_thu_Ngay = 0;
        Nguoi_dung.Doanh_thu_Thang = 0;

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
            });
        });
    }
}

//************************* View-Layers/Presentation Layers VL/PL **********************************
public partial class XL_UNG_DUNG
{
    public string Dia_chi_Media = $"{XL_DU_LIEU.Dia_chi_Dich_vu}/Media";
    public CultureInfo Dinh_dang_VN = CultureInfo.GetCultureInfo("vi-VN");

    public string Tao_Chuoi_HTML_Doanh_thu(XL_NGUOI_DUNG Nguoi_dung)
    {
        var Chuoi_HTML = $"<div class='QUAN_TRONG' style='font-size:150%; text-align:center'>" +
                            $"Doanh thu ngày: {Nguoi_dung.Doanh_thu_Ngay.ToString("c0", Dinh_dang_VN)} - " +
                            $"Doanh thu tháng: {Nguoi_dung.Doanh_thu_Thang.ToString("c0", Dinh_dang_VN)}" +
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
                            $"{ Tao_Chuoi_HTML_Danh_sach_Mon_an_Chon(Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Chon)}" +
                            $"{ Tao_Chuoi_HTML_Danh_sach_Mon_an_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Mon_an_Xem)}" +
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

            var Chuoi_Thong_tin = $"<div>" +
                                      $"<strong>{Mon_an.Ten}</strong>" +
                                      $"<br />Đơn giá: { Mon_an.Don_gia_Ban.ToString("c0", Dinh_dang_VN) }" +
                                  $"</div>";

            var Chuoi_Chuc_nang_Chon = $"<form method='post'>" +
                                           $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='CHON_MON_AN'/>" +
                                           $"<input name='Th_Ma_so_Mon_an' type='hidden' value='{Mon_an.Ma_so}' />" +
                                           $"<button type='submit' class='btn btn-sm btn-danger' >Chọn</button>" +
                                       $"</form>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-5 col-sm-4 col-md-3 col-lg-2'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Hinh}" +
                                     $"{Chuoi_Thong_tin}" +
                                     $"{Chuoi_Chuc_nang_Chon}" +
                                 $"</div>" +
                             "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Mon_an_Chon(List<XL_MON_AN> Danh_sach)
    {
        long Tong_tien = 0;
        var Chuoi_HTML_Danh_sach = "<div class='row MON_AN_CHON'>";

        Danh_sach.ForEach(Mon_an =>
        {
            var Chuoi_Hinh = $"<img src='{Dia_chi_Media}/{Mon_an.Ma_so}.jpg' style='width:60px; height:60px;'/>";

            var Chuoi_Thong_tin = $"<div style='text-align:left'> " +
                                    $"{Mon_an.Ten}" +
                                    $"<br />{ Mon_an.Don_gia_Ban.ToString("c0", Dinh_dang_VN) } x {Mon_an.So_luong}" +
                                    $" = { Mon_an.Tien.ToString("c0", Dinh_dang_VN)}" +
                                $"</div>";

            var Chuoi_Chuc_nang_Giam_So_luong = $"<form method='post' style='float: right'>" +
                                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='GIAM_SO_LUONG_MON_AN' />" +
                                                    $"<input name='Th_Ma_so_Mon_an' type='hidden' value='{Mon_an.Ma_so}' />" +
                                                    $"<button type='submit' class='btn btn-sm btn-danger' >-</button>" +
                                               "</form>";

            var Chuoi_HTML = $"<div class='THONG_TIN'>" +
                                $"{Chuoi_Hinh}" + $"{Chuoi_Chuc_nang_Giam_So_luong}" +
                                $"{Chuoi_Thong_tin}" +
                            "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
            Tong_tien += Mon_an.Tien;
        });

        if (Danh_sach.Count > 0)
        {
            var Chuoi_Chuc_nang_Dat_mon = $"<div style='margin-left:10px; margin-top: 10px'>" +
                                                $"<form method='post'>" +
                                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='DAT_MON' />" +
                                                    $"<span class='QUAN_TRONG'>Tổng tiền: {Tong_tien.ToString("c0", Dinh_dang_VN)}</span> " +
                                                    $"<button type='submit' id='XL_DAT_MON' class='btn btn-sm btn-danger'>Đặt món</button>" +
                                                $"</form>" + 
                                           $"</div>";

            Chuoi_HTML_Danh_sach += Chuoi_Chuc_nang_Dat_mon;
        }

        Chuoi_HTML_Danh_sach += "</div>";
        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Mon_an_Dat(List<XL_MON_AN> Danh_sach)
    {
        long Tong_tien = 0;
        var Chuoi_HTML_Danh_sach = "<div class='row MON_AN_CHON'>";

        Danh_sach.ForEach(Mon_an =>
        {
            var Chuoi_Hinh = $"<img src='{Dia_chi_Media}/{Mon_an.Ma_so}.jpg' style='width:60px; height:60px;'/>";

            var Chuoi_Thong_tin = $"<div style='text-align:left'> " +
                                    $"{Mon_an.Ten}" +
                                    $"<br />{ Mon_an.Don_gia_Ban.ToString("c0", Dinh_dang_VN) } x {Mon_an.So_luong}" +
                                    $" = { Mon_an.Tien.ToString("c0", Dinh_dang_VN)}" +
                                $"</div>";

            var Chuoi_HTML = $"<div class='THONG_TIN'>" +
                                $"{Chuoi_Hinh}" +
                                $"{Chuoi_Thong_tin}" +
                            "</div>";

            Chuoi_HTML_Danh_sach += Chuoi_HTML;
            Tong_tien += Mon_an.Tien;
        });

        Chuoi_HTML_Danh_sach += "</div>";

        if (Danh_sach.Count > 0)
        {
            var Chuoi_Chuc_nang_Dat_mon = $"<div style='clear: both'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='DAT_MON' />" +
                                                $"<span class='QUAN_TRONG'>Tổng tiền: {Tong_tien.ToString("c0", Dinh_dang_VN)}</span> " +
                                                $"<button type='submit' id='XL_DAT_MON' class='btn btn-sm btn-danger'>Đặt món</button>" +
                                           $"</div>";

            Chuoi_HTML_Danh_sach += Chuoi_Chuc_nang_Dat_mon;
        }

        return Chuoi_HTML_Danh_sach;
    }

    public string Tao_Chuoi_HTML_Xem_Danh_sach_Ban(XL_NGUOI_DUNG Nguoi_dung_Dang_nhap)
    {
        string Chuoi_Tra_cuu = Chuoi_Tra_cuu_Ban();

        var Chuoi_HTML = $"<div>" +
                            $"{Tao_Chuoi_HTML_Doanh_thu(Nguoi_dung_Dang_nhap)}" +
                            $"<div class='TIM_KIEM' style='margin-bottom: 5px'>" +
                                $"<form method='post'>" +
                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='TRA_CUU' />" +
                                    $"<input name='Th_Chuoi_Tra_cuu' value='{Chuoi_Tra_cuu}' class='form-control' placeholder='Tìm theo Mã số bàn, Tên bàn'/>" +
                                $"</form>" +
                            $"</div>" +
                            $"{ Tao_Chuoi_HTML_Danh_sach_Ban_Xem(Nguoi_dung_Dang_nhap.Danh_sach_Ban_Xem, Nguoi_dung_Dang_nhap.Danh_sach_Goi_mon)}" +
                        $"</div>";
        return Chuoi_HTML;
    }

    public string Tao_Chuoi_HTML_Danh_sach_Ban_Xem(List<XL_BAN> Danh_sach_Ban, List<XL_GOI_MON> Danh_sach_Goi_mon)
    {
        var Chuoi_HTML_Danh_sach = "<div class='row'>";

        Danh_sach_Ban.ForEach(Ban =>
        {
            // Thông tin bàn
            var Chuoi_Thong_tin = $"<div><strong>{Ban.Ten}</strong>";

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
            var Danh_sach_Goi_mon_cua_Ban = Danh_Sach_Goi_mon_cua_Ban(Danh_sach_Goi_mon, Ban);
            long Tong_tien = 0;
            bool Co_the_Thanh_toan = true;

            if (Danh_sach_Goi_mon_cua_Ban.Count != 0)
            {
                Chuoi_Thong_tin += $"<Ol>";
                Danh_sach_Goi_mon_cua_Ban.ForEach(Goi_mon =>
                {
                    Chuoi_Thong_tin += Tao_chuoi_HTML_Mon_an_cua_Ban(Goi_mon);
                    Tong_tien += Goi_mon.Thanh_tien;

                    if(!(Goi_mon.Trang_thai == "DA_PHUC_VU" || Goi_mon.Trang_thai == "HUY"))
                    {
                        Co_the_Thanh_toan = false;
                    }
                });

                Chuoi_Thong_tin += $"</Ol>";
            }

            // Thanh toán
            var Chuoi_Tong_tien = "";
            var Chuoi_Chuc_nang = $"<div style='text-align:center'>";

            if(Ban.Trang_thai == "CO_KHACH")
            {
                Chuoi_Tong_tien = $"<div style='margin-top:5px; text-align:right; color:red; font-weight:bold'>Tổng tiền phải thu: {Tong_tien.ToString("c0", Dinh_dang_VN)}</div>";

                var Chuoi_XL_Click = "";

                if(!Co_the_Thanh_toan)
                {
                    Chuoi_XL_Click = "onclick='alert(\"Tất cả món ăn phải được phục vụ hoặc hủy mới có thể thanh toán!\"); event.stopPropagation(); return false;'";
                }

                Chuoi_Chuc_nang += $"<form method='post' style='display:inline'>" +
                                        $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='DAT_MON' />" +
                                        $"<input name='Th_Ma_so_Ban' type='hidden' value='{Ban.Ma_so}' />" +
                                        $"<button type='submit' class='btn btn-sm btn-primary'>Đặt món</button>" +
                                    $"</form> " +
                                    $"<form method='post' style='display:inline'>" +
                                        $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='THANH_TOAN' />" +
                                        $"<input name='Th_Ma_so_Ban' type='hidden' value='{Ban.Ma_so}' />" +
                                        $"<button type='submit' class='btn btn-sm btn-danger' {Chuoi_XL_Click}>Thanh toán</button>" +
                                    $"</form>" +
                                    $"<div style='margin-top:5px'>" +
                                        $"<button type='button' class='btn btn-sm btn-danger' {Chuoi_XL_Click} data-toggle='modal' data-target='#modal-{Ban.Ma_so}'>Góp ý & Thanh toán</button>" +
                                    $"</div>" +
                                    $"<div class='modal fade' id='modal-{Ban.Ma_so}' tabindex='-1' role='dialog' aria-labelledby='exampleModalLabel' aria-hidden='true'>" +
                                      $"<div class='modal-dialog' role='document'>" +
                                        $"<div class='modal-content'>" +
                                          $"<div class='modal-header'>" +
                                            $"<h5 class='modal-title' id='exampleModalLabel'>Góp ý & Thanh toán cho {Ban.Ten}</h5>" +
                                            $"<button type='button' class='close' data-dismiss='modal' aria-label='Close'>" +
                                              $"<span aria-hidden='true'>&times;</span>" +
                                            $"</button>" +
                                          $"</div>" +
                                          $"<form method='post'>" +
                                              $"<div class='modal-body'>" +
                                                    $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='GOP_Y_VA_THANH_TOAN' />" +
                                                    $"<input name='Th_Ma_so_Ban' type='hidden' value='{Ban.Ma_so}' />" +
                                                    $"<table style='width:100%;text-align:left'><tr>" +
                                                        $"<td style='width:30%;'>Đánh giá phục vụ:</td>" +
                                                        $"<td><select name='Th_Danh_gia_Phuc_vu' class='form-control'>" +
                                                            $"<option value='HAI_LONG'>Hài lòng</option>" +
                                                            $"<option value='KHONG_HAI_LONG'>Không hài lòng</option>" +
                                                        $"</select></td>" +
                                                        $"</tr><tr>" +
                                                        $"<td>Đánh giá món ăn</td>" +
                                                        $"<td><select name='Th_Danh_gia_Mon_an' class='form-control'>" +
                                                            $"<option value='HAI_LONG'>Hài lòng</option>" +
                                                            $"<option value='KHONG_HAI_LONG'>Không hài lòng</option>" +
                                                        $"</select></td>" +
                                                        $"</tr><tr>" +
                                                        $"<td>Ý kiến:</td>" +
                                                        $"<td><textarea name='Th_Noi_dung_Gop_y' class='form-control' required></textarea></td>" +
                                                    $"</tr></table>" +
                                                    $"{Chuoi_Tong_tien}" +
                                              $"</div>" +
                                              $"<div class='modal-footer'>" +
                                                $"<button type='button' class='btn btn-sm btn-secondary' data-dismiss='modal'>Close</button>" +
                                                $"<button type='submit' class='btn btn-sm btn-danger'>Đồng ý</button>" +
                                              $"</div>" +
                                          $"</form>" +
                                        $"</div>" +
                                      $"</div>" +
                                    $"</div>";
            }
            else
            {
                Chuoi_Chuc_nang += $"<form method='post' style='display:inline'>" +
                                        $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='BAT_DAU_PHUC_VU' />" +
                                        $"<input name='Th_Ma_so_Ban' type='hidden' value='{Ban.Ma_so}' />" +
                                        $"<button type='submit' class='btn btn-sm btn-danger'>Bắt đầu phục vụ</button>" +
                                    $"</form>";
            }

            Chuoi_Chuc_nang += "</div>";

            var Chuoi_HTML = $"<div class='KHUNG col-xs-12 col-sm-12 col-md-6 col-lg-6'>" +
                                 $"<div class='THONG_TIN'>" +
                                     $"{Chuoi_Thong_tin}" +
                                     $"{Chuoi_Tong_tien}" +
                                     $"{Chuoi_Chuc_nang}" +
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
            var Chuoi_Chuc_nang_Phuc_vu = "";
            var Chuoi_Chuc_nang_Huy_mon = $"<form method='post' style='display:inline'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='HUY_GOI_MON' />" +
                                                $"<input name='Th_Ma_so_Goi_mon' type='hidden' value='{Goi_mon.Ma_so}' />" +
                                                $"<button type='submit' class='btn btn-sm btn-danger'>Hủy</button>" +
                                            $"</form>";

            if (Goi_mon.Mon_an.Loai_Mon_an.Ma_so == "NUOC_GIAI_KHAT")
            {
                Chuoi_Trang_thai = "CHỜ PHỤC VỤ";
                Chuoi_Chuc_nang_Phuc_vu = $"<form method='post' style='display:inline'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='PHUC_VU_MON_AN' />" +
                                                $"<input name='Th_Ma_so_Goi_mon' type='hidden' value='{Goi_mon.Ma_so}' />" +
                                                $"<button type='submit' class='btn btn-sm btn-primary'>Phục vụ</button>" +
                                            $"</form>";
            }

            Chuoi_HTML += $"<span style='color:red;'>[{Chuoi_Trang_thai}] [{So_phut_Chua_phuc_vu} phút]</span> " +
                          $"{Chuoi_Chuc_nang_Phuc_vu} {Chuoi_Chuc_nang_Huy_mon}";

        }
        else if (Goi_mon.Trang_thai == "DA_NAU")
        {
            var Chuoi_Chuc_nang_Phuc_vu = $"<form method='post' style='display:inline'>" +
                                                $"<input name='Th_Ma_so_Chuc_nang' type='hidden' value='PHUC_VU_MON_AN' />" +
                                                $"<input name='Th_Ma_so_Goi_mon' type='hidden' value='{Goi_mon.Ma_so}' />" +
                                                $"<button type='submit' class='btn btn-sm btn-primary'>Phục vụ</button>" +
                                            $"</form>";

            Chuoi_HTML += $"<span style='color:red;'>[CHỜ PHỤC VỤ] [{So_phut_Chua_phuc_vu} phút]</span> " +
                          $"{Chuoi_Chuc_nang_Phuc_vu}";
        }
        else if (Goi_mon.Trang_thai == "DA_PHUC_VU")
        {
            Chuoi_HTML += $"<span style='color:green;'>[ĐÃ PHỤC VỤ]</span>";
        }

        Chuoi_HTML += "</li>";

        return Chuoi_HTML;
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
}

//************************* Data-Layers DL **********************************
public partial class XL_DU_LIEU
{
    public static string Dia_chi_Dich_vu = "http://localhost:50963";
    static string Dia_chi_Dich_vu_Nhan_vien_Phuc_vu = $"{Dia_chi_Dich_vu}/1-Dich_vu_Giao_tiep/DV_Nhan_vien_Phuc_vu.cshtml";

    public static XL_DU_LIEU Doc_Du_lieu()
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = "Ma_so_Xu_ly=KHOI_DONG_DU_LIEU_NHAN_VIEN_PHUC_VU";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Chuoi_JSON = Xu_ly.DownloadString(Dia_chi_Xu_ly);
        var Du_lieu = JsonConvert.DeserializeObject<XL_DU_LIEU>(Chuoi_JSON);

        return Du_lieu;
    }

    public static string Ghi_Mon_an(XL_MON_AN Mon_an)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = "Ma_so_Xu_ly=GHI_MON_AN";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Ket_qua = Xu_ly.UploadString(Dia_chi_Xu_ly, JsonConvert.SerializeObject(Mon_an));

        return Ket_qua;
    }

    public static string Ghi_Ban(XL_BAN Ban)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = "Ma_so_Xu_ly=GHI_BAN";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Ket_qua = Xu_ly.UploadString(Dia_chi_Xu_ly, JsonConvert.SerializeObject(Ban));

        return Ket_qua;
    }

    public static string Ghi_Phieu_Tinh_tien(XL_PHIEU_TINH_TIEN Phieu_Tinh_tien, string Ma_so_Ban, XL_Y_KIEN Y_kien)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = $"Ma_so_Xu_ly=GHI_PHIEU_TINH_TIEN&Ma_so_Ban={Ma_so_Ban}";

        if(Y_kien != null)
        {
            Tham_so += $"&Danh_gia_Phuc_vu={Y_kien.Danh_gia_Phuc_vu}&Danh_gia_Mon_an={Y_kien.Danh_gia_Mon_an}&Noi_dung_Gop_y={Y_kien.Noi_dung}";
        }

        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Ket_qua = Xu_ly.UploadString(Dia_chi_Xu_ly, JsonConvert.SerializeObject(Phieu_Tinh_tien));

        return Ket_qua;
    }

    public static string Ghi_Cap_nhap_Tinh_trang_Ban(XL_BAN Ban, string Trang_thai)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = $"Ma_so_Xu_ly=GHI_CAP_NHAP_TRANG_THAI_BAN&Ma_so_Ban={Ban.Ma_so}&Trang_thai={Trang_thai}";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Kq = Xu_ly.DownloadString(Dia_chi_Xu_ly);

        return Kq;
    }

    public static string Ghi_Cap_nhap_Trang_thai_Mon_an(string Ma_so_Ban, string Ma_so_Mon_an, string Ma_so_Goi_mon, string Trang_thai)
    {
        var Kq = "";
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;
        var Tham_so = $"Ma_so_Xu_ly=GHI_CAP_NHAP_TRANG_THAI_MON_AN&Ma_so_Mon_an={Ma_so_Mon_an}&Ma_so_Goi_mon={Ma_so_Goi_mon}&Trang_thai={Trang_thai}";
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
            //var Ban = Nguoi_dung_Dang_nhap.Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Ma_so_Ban);
            //var Mon_an = Ban.Danh_sach_Mon_an_Cho_nau.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
            //var Goi_mon_Chon = Mon_an.Danh_sach_Goi_mon.FirstOrDefault(x => x.Ma_so == Ma_so_Goi_mon);
            //Goi_mon_Chon.Trang_thai = "DA_NAU";
        }
        return Kq;
    }

    public static string Ghi_Goi_mon_Moi(XL_GOI_MON Goi_mon, XL_MON_AN Mon_an)
    {
        var Xu_ly = new WebClient();
        Xu_ly.Encoding = System.Text.Encoding.UTF8;

        var Tham_so = $"Ma_so_Xu_ly=GHI_GOI_MON_MOI&Ma_so_Mon_an={Mon_an.Ma_so}";
        var Dia_chi_Xu_ly = $"{Dia_chi_Dich_vu_Nhan_vien_Phuc_vu}?{Tham_so}";
        var Ket_qua = Xu_ly.UploadString(Dia_chi_Xu_ly, JsonConvert.SerializeObject(Goi_mon));

        return Ket_qua;
    }
}