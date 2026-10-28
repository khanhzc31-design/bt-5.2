using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Bai4._2
{
    public partial class Form1 : Form
    {

        private string connectionString = @"Server=.;Database=QuanLyHocVien;Trusted_Connection=True;TrustServerCertificate=True;";

        public class Course
        {
            public string CourseID { get; set; }
            public string CourseName { get; set; }

            public Course(string id, string name)
            {
                CourseID = id;
                CourseName = name;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            dtpBirthDate.Format = DateTimePickerFormat.Custom;
            dtpBirthDate.CustomFormat = "dd/MM/yyyy";
            mtxtPhone.Mask = "(000) 000-0000";

            LoadCoursesFromSQL();
        }

        private void LoadCoursesFromSQL()
        {
            List<Course> listCourses = new List<Course>();
            string query = "SELECT MaKhoaHoc, TenKhoaHoc FROM KhoaHoc";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string id = reader["MaKhoaHoc"].ToString();
                                string name = reader["TenKhoaHoc"].ToString();
                                listCourses.Add(new Course(id, name));
                            }
                        }
                    }

                    cboCourse.DataSource = listCourses;
                    cboCourse.DisplayMember = "CourseName";
                    cboCourse.ValueMember = "CourseID";
                }
                catch (Exception)
                {

                    listCourses = new List<Course>
                    {
                        new Course("101", "Lập trình C# WinForms"),
                        new Course("102", "Lập trình Web ASP.NET"),
                        new Course("103", "Cơ sở dữ liệu SQL Server"),
                        new Course("104", "Lập trình Python")
                    };

                    cboCourse.DataSource = listCourses;
                    cboCourse.DisplayMember = "CourseName";
                    cboCourse.ValueMember = "CourseID";
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string gender = rdoMale.Checked ? "Nam" : (rdoNu.Checked ? "Nữ" : "Chưa chọn");

            List<string> selectedSkills = new List<string>();
            if (tienganh.Checked) selectedSkills.Add(tienganh.Text);
            if (laptrinh.Checked) selectedSkills.Add(laptrinh.Text);
            if (thietke.Checked) selectedSkills.Add(thietke.Text);

            string skillsText = selectedSkills.Count > 0
                ? string.Join(", ", selectedSkills)
                : "Không chọn";

            string courseName = cboCourse.Text;
            string courseId = cboCourse.SelectedValue?.ToString() ?? "";
            DateTime birthDateVal = dtpBirthDate.Value;
            string birthDateStr = birthDateVal.ToString("dd/MM/yyyy");
            string phone = mtxtPhone.Text;

   
            if (string.IsNullOrWhiteSpace(phone) || !mtxtPhone.MaskCompleted)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtxtPhone.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

           
                    string getMaxIdQuery = "SELECT ISNULL(MAX(MaHV), 0) + 1 FROM HocVien";
                    SqlCommand cmdMax = new SqlCommand(getMaxIdQuery, conn);
                    int newMaHV = Convert.ToInt32(cmdMax.ExecuteScalar());

      
                    string insertQuery = @"INSERT INTO HocVien (MaHV, HoTen, SoDienThoai, NgaySinh, GioiTinh, MaKhoaHoc, KyNang) 
                                           VALUES (@MaHV, @HoTen, @Phone, @NgaySinh, @GioiTinh, @MaKhoaHoc, @KyNang)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaHV", newMaHV);
                        cmd.Parameters.AddWithValue("@HoTen", "Học viên"); 
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@NgaySinh", birthDateVal);
                        cmd.Parameters.AddWithValue("@GioiTinh", gender);
                        cmd.Parameters.AddWithValue("@MaKhoaHoc", courseId);
                        cmd.Parameters.AddWithValue("@KyNang", skillsText);

                        cmd.ExecuteNonQuery();
                    }

                    string info = $"=== ĐĂNG KÝ THÀNH CÔNG (ĐÃ LƯU SQL) ===\n\n" +
                                  $"• Mã học viên: {newMaHV}\n" +
                                  $"• SĐT: {phone}\n" +
                                  $"• Ngày sinh: {birthDateStr}\n" +
                                  $"• Giới tính: {gender}\n" +
                                  $"• Khóa học: {courseName} (Mã: {courseId})\n" +
                                  $"• Kỹ năng: {skillsText}";

                    MessageBox.Show(info, "Kết quả đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi lưu vào SQL: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tienganh_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}