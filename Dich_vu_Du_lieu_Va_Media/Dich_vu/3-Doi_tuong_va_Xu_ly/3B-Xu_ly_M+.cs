using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Globalization;
using Newtonsoft.Json;

//************************* M+ (Model for All ) **********************************
public partial class XL_DU_LIEU
{
    static XL_DU_LIEU Du_lieu_cua_Ung_dung;

    public static XL_DU_LIEU Khoi_dong_Du_lieu_Ung_dung()
    {   
        if (Du_lieu_cua_Ung_dung == null)
        {
            Du_lieu_cua_Ung_dung = Doc_Du_lieu();
        }
       
        return Du_lieu_cua_Ung_dung;
    }
}

//************************* Business-Layers BL **********************************
public partial class XL_DU_LIEU
{
    public XL_DU_LIEU Tao_Du_lieu_cua_Khach_tham_quan()
    {
        var Du_lieu_cua_Khach_tham_quan = new XL_DU_LIEU();
        Du_lieu_cua_Khach_tham_quan.Danh_sach_Nguoi_dung = Du_lieu_cua_Ung_dung.Danh_sach_Nguoi_dung;   // Dùng cho việc đăng nhập từ phân hệ khách tham quan
        Du_lieu_cua_Khach_tham_quan.Nha_hang.Ma_so = Du_lieu_cua_Ung_dung.Nha_hang.Ma_so;
        Du_lieu_cua_Khach_tham_quan.Nha_hang.Ten = Du_lieu_cua_Ung_dung.Nha_hang.Ten;
        Du_lieu_cua_Khach_tham_quan.Nha_hang.Mail = Du_lieu_cua_Ung_dung.Nha_hang.Mail;
        Du_lieu_cua_Khach_tham_quan.Nha_hang.Dien_thoai = Du_lieu_cua_Ung_dung.Nha_hang.Dien_thoai;
        Du_lieu_cua_Khach_tham_quan.Nha_hang.Dia_chi = Du_lieu_cua_Ung_dung.Nha_hang.Dia_chi;
        Du_lieu_cua_Khach_tham_quan.Nha_hang.Danh_sach_Loai_Mon_an = Du_lieu_cua_Ung_dung.Nha_hang.Danh_sach_Loai_Mon_an;

        foreach (XL_MON_AN Mon_an in Du_lieu_cua_Ung_dung.Danh_sach_Mon_an)
        {
            var Mon_an_cua_Phan_he = new XL_MON_AN();
            Mon_an_cua_Phan_he.Ten = Mon_an.Ten;
            Mon_an_cua_Phan_he.Ma_so = Mon_an.Ma_so;
            Mon_an_cua_Phan_he.Don_gia_Ban = Mon_an.Don_gia_Ban;
            Mon_an_cua_Phan_he.Loai_Mon_an = Mon_an.Loai_Mon_an;
            Du_lieu_cua_Khach_tham_quan.Danh_sach_Mon_an.Add(Mon_an_cua_Phan_he);
        }

        return Du_lieu_cua_Khach_tham_quan;
    }

    public XL_DU_LIEU Tao_Du_lieu_cua_Dau_bep()
    {
        var Du_lieu_cua_Dau_bep = new XL_DU_LIEU();
        Du_lieu_cua_Dau_bep.Danh_sach_Nguoi_dung = Du_lieu_cua_Ung_dung.Danh_sach_Nguoi_dung;
        Du_lieu_cua_Dau_bep.Nha_hang.Danh_sach_Dau_bep = Du_lieu_cua_Ung_dung.Nha_hang.Danh_sach_Dau_bep;
        Du_lieu_cua_Dau_bep.Nha_hang.Danh_sach_Nhan_vien_Phuc_vu = Du_lieu_cua_Ung_dung.Nha_hang.Danh_sach_Nhan_vien_Phuc_vu;
        Du_lieu_cua_Dau_bep.Nha_hang.Danh_sach_Loai_Mon_an = Du_lieu_cua_Ung_dung.Nha_hang.Danh_sach_Loai_Mon_an;
        Du_lieu_cua_Dau_bep.Nha_hang.Ma_so = Du_lieu_cua_Ung_dung.Nha_hang.Ma_so;
        Du_lieu_cua_Dau_bep.Nha_hang.Ten = Du_lieu_cua_Ung_dung.Nha_hang.Ten;
        foreach (XL_BAN Ban in Du_lieu_cua_Ung_dung.Danh_sach_Ban)
        {
            var Ban_cua_Phan_he = new XL_BAN();
            Ban_cua_Phan_he.Ten = Ban.Ten;
            Ban_cua_Phan_he.Ma_so = Ban.Ma_so;
            Ban_cua_Phan_he.Trang_thai = Ban.Trang_thai;
            Du_lieu_cua_Dau_bep.Danh_sach_Ban.Add(Ban_cua_Phan_he);
        }
        foreach (XL_MON_AN Mon_an in Du_lieu_cua_Ung_dung.Danh_sach_Mon_an)
        {
            var Mon_an_cua_Phan_he = new XL_MON_AN();
            Mon_an_cua_Phan_he.Ten = Mon_an.Ten;
            Mon_an_cua_Phan_he.Ma_so = Mon_an.Ma_so;
            Mon_an_cua_Phan_he.Loai_Mon_an = Mon_an.Loai_Mon_an;

            //**Hiện tại chưa dùng vì cần lấy dữ liệu ngày cũ để làm ví dụ biễu diễn

            //Mon_an_cua_Phan_he.Danh_sach_Goi_mon = Lay_Du_lieu_Goi_mon_trong_ngay(Mon_an);

            Mon_an_cua_Phan_he.Danh_sach_Goi_mon = Mon_an.Danh_sach_Goi_mon;
            //============================================================================//

            Du_lieu_cua_Dau_bep.Danh_sach_Mon_an.Add(Mon_an_cua_Phan_he);
        }
        return Du_lieu_cua_Dau_bep;
    }

    public XL_DU_LIEU Tao_Du_lieu_Dung_chung()
    {
        var Du_lieu = Du_lieu_cua_Ung_dung;

        Du_lieu.Danh_sach_Mon_an.ForEach(Mon_an =>
        {
            Mon_an.Danh_sach_Goi_mon.ForEach(Goi_mon => {
                // Chuyển sang múi giờ địa phương
                Goi_mon.Thoi_diem_Goi = Goi_mon.Thoi_diem_Goi.ToLocalTime();
                Goi_mon.Thoi_diem_Huy = Goi_mon.Thoi_diem_Huy.ToLocalTime();
                Goi_mon.Thoi_diem_Nau_xong = Goi_mon.Thoi_diem_Nau_xong.ToLocalTime();
                Goi_mon.Thoi_diem_Phuc_vu = Goi_mon.Thoi_diem_Phuc_vu.ToLocalTime();
                Goi_mon.Thoi_diem_Thanh_toan = Goi_mon.Thoi_diem_Thanh_toan.ToLocalTime();
            });
        });

        Du_lieu.Danh_sach_Ban.ForEach(Ban =>
        {
            // Khởi tạo danh sách phiếu tính tiền
            Ban.Danh_sach_Phieu_Tinh_tien.ForEach(Phieu_Tinh_tien => {
                // Chuyển sang múi giờ địa phương
                Phieu_Tinh_tien.Ngay = Phieu_Tinh_tien.Ngay.ToLocalTime();

                if (!Du_lieu.Danh_sach_Phieu_Tinh_tien.Contains(Phieu_Tinh_tien))
                {
                    Du_lieu.Danh_sach_Phieu_Tinh_tien.Add(Phieu_Tinh_tien);
                }
            });

            // Khởi tạo danh sách ý kiến
            Ban.Danh_sach_Y_kien.ForEach(Y_kien => {
                // Chuyển sang múi giờ địa phương
                Y_kien.Ngay = Y_kien.Ngay.ToLocalTime();

                // Chỉ lấy ra ý kiến của ngày hiện tại
                //if(Y_kien.Ngay.Day == DateTime.Today.Day && Y_kien.Ngay.Month == DateTime.Today.Month && Y_kien.Ngay.Year == DateTime.Today.Year) {
                if (!Du_lieu.Danh_sach_Y_kien.Contains(Y_kien))
                {
                    Y_kien.Ma_so_Ban = Ban.Ma_so;
                    Du_lieu.Danh_sach_Y_kien.Add(Y_kien);
                }
                //}
            });
        });

        // Thống kê tổng doanh thu theo bàn
        Du_lieu.Danh_sach_Ban.ForEach(Ban =>
        {
            if (Ban.Doanh_thu == 0)
            {
                Ban.Doanh_thu += Tinh_Doanh_thu_Ban(Ban);
            }
        });

        // Thống kê tổng doanh thu theo món ăn
        Du_lieu.Danh_sach_Mon_an.ForEach(Mon_an =>
        {
            if (Mon_an.Doanh_thu == 0)
            {
                Mon_an.Doanh_thu += Tinh_Doanh_thu_Mon_an(Mon_an);
            }
        });

        // Thống kê tổng doanh thu theo loại món ăn
        Du_lieu.Nha_hang.Danh_sach_Loai_Mon_an.ForEach(Loai_Mon_an =>
        {
            if (Loai_Mon_an.Doanh_thu == 0)
            {
                Loai_Mon_an.Doanh_thu += Tinh_Doanh_thu_Loai_Mon_an(Loai_Mon_an);
            }
        });

        return Du_lieu;
    }

    public XL_DU_LIEU Tao_Du_lieu_cua_Nhan_vien_Phuc_vu()
    {
        var Du_lieu = Tao_Du_lieu_Dung_chung();

        // Xoá dữ liệu không cần thiết
        Du_lieu.Danh_sach_Phieu_Dat_ban = null;
        Du_lieu.Nha_hang.Danh_sach_Dau_bep = null;
        Du_lieu.Nha_hang.Danh_sach_Quan_ly = null;

        return Du_lieu;
    }

    public XL_DU_LIEU Tao_Du_lieu_cua_Quan_ly_Nha_hang()
    {
        var Du_lieu = Tao_Du_lieu_Dung_chung();

        return Du_lieu;
    }

    //Tinh toan
    public List<XL_GOI_MON> Lay_Du_lieu_Goi_mon_trong_ngay(XL_MON_AN Mon_an)
    {
        var Danh_sach_Goi_mon_trong_ngay = Mon_an.Danh_sach_Goi_mon.FindAll(Goi_mon => Goi_mon.Thoi_diem_Goi.Date == DateTime.Today.Date);
        return Danh_sach_Goi_mon_trong_ngay;
    }

    public long Tinh_Doanh_thu_Ban(XL_BAN Ban)
    {
        long Doanh_thu = 0;

        Ban.Danh_sach_Phieu_Tinh_tien.ForEach(Phieu_Tinh_tien => {
            Doanh_thu += Phieu_Tinh_tien.Tong_tien;
        });

        return Doanh_thu;
    }

    public long Tinh_Doanh_thu_Mon_an(XL_MON_AN Mon_an)
    {
        long Doanh_thu = 0;

        Mon_an.Danh_sach_Goi_mon.ForEach(Goi_mon => {
            if (Goi_mon.Trang_thai == "DA_THANH_TOAN")
            {
                Doanh_thu += Goi_mon.Thanh_tien;
            }
        });

        return Doanh_thu;
    }

    public long Tinh_Doanh_thu_Loai_Mon_an(XL_LOAI_MON_AN Loai_Mon_an)
    {
        long Doanh_thu = 0;

        Danh_sach_Mon_an.ForEach(Mon_an =>
        {
            if (Mon_an.Loai_Mon_an.Ma_so == Loai_Mon_an.Ma_so)
            {
                Doanh_thu += Mon_an.Doanh_thu;
            }
        });

        return Doanh_thu;
    }

    public string Ghi_Phieu_Tinh_tien(XL_PHIEU_TINH_TIEN Phieu_Tinh_tien, string Ma_so_Ban, XL_Y_KIEN Y_kien = null)
    {
        var Kq = "";

        var Ban = Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Ma_so_Ban);

        Ban.Danh_sach_Phieu_Tinh_tien.Add(Phieu_Tinh_tien);
        Ban.Trang_thai = "TRONG";

        if(Y_kien != null)
        {
            Y_kien.Nhan_vien_Phuc_vu = Phieu_Tinh_tien.Nhan_vien_Phuc_vu;
            Ban.Danh_sach_Y_kien.Add(Y_kien);
        }

        Kq = Ghi_Ban(Ban);

        if(Kq == "OK")
        {
            Phieu_Tinh_tien.Danh_sach_Goi_mon.ForEach(Ma_so_Goi_mon => {
                Danh_sach_Mon_an.ForEach(Mon_an => {
                    var Goi_mon = Mon_an.Danh_sach_Goi_mon.FirstOrDefault(x => x.Ma_so == Ma_so_Goi_mon);

                    if(Goi_mon != null)
                    {
                        Goi_mon.Trang_thai = "DA_THANH_TOAN";
                        Goi_mon.Thoi_diem_Thanh_toan = DateTime.Now;

                        Ghi_Mon_an(Mon_an);
                    }
                    
                });
            });
        }

        return Kq;
    }
    public string Ghi_Cap_nhap_Trang_thai_Ban(string Ma_so_Ban, string Trang_thai)
    {
        var Kq = "";
        var Ban = Danh_sach_Ban.FirstOrDefault(x => x.Ma_so == Ma_so_Ban);
        if(Ban != null)
        {
            var Trang_thai_Cu = Ban.Trang_thai;
            Ban.Trang_thai = Trang_thai;
            Kq = Ghi_Ban(Ban);
            if(Kq != "OK")
            {
                Ban.Trang_thai = Trang_thai_Cu;
            }
        }
        else
        {
            Kq = $"{Ban.Ma_so}_doesnt_exist";
        }
        return Kq;
    }

    public string Ghi_Sua_Trang_Thai_Goi_Mon(string Ma_so_Mon_an, string Ma_so_Goi_mon, string Trang_thai)
    {
        var Kq = "";
        var Mon_an = Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
        
        if (Mon_an != null)
        {
            var Goi_mon = Mon_an.Danh_sach_Goi_mon.FirstOrDefault(x => x.Ma_so == Ma_so_Goi_mon);
            if(Goi_mon != null)
            {
                var Trang_thai_Cu = Goi_mon.Trang_thai;
                Goi_mon.Trang_thai = Trang_thai;
                switch (Trang_thai)
                {
                    case "DA_NAU":
                        Goi_mon.Thoi_diem_Nau_xong = DateTime.Now;
                        break;
                    case "DA_PHUC_VU":
                        Goi_mon.Thoi_diem_Phuc_vu = DateTime.Now;
                        break;
                    case "HUY":
                        Goi_mon.Thoi_diem_Huy = DateTime.Now;
                        break;
                    default:
                        Kq = "Wrong Trang_thai";
                        break;
                }
                if(Kq == "")
                {
                    Kq = Ghi_Mon_an(Mon_an);
                    if (Kq != "OK")
                    {
                        Goi_mon.Trang_thai = Trang_thai_Cu;
                    }
                }
                else
                {
                    return Kq;
                }
            }
            else
            {
                Kq = $"{Goi_mon.Ma_so}_doesn't_exist";
            }
        }
        else
        {
            Kq = $"{Mon_an.Ma_so}_doesn't_exist";
        }

        return Kq;
    }

    public string Ghi_Goi_mon_moi(string Ma_so_Mon_an, XL_GOI_MON Goi_mon)
    {
        var Kq = "";
        var Mon_an = Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
        Mon_an.Danh_sach_Goi_mon.Add(Goi_mon);
        Kq = Ghi_Mon_an(Mon_an);
        if(Kq != "OK")
        {
            Mon_an.Danh_sach_Goi_mon.Remove(Goi_mon);
        }
        return Kq;
    }
}

//************************* Data-Layers DL **********************************
public partial class XL_DU_LIEU
{    
    static DirectoryInfo Thu_muc_Project = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath);
    static DirectoryInfo Thu_muc_Du_lieu = Thu_muc_Project.GetDirectories("2-Du_lieu_luu_tru")[0];
    static DirectoryInfo Thu_muc_Cua_hang = Thu_muc_Du_lieu.GetDirectories("Nha_hang")[0];
    static DirectoryInfo Thu_muc_Nguoi_dung = Thu_muc_Du_lieu.GetDirectories("Nguoi_dung")[0];
    static DirectoryInfo Thu_muc_Ban = Thu_muc_Du_lieu.GetDirectories("Ban")[0];
    static DirectoryInfo Thu_muc_Mon_an = Thu_muc_Du_lieu.GetDirectories("Mon_an")[0];
    static DirectoryInfo Thu_muc_Phieu_Dat_ban = Thu_muc_Du_lieu.GetDirectories("Phieu_Dat_ban")[0];

    static XL_DU_LIEU Doc_Du_lieu()
    {
        Du_lieu_cua_Ung_dung = new XL_DU_LIEU();
        Du_lieu_cua_Ung_dung.Nha_hang = Doc_Danh_sach_Nha_hang()[0];
        Du_lieu_cua_Ung_dung.Danh_sach_Nguoi_dung = Doc_Danh_sach_Nguoi_dung();
        Du_lieu_cua_Ung_dung.Danh_sach_Ban = Doc_Danh_sach_Ban();
        Du_lieu_cua_Ung_dung.Danh_sach_Mon_an = Doc_Danh_sach_Mon_an();
        Du_lieu_cua_Ung_dung.Danh_sach_Phieu_Dat_ban = Doc_Danh_sach_Phieu_Dat_ban();

        return Du_lieu_cua_Ung_dung;
    }

    static List<XL_NHA_HANG> Doc_Danh_sach_Nha_hang()
    {
        var Danh_sach_Nha_hang = new List<XL_NHA_HANG>();
        var Danh_sach_Tap_tin = Thu_muc_Cua_hang.GetFiles("*.json").ToList();

        Danh_sach_Tap_tin.ForEach(Tap_tin =>
        {
            var Duong_dan = Tap_tin.FullName;
            var Chuoi_JSON = File.ReadAllText(Duong_dan);
            var Cua_hang = JsonConvert.DeserializeObject<XL_NHA_HANG>(Chuoi_JSON);
            Danh_sach_Nha_hang.Add(Cua_hang);
        });

        return Danh_sach_Nha_hang;
    }

    static List<XL_NGUOI_DUNG> Doc_Danh_sach_Nguoi_dung()
    {
        var Danh_sach_Nguoi_dung = new List<XL_NGUOI_DUNG>();
        var Danh_sach_Tap_tin = Thu_muc_Nguoi_dung.GetFiles("*.json").ToList();

        Danh_sach_Tap_tin.ForEach(Tap_tin =>
        {
            var Duong_dan = Tap_tin.FullName;
            var Chuoi_JSON = File.ReadAllText(Duong_dan);
            var Ban = JsonConvert.DeserializeObject<XL_NGUOI_DUNG>(Chuoi_JSON);
            Danh_sach_Nguoi_dung.Add(Ban);
        });

        return Danh_sach_Nguoi_dung;
    }

    static List<XL_BAN> Doc_Danh_sach_Ban()
    {
        var Danh_sach_Ban = new List<XL_BAN>();
        var Danh_sach_Tap_tin = Thu_muc_Ban.GetFiles("*.json").ToList();

        Danh_sach_Tap_tin.ForEach(Tap_tin =>
        {
            var Duong_dan = Tap_tin.FullName;
            var Chuoi_JSON = File.ReadAllText(Duong_dan);
            var Ban = JsonConvert.DeserializeObject<XL_BAN>(Chuoi_JSON);
            Danh_sach_Ban.Add(Ban);
        });

        return Danh_sach_Ban;
    }

    static List<XL_MON_AN> Doc_Danh_sach_Mon_an()
    {
        var Danh_sach_Mon_an = new List<XL_MON_AN>();
        var Danh_sach_Tap_tin = Thu_muc_Mon_an.GetFiles("*.json").ToList();

        Danh_sach_Tap_tin.ForEach(Tap_tin =>
        {
            var Duong_dan = Tap_tin.FullName;
            var Chuoi_JSON = File.ReadAllText(Duong_dan);
            var Mon_an = JsonConvert.DeserializeObject<XL_MON_AN>(Chuoi_JSON);
            Danh_sach_Mon_an.Add(Mon_an);
        });

        return Danh_sach_Mon_an;
    }

    static List<XL_PHIEU_DAT_BAN> Doc_Danh_sach_Phieu_Dat_ban()
    {
        var Danh_sach_Phieu_Dat_ban = new List<XL_PHIEU_DAT_BAN>();
        var Danh_sach_Tap_tin = Thu_muc_Phieu_Dat_ban.GetFiles("*.json").ToList();

        Danh_sach_Tap_tin.ForEach(Tap_tin =>
        {
            var Duong_dan = Tap_tin.FullName;
            var Chuoi_JSON = File.ReadAllText(Duong_dan);
            var Phieu_Dat_ban = JsonConvert.DeserializeObject<XL_PHIEU_DAT_BAN>(Chuoi_JSON);
            Danh_sach_Phieu_Dat_ban.Add(Phieu_Dat_ban);
        });

        return Danh_sach_Phieu_Dat_ban;
    }

    

    public string Ghi_Mon_an(XL_MON_AN Mon_an)
    {
        var Kq = "";
        var Duong_dan = $"{Thu_muc_Mon_an.FullName}\\{Mon_an.Ma_so}.json";
        var Chuoi_JSON = JsonConvert.SerializeObject(Mon_an, Formatting.Indented);

        try
        {
            File.WriteAllText(Duong_dan, Chuoi_JSON);
            Kq = "OK";

        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
        }

        return Kq;
    }

    public string Cap_nhat_Gia_ban(string Ma_so_Mon_an, string Gia_moi)
    {
        var Kq = "";

        var Mon_an = Danh_sach_Mon_an.FirstOrDefault(x => x.Ma_so == Ma_so_Mon_an);
        var Don_gia_cu = Mon_an.Don_gia_Ban;
        Mon_an.Don_gia_Ban = long.Parse(Gia_moi);

        var Duong_dan = $"{Thu_muc_Mon_an.FullName}\\{Mon_an.Ma_so}.json";
        var Chuoi_JSON = JsonConvert.SerializeObject(Mon_an, Formatting.Indented);

        try
        {
            File.WriteAllText(Duong_dan, Chuoi_JSON);
            Kq = "OK";
        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
            Mon_an.Don_gia_Ban = Don_gia_cu;
        }

        return Kq;
    }

    public string Ghi_Ban(XL_BAN Ban)
    {
        var Kq = "";
        var Duong_dan = $"{Thu_muc_Ban.FullName}\\{Ban.Ma_so}.json";
        var Chuoi_JSON = JsonConvert.SerializeObject(Ban, Formatting.Indented);
        try
        {
            File.WriteAllText(Duong_dan, Chuoi_JSON);
            Kq = "OK";

        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
        }
        return Kq;

    }

    public string Ghi_Phieu_Dat_Ban_Moi(XL_PHIEU_DAT_BAN Phieu_Dat_Ban)
    {
        var Kq = "";
        var Tong_so_Phieu_Dat_Ban = Du_lieu_cua_Ung_dung.Danh_sach_Phieu_Dat_ban.Count();
        Tong_so_Phieu_Dat_Ban++;
        Phieu_Dat_Ban.Ma_so = "Phieu_Dat_ban_" + Tong_so_Phieu_Dat_Ban.ToString();
        var Duong_dan = Thu_muc_Phieu_Dat_ban.FullName + $"\\{Phieu_Dat_Ban.Ma_so}.json";
        if (File.Exists(Duong_dan))
        {
            return "Phieu dat already exists";
        }
        try
        {
            Du_lieu_cua_Ung_dung.Danh_sach_Phieu_Dat_ban.Add(Phieu_Dat_Ban);
            var Chuoi_JSON = JsonConvert.SerializeObject(Phieu_Dat_Ban, Formatting.Indented);
            File.WriteAllText(Duong_dan, Chuoi_JSON);
            Kq = "OK";
        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
        }
        if (Kq != "OK" && Phieu_Dat_Ban != null)
        {
            Du_lieu_cua_Ung_dung.Danh_sach_Phieu_Dat_ban.Remove(Phieu_Dat_Ban);
            if (File.Exists(Duong_dan))
                File.Delete(Duong_dan);
        }
        return Kq;

    }

    public string Cap_nhat_Trang_thai_Phieu_Dat_ban(string Ma_so_Phieu_Dat_ban, string Trang_thai)
    {
        var Kq = "";

        var Phieu_Dat_ban = Danh_sach_Phieu_Dat_ban.FirstOrDefault(x => x.Ma_so == Ma_so_Phieu_Dat_ban);
        var Trang_thai_cu = Phieu_Dat_ban.Trang_thai;
        Phieu_Dat_ban.Trang_thai = Trang_thai;

        var Duong_dan = $"{Thu_muc_Phieu_Dat_ban.FullName}\\{Phieu_Dat_ban.Ma_so}.json";
        var Chuoi_JSON = JsonConvert.SerializeObject(Phieu_Dat_ban, Formatting.Indented);

        try
        {
            File.WriteAllText(Duong_dan, Chuoi_JSON);
            Kq = "OK";
        }
        catch (Exception Loi)
        {
            Kq = Loi.Message;
            Phieu_Dat_ban.Trang_thai = Trang_thai_cu;
        }

        return Kq;
    }
}