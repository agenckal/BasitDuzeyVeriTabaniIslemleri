using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;//İŞLEMLER YAPABİLMEK İÇİN GEREKLİ NAMESPACELERİ EKLİYORUZ
using System.Data.SqlClient;//İŞLEMLER YAPABİLMEK İÇİN GEREKLİ NAMESPACELERİ EKLİYORUZ
/*BEN SQL EKLEDİM ÇÜNKÜ SQL SERVER VERİTABANINDA İŞLEM YAPACAĞIM.EĞER SİZ ACCESS VERİTABANINI KULLANACAKSANIZ
 *"SqlClient"YAZAN YERİ "OleDb" OLARAK DEĞİŞTİRECEKSİNİZ 
 *
 * GENEL AÇIKLAMA : EĞER ACCESS VERİTABANINDA İŞLEM YAPMAK İSTİYORSANIZ 
 * BURADA "Sql" YAZAN YERLERİ "OleDb" OLARAK DEĞİŞTİRİNCE İSTEĞİNİZ YERİNE GELİR.
 * 
 */
namespace VeriTabaniIslemleri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //BAĞLANTI İÇİN YUKARIDA TANIMLADIĞIMIZ NAMESPACEDEN BİR NESNE TÜRETİYORUZ.
        SqlConnection baglanti = new SqlConnection(); 
        //VERİLERİ LİSTELEMEK İÇİN BİR PROSEDÜR YAZIYORUZ
        void DATAGRIDVIEW_YENILE() 
        {
            //BÜTÜN VERİLERİ LİSTELEME
            string sorguStr = "SELECT * FROM Tablomuz";//VERİLERİ ÇEKMEK İÇİN "SELECT * " CÜMLESİNİ KULLANIYORUM
            SqlCommand sorgu = new SqlCommand(sorguStr, baglanti);//Tablodaki verileri çekiyorum
            SqlDataAdapter dp = new SqlDataAdapter(sorgu);//VERİLERİ DATAADAPTER AKTARIYORUM
            DataTable dTablo = new DataTable();
            //DATAADAPTER'E  AKTARMIŞ OLDUĞUM VERİLERİ DATATABLE'LA DOLDURUYORUM
            dp.Fill(dTablo);
            //DATATABLEYIDA DATAGRİDVİEWİN VERİ KAYNAĞI OLARAK GÖSTERİP VERİLERİ DATAGRİDVİEWDE LİSTELEMİŞ OLUYORUM
            dataGridView1.DataSource = dTablo;
            //YUKARIDA AÇMIŞ OLDUĞUM BAĞLANTIYI KAPATIYORUZ.
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //TÜRETTİĞİMİZ NESNE HANGİ VERİTABANI ÜZERİNDE İŞLEM YAPACAKSA O VERİTABANININ BAĞLANTI CÜMLESİNİ YAZIYORUZ
            //BENİM YAPTIĞIM SQL SERVER OLDUĞU İÇİN VE VERİTABANI PROJENİN DİZİNİNDE OLDUĞU İÇİN AŞAĞIDAKİ BAĞLANTI CÜMLESİNİ YAZDIM
            //ACCESS İÇİN "Provider=Microsoft.Jet.Oledb.4.0;Data Source=VERİTABANININ YOLU" YAZMANIZ YETERLİ OLACAKTIR.
            baglanti.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\KisilerVt.mdf;Integrated Security=True;User Instance=True";
            //SAĞLAMIŞ OLDUĞUM BAĞLANTIYI AÇIYORUM
            baglanti.Open();
            //VERİLERİ LİSTELEMEK İÇİN PROSEDÜRÜMÜZÜ ÇAĞIRIYORUZ
            DATAGRIDVIEW_YENILE();
            baglanti.Close();
        }
        //VERİ KAYDETME
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //VERİTABANINDA İŞLEM YAPABİLMEK İÇİN BAĞLANTIMIZI AÇIYORUZ
                baglanti.Open();
                //TEXTBOXLARI KONTROL EDİYORUZ
                if (txtAd.Text == "" || txtNumara.Text == "" || txtSoyad.Text == "")
                {
                    MessageBox.Show("Boş alan bırakmayınız.");
                    txtAd.Focus();
                }
                else
                {
                    //VERİ EKLEMEK İÇİN GEREKLİ TSQL KOMUTUNU PARAMETRELİ ŞEKİLDE YAZIYORUZ
                    //PARAMETRE KULLANMA AMACIMIZ VERİTABANI İŞLEMLERİNDE GÜVENLİĞİ ÜST DÜZEYE CIKARMAKTIR.
                    string EkleStr = "INSERT INTO Tablomuz (Ad,SoyAd,Numara)VALUES (@Ad,@Soyad,@Numara)";
                    //YAZMIŞ OLDUĞUMUZ VERİ EKLEME KOMUTUNU NESNEMİZE TANITIYORUZ
                    SqlCommand sorgu = new SqlCommand(EkleStr, baglanti);
                    //PARAMETRELERİMİZE VERİ TİPLERİNE GÖRE DEĞER ATIYORUZ
                    sorgu.Parameters.Add("@Ad", SqlDbType.NVarChar).Value = txtAd.Text;
                    sorgu.Parameters.Add("@SoyAd", SqlDbType.NVarChar).Value = txtSoyad.Text;
                    sorgu.Parameters.Add("@Numara", SqlDbType.Int).Value = txtNumara.Text;
                   //SORGUUMUZU ÇALIŞTIRIYORUZ
                    //SORGU GERÇEKLEŞMEZ İSE KULLANICIYA HATA MSJ VERDİRİYORUZ
                    if (sorgu.ExecuteNonQuery()<0)
                    {
                        MessageBox.Show("Bir Hata oluştu.");
                    }
                    //GERÇEKLEŞİRSE BİLGİ MSJI VERİP,YAPTIĞIMIZ PROSEDÜR VASITASIYLA DATAGİRDVİEWİMİZİ YENİLİYORUZ
                    else
                    {
                        MessageBox.Show("Kayıt başarı ile ekledi.");
                        DATAGRIDVIEW_YENILE();
                    }
                }
            }
            catch (Exception hata)
            {

                MessageBox.Show("Hata : " + hata.Message.ToString());
            }
            finally 
            {
                //YUKARIDA AÇMIŞ OLDUĞUM BAĞLANTIYI KAPATIYORUZ.
                baglanti.Close();
            }

        }
        //GÜNCELLENECEK KAYITI SEÇİYORUZ
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //TEXTBOXLARI KONTROL EDİYORUZ
                if (txtNo.Text == "")
                {
                    MessageBox.Show("Devam etmek için kayıt No'su giriniz.");
                }
                else
                {
                    //BAĞLANTIMIZI AÇIYORUZ
                    baglanti.Open();
                    //SEÇME SORGUMUZU NO ALANINA GÖRE OLUŞTURUYORUZ
                    string sorgu = "select * from Tablomuz where No='" + txtNo.Text + "' ";
                    //SORGUMUZU ÇALIŞTIRIYORUZ
                    SqlDataAdapter sqldAtap = new SqlDataAdapter(sorgu, baglanti);
                    //GELEN VERİLERİ DATASETE ATIYORUZ
                    DataSet ds = new DataSet();
                    //DATASETTEN TABLOMUZU SEÇİP DATAADAPTÖRE DOLDURUYORUZ
                    sqldAtap.Fill(ds, "Tablomuz");
                    //VERİLERİ TEXTBOXLARA DOLDURUYORUZ
                    textBox1.Text = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                    textBox2.Text = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                    textBox3.Text = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                }
            }
            catch (Exception hata)
            {

                MessageBox.Show("Hata : " + hata.Message.ToString());
            }
            finally
            {
                //YUKARIDA AÇMIŞ OLDUĞUM BAĞLANTIYI KAPATIYORUZ.
                baglanti.Close();
            }
        }
        //SEÇTİĞİMİZ KAYITI GÜNCELLİYORUZ
        private void button2_Click(object sender, EventArgs e)
        {
            try
            { 
                //BAĞLANTIMIZI AÇIYORUZ
                baglanti.Open();
                //TEXTBOXLARI KONTROL EDİYORUZ
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                {
                    MessageBox.Show("Hata.Kayıt Seçiniz. ");
                }
                else
                {                    
                    //SEÇTİĞİMİZ VERİYİ DEĞİŞTİRMEK İÇİN İÇİN GEREKLİ TSQL KOMUTUNU PARAMETRELİ ŞEKİLDE YAZIYORUZ
                    //PARAMETRE KULLANMA AMACIMIZ VERİTABANI İŞLEMLERİNDE GÜVENLİĞİ ÜST DÜZEYE CIKARMAKTIR.
                  
                    string degistirStr = "UPDATE Tablomuz SET Ad=@Ad,SoyAd=@SoyAd,Numara=@Numara WHERE No=@No";
                    //YAZMIŞ OLDUĞUMUZ VERİ GÜNCELLEME KOMUTUNU NESNEMİZE TANITIYORUZ
                    SqlCommand sorgu = new SqlCommand(degistirStr, baglanti);
                    //PARAMETRELERİMİZE VERİ TİPLERİNE GÖRE DEĞER ATIYORUZ
                    sorgu.Parameters.Add("@Ad", SqlDbType.NVarChar).Value = textBox1.Text;
                    sorgu.Parameters.Add("@SoyAd", SqlDbType.NVarChar).Value = textBox2.Text;
                    sorgu.Parameters.Add("@Numara", SqlDbType.Int).Value = textBox3.Text;
                    sorgu.Parameters.Add("@No", SqlDbType.Int).Value = txtNo.Text;
                    //SORGUUMUZU ÇALIŞTIRIYORUZ
                    //SORGU GERÇEKLEŞMEZ İSE KULLANICIYA HATA MSJ VERDİRİYORUZ
                    if (sorgu.ExecuteNonQuery() < 0)
                    {
                        MessageBox.Show("Bir Hata oluştu.");
                    }
                    //GERÇEKLEŞİRSE BİLGİ MSJI VERİP,YAPTIĞIMIZ PROSEDÜR VASITASIYLA DATAGİRDVİEWİMİZİ YENİLİYORUZ
                    else
                    {
                        MessageBox.Show("Kayıt başarı değiştirildi.");
                        DATAGRIDVIEW_YENILE();
                    }
                }
            }
            catch (Exception hata)
            {

                MessageBox.Show("Hata : " + hata.Message.ToString());
            }
            finally
            {
                //BAĞLANTIMIZI KAPATIYORUZ
                baglanti.Close();
            }
                        
        }
        //VERİ SİLME
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //BAĞLANTIMIZI AÇIYORUZ
                baglanti.Open();
                //TEXTBOXI KONTROL EDİYORUZ
                if (textBox4.Text == "")
                {
                    MessageBox.Show("Kayıt silmek için no giriniz.");
                }
                else
                {
                    //SEÇTİĞİMİZ VERİYİ SİLMEK İÇİN İÇİN GEREKLİ TSQL KOMUTUNU PARAMETRELİ ŞEKİLDE YAZIYORUZ
                    //PARAMETRE KULLANMA AMACIMIZ VERİTABANI İŞLEMLERİNDE GÜVENLİĞİ ÜST DÜZEYE CIKARMAKTIR.
                    string silmeStr = "DELETE FROM Tablomuz WHERE No=@No";

                    SqlCommand sorgu = new SqlCommand(silmeStr,baglanti);
                    sorgu.Parameters.Add("@No", SqlDbType.Int).Value = textBox4.Text;
                   //SORGUUMUZU ÇALIŞTIRIYORUZ
                   //SORGU GERÇEKLEŞMEZ İSE KULLANICIYA HATA MSJ VERDİRİYORUZ
                   if (sorgu.ExecuteNonQuery()<0)
                    {
                        MessageBox.Show("Bir Hata oluştu.");
                    }
                   //GERÇEKLEŞİRSE BİLGİ MSJI VERİP,YAPTIĞIMIZ PROSEDÜR VASITASIYLA DATAGİRDVİEWİMİZİ YENİLİYORUZ
                   else
                   {
                       MessageBox.Show("Kayıt başarı silindi.");
                       DATAGRIDVIEW_YENILE();
                   }
                    
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata : " + hata.Message.ToString());
            }
            finally 
            {
                //BAĞLANTIMIZI KAPATIYORUZ
                baglanti.Close();
            } 

        }
    }
}
