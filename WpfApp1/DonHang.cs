using System;

namespace WpfApp1
{
    internal class DonHang
    {
        public DonHang()
        {
        }

        public int ID_DH { get; set; }
        public int ID_KH { get; set; }
        public int ID_NVBanHang { get; set; }
        public int ID_NVGiaoHang { get; set; }
        public int SoLuongSP { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public decimal TongGia { get; set; }
        public string ThoiDiemDatHang { get; set; }
        public string ThoiDiemDuyetDH { get; set; }
        public string ThoiDiemHoanThanh { get; set; }
        public string ThoiDiemHuy { get; set; }
        public int ID_HD { get; internal set; }

        public string TrangThai
        {
            get
            {
                if (ThoiDiemHuy != "")
                    return "Bị hủy";
                if (ThoiDiemHoanThanh != "")
                    return "Hoàn thành";
                if (ThoiDiemDuyetDH != "")
                    return "Chờ giao";
                return "Chờ duyệt";
            }
        }
    }
}