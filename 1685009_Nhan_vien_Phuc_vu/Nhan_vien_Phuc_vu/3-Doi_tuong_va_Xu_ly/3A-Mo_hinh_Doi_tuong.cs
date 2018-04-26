using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class XL_DU_LIEU
{
    public XL_NHA_HANG Nha_hang = new XL_NHA_HANG();
    public List<XL_NGUOI_DUNG> Danh_sach_Nguoi_dung = new List<XL_NGUOI_DUNG>();
    public List<XL_MON_AN> Danh_sach_Mon_an = new List<XL_MON_AN>();
    public List<XL_BAN> Danh_sach_Ban = new List<XL_BAN>();
}

//==============================To chuc ==========================

public class XL_NHA_HANG
{
    public string Ten = "";
    public string Ma_so = "";
    public string Dien_thoai = "";
    public string Dia_chi = "";
    public string Mail = "";
    public List<XL_LOAI_MON_AN> Danh_sach_Loai_Mon_an = new List<XL_LOAI_MON_AN>();
    public List<XL_NHAN_VIEN_PHUC_VU> Danh_sach_Nhan_vien_Phuc_vu = new List<XL_NHAN_VIEN_PHUC_VU>();
    public List<XL_DAU_BEP> Danh_sach_Dau_bep = new List<XL_DAU_BEP>();
    public List<XL_QUAN_LY> Danh_sach_Quan_ly = new List<XL_QUAN_LY>();
}

public class XL_LOAI_MON_AN
{
    public string Ten = "";
    public string Ma_so = "";

    // Tính toán
    public int So_luong_Mon;
    public long Doanh_thu;
}

public class XL_NHOM_NGUOI_DUNG
{
    public string Ten = "";
    public string Ma_so = "";
}

//====================================Con nguoi====================

public class XL_NGUOI_DUNG
{
    public string Ho_ten = "";
    public string Ma_so = "";
    public string Ten_Dang_nhap = "";
    public string Mat_khau = "";
    public XL_NHOM_NGUOI_DUNG Nhom_Nguoi_dung = new XL_NHOM_NGUOI_DUNG();
    public List<XL_BAN> Danh_sach_Ban = new List<XL_BAN>();

    // Tính toán
    public long Doanh_thu_Ngay, Doanh_thu_Thang;

    // Online 
    public List<XL_LOAI_MON_AN> Danh_sach_Loai_Mon_an = new List<XL_LOAI_MON_AN>();
    public List<XL_LOAI_MON_AN> Danh_sach_Loai_Mon_an_Xem = new List<XL_LOAI_MON_AN>();
    public List<XL_MON_AN> Danh_sach_Mon_an = new List<XL_MON_AN>();
    public List<XL_MON_AN> Danh_sach_Mon_an_Xem = new List<XL_MON_AN>();
    public List<XL_MON_AN> Danh_sach_Mon_an_Chon = new List<XL_MON_AN>();
    public List<XL_BAN> Danh_sach_Ban_Xem = new List<XL_BAN>();
    public List<XL_GOI_MON> Danh_sach_Goi_mon = new List<XL_GOI_MON>();
}

public class XL_NHAN_VIEN_PHUC_VU
{
    public string Ho_ten = "";
    public string Ma_so = "";
    public string Ten_Dang_nhap = "";
    public string Mat_khau = "";
    public List<XL_BAN> Danh_sach_Ban = new List<XL_BAN>();
}

public class XL_DAU_BEP
{
    public string Ho_ten = "";
    public string Ma_so = "";
    public string Ten_Dang_nhap = "";
    public string Mat_khau = "";
}

public class XL_QUAN_LY
{
    public string Ho_ten = "";
    public string Ma_so = "";
    public string Ten_Dang_nhap = "";
    public string Mat_khau = "";
}

//=========================== Doi tuong xu ly chinh =====================

public class XL_MON_AN
{
    public string Ma_so = "";
    public string Ten = "";
    public long Don_gia_Ban = 0;
    public XL_LOAI_MON_AN Loai_Mon_an = new XL_LOAI_MON_AN();
    public List<XL_GOI_MON> Danh_sach_Goi_mon = new List<XL_GOI_MON>();

    // Tính toán
    public long Doanh_thu;
    public int So_luong;
    public long Tien;
}

public class XL_GOI_MON
{
    public string Ma_so = "";
    public int So_luong = 0;
    public long Don_gia = 0;
    public long Thanh_tien = 0;
    public DateTime Thoi_diem_Goi = DateTime.Now;
    public DateTime Thoi_diem_Huy = DateTime.Now;
    public DateTime Thoi_diem_Nau_xong = DateTime.Now;
    public DateTime Thoi_diem_Phuc_vu = DateTime.Now;
    public DateTime Thoi_diem_Thanh_toan = DateTime.Now;
    public string Trang_thai = "CHO_NAU";
    public XL_NGUOI_DUNG Nhan_vien_Phuc_vu = new XL_NGUOI_DUNG();
    public XL_BAN Ban = new XL_BAN();

    // Tính toán
    public XL_MON_AN Mon_an = new XL_MON_AN();
}

public class XL_BAN
{
    public string Ten = "";
    public string Ma_so = "";
    public string Trang_thai = "TRONG";
    public List<XL_PHIEU_TINH_TIEN> Danh_sach_Phieu_Tinh_tien = new List<XL_PHIEU_TINH_TIEN>();
    public List<XL_Y_KIEN> Danh_sach_Y_kien = new List<XL_Y_KIEN>();
}

public class XL_PHIEU_TINH_TIEN
{
    public string Ma_so = "";
    public DateTime Ngay = DateTime.Now;
    public long Tong_tien = 0;
    public XL_NGUOI_DUNG Nhan_vien_Phuc_vu = new XL_NGUOI_DUNG();
    public List<String> Danh_sach_Goi_mon = new List<String>();
}

public class XL_Y_KIEN
{
    public DateTime Ngay = DateTime.Now;
    public string Danh_gia_Phuc_vu = "HAI_LONG";
    public string Danh_gia_Mon_an = "HAI_LONG";
    public XL_NGUOI_DUNG Nhan_vien_Phuc_vu = new XL_NGUOI_DUNG();
    public string Noi_dung = "";
} 

public class XL_PHIEU_DAT_BAN
{
    public string Ma_so = "";
    public DateTime Ngay_dat = DateTime.Now;
    public DateTime Ngay_den = DateTime.Now;
    public int So_Nguoi_lon = 0;
    public int So_Tre_em = 0;
    public string Ghi_chu = "";
    public string Ho_ten = "", Dien_thoai = "", Email = "";
    public bool Co_den = false;
}