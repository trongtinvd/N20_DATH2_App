using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Common;
using System.Drawing;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection cnn = null;
        private KhachHang kh;
        private SanPham selectedSP = null;
        private GioHang gioHang = new GioHang();


        public MainWindow()
        {
            InitializeComponent();
            ConnectionStringInput.Text = "Data Source=TrongtinvdPC;Initial Catalog=DA2;User ID=sa;Password=password";
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectDB();
                MessageBox.Show("Kết nối tới CSDL thành công!");
                UpdateSanPhamList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateSanPhamList()
        {
            SanPhamList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"
                                    select ID_SP, Ten, Gia, MoTa, LoaiSP 
                                    from SanPham;
                                    ";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int STT = 1;
                    while (reader.Read())
                    {
                        int ID_SP = reader.GetInt32(0);
                        string Ten = reader.GetString(1);
                        decimal Gia = reader.GetDecimal(2);
                        string MoTa = reader.GetString(3);
                        string LoaiSP = reader.GetString(4);
                        SanPhamList.Items.Add(new SanPham()
                        {
                            STT = STT,
                            ID_SP = ID_SP,
                            Ten = Ten,
                            Gia = Gia,
                            MoTa = MoTa,
                            LoaiSP = LoaiSP
                        });
                        STT++;
                    }
                }
            }
            command.Dispose();
        }

        private void ConnectDB()
        {
            string connectionString = ConnectionStringInput.Text;
            cnn = new SqlConnection(connectionString);
            cnn.Open();
        }

        private void SanPhamList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = sender as ListViewItem;
                if (item != null && item.IsSelected)
                {
                    SanPham SP = item.Content as SanPham;
                    MoTaSanPham.Text = SP.MoTa;
                    UpdateChiTietBoHoaList(SP);
                    UpdateSelectedSanPham(SP);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void UpdateSelectedSanPham(SanPham SP)
        {
            selectedSP = SP;
            SelectedSPLabel.Content = selectedSP.Ten;
        }

        private void UpdateChiTietBoHoaList(SanPham SP)
        {
            ChiTietBoHoaList.Items.Clear();

            if (SP.LoaiSP != "BoHoa")
                return;

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"
                                    select sp.Ten, ctbh.SoLuongHoa
                                    from SanPham sp
                                    join ChiTietBoHoa ctbh
                                    on sp.ID_SP = ctbh.ID_SP_Hoa
                                    where ctbh.ID_SP_BoHoa = @ID_SP_BoHoa
                                    ";

            command.Parameters.Add("@ID_SP_BoHoa", System.Data.SqlDbType.Int).Value = SP.ID_SP;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int STT = 1;
                    while (reader.Read())
                    {
                        string Ten = reader.GetString(0);
                        int SoLuongHoa = reader.GetInt32(1);
                        ChiTietBoHoaList.Items.Add(new
                        {
                            STT = STT,
                            Ten = Ten,
                            SoLuongHoa = SoLuongHoa
                        });

                        STT++;
                    }
                }
            }
            command.Dispose();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cnn == null)
                {
                    MessageBox.Show("Chưa kết nối đến CSDL.");
                    return;
                }
                SignUp signUp = new SignUp(cnn);
                signUp.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cnn != null)
            {
                cnn.Close();
                cnn.Dispose();
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cnn == null)
                {
                    MessageBox.Show("Chưa kết nối đến CSDL.");
                    return;
                }

                string username = UsernameInput.Text;
                string password = PasswordInput.Password;
                KhachHang kh = Login(username, password);
                if (kh != null)
                {
                    MessageBox.Show("Login thành công.");
                    this.kh = kh;
                    UpdateKhachHangUI();
                    UpdateDonHangList();
                }
                else
                {
                    MessageBox.Show("Username hoặc password không đúng.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }


        }

        private void UpdateDonHangList()
        {
            DonHangList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"select ID_DH, ID_KH, ID_NVBanHang, ID_NVGiaoHang, ID_HD, SoLuongSP, DiaChiGiaoHang, TongGia, ThoiDiemDatHang, ThoiDiemDuyetDH, ThoiDiemHoanThanh, ThoiDiemHuy
                                    from DonHang;";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int ID_DH = -1;
                        int ID_KH = -1;
                        int ID_NVBanHang = -1;
                        int ID_NVGiaoHang = -1;
                        int ID_HD = -1;
                        int SoLuongSP = -1;
                        string DiaChiGiaoHang = "";
                        decimal TongGia = -1;
                        string ThoiDiemDatHang = "";
                        string ThoiDiemDuyetDH = "";
                        string ThoiDiemHoanThanh = "";
                        string ThoiDiemHuy = "";

                        if (!reader.IsDBNull(0)) ID_DH = reader.GetInt32(0);
                        if (!reader.IsDBNull(1)) ID_KH = reader.GetInt32(1);
                        if (!reader.IsDBNull(2)) ID_NVBanHang = reader.GetInt32(2);
                        if (!reader.IsDBNull(3)) ID_NVGiaoHang = reader.GetInt32(3);
                        if (!reader.IsDBNull(4)) ID_HD = reader.GetInt32(4);
                        if (!reader.IsDBNull(5)) SoLuongSP = reader.GetInt32(5);
                        if (!reader.IsDBNull(6)) DiaChiGiaoHang = reader.GetString(6);
                        if (!reader.IsDBNull(7)) TongGia = reader.GetDecimal(7);
                        if (!reader.IsDBNull(8)) ThoiDiemDatHang = reader.GetDateTime(8).ToString();
                        if (!reader.IsDBNull(9)) ThoiDiemDuyetDH = reader.GetDateTime(9).ToString();
                        if (!reader.IsDBNull(10)) ThoiDiemHoanThanh = reader.GetDateTime(10).ToString();
                        if (!reader.IsDBNull(11)) ThoiDiemHuy = reader.GetDateTime(11).ToString();

                        DonHangList.Items.Add(new DonHang()
                        {
                            ID_DH = ID_DH,
                            ID_KH = ID_KH,
                            ID_NVBanHang = ID_NVBanHang,
                            ID_NVGiaoHang = ID_NVGiaoHang,
                            ID_HD = ID_HD,
                            SoLuongSP = SoLuongSP,
                            DiaChiGiaoHang = DiaChiGiaoHang,
                            TongGia = TongGia,
                            ThoiDiemDatHang = ThoiDiemDatHang,
                            ThoiDiemDuyetDH = ThoiDiemDuyetDH,
                            ThoiDiemHoanThanh = ThoiDiemHoanThanh,
                            ThoiDiemHuy = ThoiDiemHuy

                        });
                    }
                }
            }
            command.Dispose();
        }

        private void UpdateKhachHangUI()
        {
            if (kh != null)
            {
                TenKhachHangLabel.Content = kh.Ten;
                SdtLabel.Content = kh.Sdt;
            }
        }

        private KhachHang Login(string username, string password)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"select ID_KH, Ten, SDT, DiaChi, LoaiKH, UserName, Password
                                    from KhachHang
                                    where UserName = @Username and Password = @Password";

            command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar).Value = username;
            command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar).Value = password;


            KhachHang loginKh = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    loginKh = new KhachHang()
                    {
                        ID_KH = reader.GetInt32(0),
                        Ten = reader.GetString(1),
                        Sdt = reader.GetString(2),
                        DiaChi = reader.GetString(3),
                        LoaiKH = reader.GetString(4),
                        Username = reader.GetString(5),
                        Password = reader.GetString(6)
                    };
                }
            }
            command.Dispose();
            return loginKh;
        }

        private void AddSpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int soLuong = int.Parse(SoLuongSpInput.Text);

                if (soLuong < 1)
                {
                    MessageBox.Show("Số lượng SP phải lớn hơn 0.");
                    return;
                }

                gioHang.AddItem(selectedSP, soLuong);
                GioHangList.ItemsSource = gioHang.Items;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            GioHang.Item item = (sender as Button).DataContext as GioHang.Item;
            gioHang.Remove(item.SP);
            GioHangList.ItemsSource = gioHang.Items;
        }

        private void DatHangBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (kh == null)
                {
                    MessageBox.Show("Bạn chưa đăng nhập.");
                    return;
                }
                if (GioHangList.Items.Count == 0)
                {
                    MessageBox.Show("Giỏ hàng chưa có sản phẩm.");
                    return;
                }

                string diaChiGiaoHang = DiaChiGiaoHangInput.Text;
                if (diaChiGiaoHang.Length == 0)
                {
                    MessageBox.Show("Xin hãy nhập địa chỉ giao hàng.");
                    return;
                }
                int ID_DH = TaoDonHang(kh, diaChiGiaoHang);

                foreach (GioHang.Item item in gioHang.Items)
                {
                    TaoChiTietDonHang(ID_DH, item.SP.ID_SP, item.SoLuong);
                }

                UpdateDonHangList();

                gioHang.Clear();
                GioHangList.ItemsSource = gioHang.Items;
                MessageBox.Show("Tạo đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        private void TaoChiTietDonHang(int ID_DH, int ID_SP, int soLuong)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"insert into ChiTietDonHang(ID_DH, ID_SP, SoLuong)
                                    values(@ID_DH, @ID_SP, @SoLuong)";

            command.Parameters.Add("@ID_DH", System.Data.SqlDbType.Int).Value = ID_DH;
            command.Parameters.Add("@ID_SP", System.Data.SqlDbType.Int).Value = ID_SP;
            command.Parameters.Add("@SoLuong", System.Data.SqlDbType.Int).Value = soLuong;

            command.ExecuteNonQuery();
            command.Dispose();
        }

        private int TaoDonHang(KhachHang kh, string diaChiGiaoHang)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"insert into DonHang(ID_KH, DiaChiGiaoHang)
                                    values(@ID_KH, @DiaChiGiaoHang);
                                    SELECT SCOPE_IDENTITY();";

            command.Parameters.Add("@ID_KH", System.Data.SqlDbType.Int).Value = kh.ID_KH;
            command.Parameters.Add("@DiaChiGiaoHang", System.Data.SqlDbType.NVarChar).Value = diaChiGiaoHang;


            int ID_DH = -1;
            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    ID_DH = Convert.ToInt32(reader.GetDecimal(0));
                }
            }
            command.Dispose();
            return ID_DH;
        }

        private void DonHangList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = sender as ListViewItem;
                if (item != null && item.IsSelected)
                {
                    DonHang DH = item.Content as DonHang;
                    UpdateDonHangListThongTinDonHang(DH.ID_DH);
                    UpdateChiTietDonHang(DH.ID_DH);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateDonHangListThongTinDonHang(int ID_DH)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"select ThoiDiemDatHang, ThoiDiemDuyetDH, ThoiDiemHoanThanh, ThoiDiemHuy, DiaChiGiaoHang
                                    from DonHang
                                    where ID_DH = @ID_DH;";

            command.Parameters.Add("@ID_DH", System.Data.SqlDbType.Int).Value = ID_DH;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    string ThoiDiemDatHang = "";
                    string ThoiDiemDuyetDH = "";
                    string ThoiDiemHoanThanh = "";
                    string ThoiDiemHuy = "";
                    string DiaChiGiaoHang = "";

                    if (!reader.IsDBNull(0)) ThoiDiemDatHang = reader.GetDateTime(0).ToString();
                    if (!reader.IsDBNull(1)) ThoiDiemDuyetDH = reader.GetDateTime(1).ToString();
                    if (!reader.IsDBNull(2)) ThoiDiemHoanThanh = reader.GetDateTime(2).ToString();
                    if (!reader.IsDBNull(3)) ThoiDiemHuy = reader.GetDateTime(3).ToString();
                    if (!reader.IsDBNull(4)) DiaChiGiaoHang = reader.GetString(4);

                    ThoiDiemTaoLabel.Content = ThoiDiemDatHang;
                    ThoiDiemDuyetLabel.Content = ThoiDiemDuyetDH;
                    ThoiDiemHoanThanhLabel.Content = ThoiDiemHoanThanh;
                    ThoiDiemHuyLabel.Content = ThoiDiemHuy;
                    DiaChiGiaoHangLabel.Content = DiaChiGiaoHang;
                }
            }
            command.Dispose();
        }

        private void UpdateChiTietDonHang(int ID_DH)
        {
            ChiTietDonHangList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"select sp.Ten, sp.Gia, ct.SoLuong, ct.TongGia
                                    from ChiTietDonHang ct
                                    join SanPham sp
                                    on sp.ID_SP = ct.ID_SP
                                    where ct.ID_DH = @ID_DH;";

            command.Parameters.Add("@ID_DH", System.Data.SqlDbType.Int).Value = ID_DH;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ChiTietDonHangList.Items.Add(new
                        {
                            Ten = reader.GetString(0),
                            Gia = reader.GetDecimal(1),
                            SoLuong = reader.GetInt32(2),
                            TongGia = reader.GetDecimal(3)
                        });
                    }
                }
            }

            command.Dispose();
        }
    }
}
