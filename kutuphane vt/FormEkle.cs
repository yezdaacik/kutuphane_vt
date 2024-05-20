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
    public partial class FormEkle : Form
    {
        string baglanti = "Server=localhost;Database=kutuphane;Uid=root;Pwd=;";
        string yeniAd;
        public FormEkle()
        {
            InitializeComponent();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "INSERT INTO kitaplar  VALUES (NULL, @adi, @yazari, @sayfa_sayisi, @cilt, @basim_yili, @poster);";
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@adi", txtAdEkle.Text);
                cmd.Parameters.AddWithValue("@yazari", txtYazarEkle.Text);
                cmd.Parameters.AddWithValue("@sayfa_sayisi", txtSayfaEkle.Text);
                cmd.Parameters.AddWithValue("@cilt", cbCiltEkle.Checked);
                cmd.Parameters.AddWithValue("@basim_yili", txtYilEkle.Text);
                cmd.Parameters.AddWithValue("@poster", "");

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Kayıt Eklendi");
                }

            }
        }

        private void pbResimEkle_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResimEkle.Image = null;

            if (File.Exists(hedefDosya))
            {
                pbResimEkle.Image = Image.FromFile(hedefDosya);
                pbResimEkle.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
