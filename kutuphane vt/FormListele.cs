using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kutuphane_vt
{
    public partial class FormListele : Form
    {
        string baglanti = "Server=localhost;Database=kutuphane;Uid=root;Pwd=;";
        string yeniAd = "";
        public FormListele()
        {
            InitializeComponent();
        }

        void DgwDoldur()
        {

            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT * FROM kitaplar;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                dgwKitaplar.DataSource = dt;

            }

        }

        private void FormListele_Load(object sender, EventArgs e)
        {
            DgwDoldur();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgwKitaplar.SelectedCells.Count > 0)
            {
                string sorgu = "UPDATE kitaplar SET adi=@adi, yazari = @yazari, sayfa_sayisi = @sayfa_sayisi, cilt=@cilt, basim_yili= @basim_yili, poster = @poster  WHERE id = @id;";
                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@adi", txtAd.Text);
                    cmd.Parameters.AddWithValue("@yazari", txtYazar.Text);
                    cmd.Parameters.AddWithValue("@sayfa_sayisi", txtSayfaSayisi.Text);
                    cmd.Parameters.AddWithValue("@cilt", cbCilt.Checked);
                    cmd.Parameters.AddWithValue("@basim_yili", txtYil.Text);
                    cmd.Parameters.AddWithValue("@poster", yeniAd);


                    int id = Convert.ToInt32(dgwKitaplar.SelectedRows[0].Cells["id"].Value);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();

                    DgwDoldur();
                }
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DataGridViewRow dr = dgwKitaplar.SelectedRows[0];

            int id = Convert.ToInt32(dr.Cells[0].Value);

            string posterYol = Path.Combine(Environment.CurrentDirectory, "poster", dgwKitaplar.SelectedRows[0].Cells["poster"].Value.ToString());


            DialogResult cevap = MessageBox.Show("kitabı silmek istediğinizden emin misiniz?",
                                                 "kitabı sil", MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);


            if (cevap == DialogResult.Yes)
            {

                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    int film_id = Convert.ToInt32(dgwKitaplar.SelectedRows[0].Cells["id"].Value);
                    baglan.Open();
                    string sorgu = "DELETE FROM kitaplar WHERE id=@id;";
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();


                    if (File.Exists(posterYol))
                    {

                        File.Delete(posterYol);
                    }
                    DgwDoldur();
                }
            }
        }

        private void dgwKitaplar_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwKitaplar.SelectedRows.Count > 0)
            {
                txtAd.Text = dgwKitaplar.SelectedRows[0].Cells["adi"].Value.ToString();
                txtYazar.Text = dgwKitaplar.SelectedRows[0].Cells["yazari"].Value.ToString();
                txtYil.Text = dgwKitaplar.SelectedRows[0].Cells["basim_yili"].Value.ToString();
                txtSayfaSayisi.Text = dgwKitaplar.SelectedRows[0].Cells["sayfa_sayisi"].Value.ToString();
                cbCilt.Checked = Convert.ToBoolean(dgwKitaplar.SelectedRows[0].Cells["cilt"].Value);

                string dosyaYolu = Path.Combine(Environment.CurrentDirectory, "poster", dgwKitaplar.SelectedRows[0].Cells["cilt"].Value.ToString());

                pbResim.ImageLocation = null;

                if (File.Exists(dosyaYolu))
                {
                    pbResim.ImageLocation = dosyaYolu;
                    pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            }
        }

        private void pbResim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResim.Image = null;

            if (File.Exists(hedefDosya))
            {
                pbResim.Image = Image.FromFile(hedefDosya);
                pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
