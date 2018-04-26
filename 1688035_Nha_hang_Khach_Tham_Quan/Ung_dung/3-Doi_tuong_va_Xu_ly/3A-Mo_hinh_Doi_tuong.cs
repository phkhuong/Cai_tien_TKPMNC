using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class XL_LAN_DANG_NHAP
{
    public DateTime Ngay = DateTime.Now;

}
public class XL_NGUOI_DUNG
{
    public string Ho_ten = "";
    public string Ma_so = "";
    public string Ten_Dang_nhap = "";
    public string Mat_khau = "";
    public XL_NHOM_NGUOI_DUNG Nhom_Nguoi_dung = new XL_NHOM_NGUOI_DUNG();
    public List<XL_LAN_DANG_NHAP> Danh_sach_Lan_Dang_nhap = new List<XL_LAN_DANG_NHAP>();
}
//*************************** Đối tượng Dữ liệu   *********
public partial class XL_DU_LIEU
{
    public XL_NHA_HANG Nha_hang = new XL_NHA_HANG();
    public List<XL_MON_AN> Danh_sach_Mon_an = new List<XL_MON_AN>();
    public List<XL_NGUOI_DUNG> Danh_sach_Nguoi_dung = new List<XL_NGUOI_DUNG>();
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
}

public class XL_LOAI_MON_AN
{
    public string Ten = "";
    public string Ma_so = "";
}

//=========== Đối tượng Con người ===============
public class XL_KHACH_THAM_QUAN
{
    public string Ho_ten, Ma_so = "", Ten_Dang_nhap, Mat_khau;
    public List<XL_LOAI_MON_AN> Danh_sach_Loai_Mon_an = new List<XL_LOAI_MON_AN>();
    public List<XL_MON_AN> Danh_sach_Mon_an = new List<XL_MON_AN>();
    // Online
    public List<XL_MON_AN> Danh_sach_Mon_an_Xem = new List<XL_MON_AN>();
    public XL_PHIEU_DAT_BAN Phieu_Dat_Ban = new XL_PHIEU_DAT_BAN();
}


public class XL_NHOM_NGUOI_DUNG
{
    public string Ten = "";
    public string Ma_so = "";
    public string Dia_chi_Dang_nhap = "";
}


//=========================== Doi tuong xu ly chinh =====================

public class XL_MON_AN
{
    public string Ma_so = "";
    public string Ten = "";
    public long Don_gia_Ban = 0;
    public XL_LOAI_MON_AN Loai_Mon_an = new XL_LOAI_MON_AN();
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
    public string Co_den = "TIEP_NHAN";
}

