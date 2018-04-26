using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class XL_DU_LIEU
{
    public XL_NHA_HANG Nha_hang = new XL_NHA_HANG();
    public List<XL_MON_AN> Danh_sach_Mon_an = new List<XL_MON_AN>();
    public List<XL_BAN> Danh_sach_Ban = new List<XL_BAN>();
    public List<XL_NGUOI_DUNG> Danh_sach_Nguoi_dung = new List<XL_NGUOI_DUNG>();
}

//==============================To chuc ==========================

public class XL_NHA_HANG
{
    public string Ten = "";
    public string Ma_so = "";


    public List<XL_DAU_BEP> Danh_sach_Dau_bep = new List<XL_DAU_BEP>();

}


public class XL_LOAI_MON_AN
{
    public string Ten = "";
    public string Ma_so = "";
}

public class XL_NHOM_NGUOI_DUNG
{
    public string Ten = "";
    public string Ma_so = "";
}
//====================================Con nguoi====================

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
public class XL_NGUOI_DUNG
{
    public string Ho_ten = "";
    public string Ma_so = "";
    public string Ten_Dang_nhap = "";
    public string Mat_khau = "";
    public XL_NHOM_NGUOI_DUNG Nhom_Nguoi_dung = new XL_NHOM_NGUOI_DUNG();
    public List<XL_BAN> Danh_sach_Ban = new List<XL_BAN>();
    public List<XL_BAN> Danh_sach_Ban_Xem = new List<XL_BAN>();
    public List<XL_MON_AN> Danh_sach_Mon_an = new List<XL_MON_AN>();
    public List<XL_LOAI_MON_AN> Danh_sach_Loai_Mon_an = new List<XL_LOAI_MON_AN>();
}

//=========================== Doi tuong xu ly chinh =====================

public class XL_MON_AN
{
    public string Ma_so = "";
    public string Ten = "";
    public XL_LOAI_MON_AN Loai_Mon_an = new XL_LOAI_MON_AN();
    public List<XL_GOI_MON> Danh_sach_Goi_mon = new List<XL_GOI_MON>();
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
    public XL_NHAN_VIEN_PHUC_VU Nhan_vien_Phuc_vu = new XL_NHAN_VIEN_PHUC_VU();
    public XL_BAN Ban = new XL_BAN();
}

public class XL_BAN
{
    public string Ten = "";
    public string Ma_so = "";
    public string Trang_thai = "TRONG";
    //Xử lý người dùng
    public List<XL_MON_AN> Danh_sach_Mon_an_Cho_nau = new List<XL_MON_AN>();
}
