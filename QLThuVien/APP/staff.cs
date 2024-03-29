﻿using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using QLThuVien.BLL;
using QLThuVien.DAL;
using QLThuVien.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Z.Dapper.Plus;

namespace QLThuVien.APP
{
    public partial class staff : Form
    {
        public staff()
        {
            InitializeComponent();
        }

        SqlConnection con = DBConnect.GetDBConnection();

        public void HienThi()
        {
            string sqlSelect = "select * from staff";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            //Giải Mã
            MD5_algorithm md5 = new MD5_algorithm();
            foreach (DataRow row in dt.Rows)
            {
                row["name"] = md5.GiaiMaSring(row["name"].ToString(), "NguyenKhaDang");
                row["gender"] = md5.GiaiMaSring(row["gender"].ToString(), "NguyenKhaDang");
                row["designation_id"] = md5.GiaiMaSring(row["designation_id"].ToString(), "NguyenKhaDang");
                row["address"] = md5.GiaiMaSring(row["address"].ToString(), "NguyenKhaDang");
                row["phone"] = md5.GiaiMaSring(row["phone"].ToString(), "NguyenKhaDang");

            }
            dataView.DataSource = dt;
        }
        
        ID_Check dieuKien = new ID_Check();
        private void staff_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLThuVienDataSet.staff' table. You can move, or remove it, as needed.
            //this.staffTableAdapter.Fill(this.qLThuVienDataSet.staff);
            con.Open();
            HienThi();

            SqlCommand cmd = new SqlCommand("select * from designation", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.ExecuteNonQuery();

            tbDesignation_id.DataSource = ds.Tables[0];
            tbDesignation_id.DisplayMember = "designation_id";
            //tbDesignation_id.ValueMember = "designation_id";
        }

        private void staff_FormClosing(object sender, FormClosingEventArgs e)
        {
            con.Close();
        }

        private void btInsert_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbStaff_id.Text) || String.IsNullOrEmpty(tbName.Text) || String.IsNullOrEmpty(tbGender.Text) || String.IsNullOrEmpty(tbDesignation_id.Text) || String.IsNullOrEmpty(tbAddress.Text) || String.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu vào");
                return;
            }

            
            if (dieuKien.KiemTraMa("staff", "staff_id", tbStaff_id.Text) > 0)
            {
                MessageBox.Show("Mã nhân viên đã tồn tại");
                MessageBox.Show("Hãy nhập mã khác");
                return;
            }
            
            if (dieuKien.KiemTraMa("designation", "designation_id", tbDesignation_id.Text) == 0)
            {
                MessageBox.Show("Mã chức vụ ko tồn tại");
                return;
            }
            else
            {
                
                string sqlThem = "INSERT INTO staff " +
                                "VALUES (@staff_id, @name, @gender, @designation_id, @address, @phone)";
                SqlCommand cmd = new SqlCommand(sqlThem, con);
                //cmd.Parameters.AddWithValue("staff_id", tbStaff_id.Text);
                //cmd.Parameters.AddWithValue("name", tbName.Text);
                //cmd.Parameters.AddWithValue("gender", tbGender.Text);
                //cmd.Parameters.AddWithValue("designation_id", tbDesignation_id.Text);
                //cmd.Parameters.AddWithValue("address", tbAddress.Text);
                //cmd.Parameters.AddWithValue("phone", tbPhone.Text);

                //Mã Hoá
                MD5_algorithm md5 = new MD5_algorithm();
                cmd.Parameters.AddWithValue("staff_id", tbStaff_id.Text);
                cmd.Parameters.AddWithValue("name", md5.MaHoaString((tbName.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("gender", md5.MaHoaString((tbGender.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("designation_id", md5.MaHoaString((tbDesignation_id.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("address", md5.MaHoaString((tbAddress.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("phone", md5.MaHoaString((tbPhone.Text), "NguyenKhaDang"));
                cmd.ExecuteNonQuery();
                HienThi();
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbStaff_id.Text) || String.IsNullOrEmpty(tbName.Text) || String.IsNullOrEmpty(tbGender.Text) || String.IsNullOrEmpty(tbDesignation_id.Text) || String.IsNullOrEmpty(tbAddress.Text) || String.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu vào");
                return;
            }

            
            if (dieuKien.KiemTraMa("designation", "designation_id", tbDesignation_id.Text) == 0)
            {
                MessageBox.Show("Mã chức vụ tồn tại");
                return;
            }

            else
            {
                string sqlThem = "update staff " +
                                    "set staff_id=@staff_id, name=@name, gender=@gender, designation_id=@designation_id, address=@address, phone=@phone " +
                                    "where staff_id=@staff_id";
                SqlCommand cmd = new SqlCommand(sqlThem, con);
                MD5_algorithm md5 = new MD5_algorithm();
                cmd.Parameters.AddWithValue("staff_id", tbStaff_id.Text);
                cmd.Parameters.AddWithValue("name", md5.MaHoaString((tbName.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("gender", md5.MaHoaString((tbGender.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("designation_id", md5.MaHoaString((tbDesignation_id.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("address", md5.MaHoaString((tbAddress.Text), "NguyenKhaDang"));
                cmd.Parameters.AddWithValue("phone", md5.MaHoaString((tbPhone.Text), "NguyenKhaDang"));
                cmd.ExecuteNonQuery();
                HienThi();
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbStaff_id.Text))
            {
                MessageBox.Show("Bạn phải nhập mã nhân viên vào");
                return;
            }

            string sqlThem = "delete from staff " +
                                "where staff_id=@staff_id";
            SqlCommand cmd = new SqlCommand(sqlThem, con);
            cmd.Parameters.AddWithValue("staff_id", tbStaff_id.Text);
            cmd.Parameters.AddWithValue("name", tbName.Text);
            cmd.Parameters.AddWithValue("gender", tbGender.Text);
            cmd.Parameters.AddWithValue("designation_id", tbDesignation_id.Text);
            cmd.Parameters.AddWithValue("address", tbAddress.Text);
            cmd.Parameters.AddWithValue("phone", tbPhone.Text);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        private void btTimKiem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbNoiDungTimKiem.Text))
            {
                MessageBox.Show("Bạn phải nhập dữ liệu cần tìm vào");
                return;
            }
            (dataView.DataSource as DataTable).DefaultView.RowFilter = String.Format("staff_id like '%" + tbNoiDungTimKiem.Text + "%' or name like '%" + tbNoiDungTimKiem.Text + "%' or gender like '%" + tbNoiDungTimKiem.Text + "%' or designation_id like '%" + tbNoiDungTimKiem.Text + "%' or address like '%" + tbNoiDungTimKiem.Text + "%' or phone like '%" + tbNoiDungTimKiem.Text + "%'");
        }
        private void btExport_Click(object sender, EventArgs e)
        {
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program  
            app.Visible = true;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            worksheet.Name = "staff";
            // storing header part in Excel  
            for (int i = 1; i < dataView.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataView.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  
            for (int i = 0; i < dataView.Rows.Count; i++)
            {
                for (int j = 0; j < dataView.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataView.Rows[i].Cells[j].Value.ToString();
                }
            }
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            this.tbStaff_id.Clear();
            this.tbName.Clear();
            this.tbGender.Clear();
            this.tbDesignation_id.Refresh();
            this.tbAddress.Clear();
            this.tbPhone.Clear();
            this.tbNoiDungTimKiem.Clear();
            HienThi();
        }

        
    }
}
