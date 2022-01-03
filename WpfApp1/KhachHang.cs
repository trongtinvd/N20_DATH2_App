namespace WpfApp1
{
    internal class KhachHang
    {
        public KhachHang()
        {
            LoaiKH = "Online";
        }

        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string Sdt { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LoaiKH { get; set; }
        public int ID_KH { get; internal set; }
    }
}