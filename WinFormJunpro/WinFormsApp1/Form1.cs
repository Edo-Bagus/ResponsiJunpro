using Npgsql;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        string connString = "Host=localhost;Port=5432;Username=postgres;Password=informatika;Database=ResponsiJunpro";
        List<Departemen> departemen = new List<Departemen>();
        List<Jabatan> jabatan = new List<Jabatan>();
        List<Karyawan> karyawan = new List<Karyawan>();
        NpgsqlConnection conn;
        NpgsqlCommand cmd;
        DataTable dt;
        DataGridViewRow r;
        string sql = null;

        public Form1()
        {
            InitializeComponent();
            conn = new NpgsqlConnection(connString);
            LoadDepartemen();
            LoadKaryawan();
            LoadJabatan();
        }

        private void LoadDepartemen()
        {
            try
            {
                sql = "SELECT * FROM departemen";
                conn.Open();
                comboDep.DataSource = null;
                departemen.Clear();
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    var dept = new Departemen
                    {
                        Id_Dep = (int)dr[0],
                        Nama_Dep = (string)dr[1]
                    };
                    comboDep.Items.Add(dept.Id_Dep);
                    string listStr = dept.Id_Dep.ToString() + ": " + dept.Nama_Dep.ToString();
                    listBox1.Items.Add(listStr);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LoadJabatan()
        {
            try
            {
                sql = "SELECT * FROM jabatan";
                conn.Open();
                comboJabatan.DataSource = null;
                jabatan.Clear();
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    var jab = new Jabatan
                    {
                        Id_Jabatan = (int)dr[0],
                        Nama_Jabatan = (string)dr[1],
                        Salary = (decimal)dr[2]
                    };
                    comboJabatan.Items.Add(jab.Id_Jabatan);
                    string listStr = jab.Id_Jabatan.ToString() + ": " + jab.Nama_Jabatan.ToString()+ ", salary: " + jab.Salary.ToString();
                    listBox2.Items.Add(listStr);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LoadKaryawan()
        {
            try
            {
                sql = "SELECT * FROM karyawan";
                conn.Open();
                dataGridView1.DataSource = null;
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                conn.Close();
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("SETSGE");
            Debug.WriteLine(e.RowIndex);
            if (e.RowIndex >= 0)
            {
                r = dataGridView1.Rows[e.RowIndex];
                txtNama.Text = r.Cells["nama"].Value.ToString();
                comboDep.SelectedItem = r.Cells["id_dep"].Value;
                comboJabatan.SelectedItem = r.Cells["id_jabatan"].Value;

            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"SELECT * FROM insert_karyawan(:_name,:_id_dep,:_id_jabatan)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_name", txtNama.Text);
                cmd.Parameters.AddWithValue("_id_dep", comboDep.SelectedItem);
                cmd.Parameters.AddWithValue("_id_jabatan", comboJabatan.SelectedItem);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Data Users Berhasil diinputkan", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    LoadKaryawan();
                    txtNama.Text = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Insert FAIL !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon Pilih Baris yang akan dihapus", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Apakah benar anda ingin menghapus data " + r.Cells["nama"].Value.ToString() + " ?",
                "Hapus data terkonfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                try
                {
                    conn.Open();
                    sql = @"DELETE FROM karyawan WHERE id_karyawan = :_id_karyawan";
                    Debug.WriteLine(r.Cells["id_karyawan"].Value.ToString());
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("_id_karyawan", r.Cells["id_karyawan"].Value.ToString());
                    cmd.ExecuteScalar();
                    MessageBox.Show("Data Users Berhasil dihapus", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    LoadKaryawan();
                    txtNama.Text = null;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message, "Delete FAIL !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon Pilih Baris yang akan diupdate", "Good!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                conn.Open();
                sql = @"SELECT * FROM update_karyawan(:_id,:_nama,:_id_dep,:_id_jabatan)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_id", r.Cells["id_karyawan"].Value.ToString());
                cmd.Parameters.AddWithValue("_nama", txtNama.Text);
                cmd.Parameters.AddWithValue("_id_dep", comboDep.SelectedItem);
                cmd.Parameters.AddWithValue("_id_jabatan", comboJabatan.SelectedItem);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    conn.Close();
                    MessageBox.Show("Data Users Berhasil diupdate", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadKaryawan();
                    txtNama.Text = null;
                    r = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Insert FAIL !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
