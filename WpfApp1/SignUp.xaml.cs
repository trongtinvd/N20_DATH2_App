using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        private SqlConnection cnn;

        public SignUp()
        {
            InitializeComponent();
        }

        public SignUp(SqlConnection cnn) : this()
        {
            this.cnn = cnn;
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KhachHang kh = new KhachHang()
                {
                    Ten = TenInput.Text,
                    DiaChi = DiaChiInput.Text,
                    Sdt = SDTInput.Text,
                    Username = UserNameInput.Text,
                    Password = PasswordInput.Password
                };

                if (!AccountExists(kh))
                {
                    int rowCount = KhachHangSignUp(kh);
                    if (rowCount == 1)
                    {
                        MessageBox.Show("Đăng ký tài khoản thành công.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Đã sảy ra lỗi.");
                    }
                }
                else
                {
                    MessageBox.Show("Username này đã được sử dụng.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        private int KhachHangSignUp(KhachHang kh)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"INSERT KhachHang(Ten, DiaChi, SDT, UserName, Password, LoaiKH) 
                                    VALUES(@Ten, @DiaChi, @SDT, @UserName, @Password, @LoaiKH);";

            command.Parameters.Add("@Ten", System.Data.SqlDbType.NVarChar).Value = kh.Ten;
            command.Parameters.Add("@DiaChi", System.Data.SqlDbType.NVarChar).Value = kh.DiaChi;
            command.Parameters.Add("@SDT", System.Data.SqlDbType.NVarChar).Value = kh.Sdt;
            command.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar).Value = kh.Username;
            command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar).Value = kh.Password;
            command.Parameters.Add("@LoaiKH", System.Data.SqlDbType.NVarChar).Value = kh.LoaiKH;

            int rowCount = command.ExecuteNonQuery();
            return rowCount;
        }

        private bool AccountExists(KhachHang newKhachHang)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"select count(ID_KH) from KhachHang
                                    where username = @Username";

            command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar).Value = newKhachHang.Username;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    int count = reader.GetInt32(0);
                    if (count > 0)
                    {
                        command.Dispose();
                        return true;
                    }
                }
            }

            command.Dispose();
            return false;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
