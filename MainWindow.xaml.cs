using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace yazartaspit
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiUrl = "http://127.0.0.1:8000/tahmin";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnAnaliz_Click(object sender, RoutedEventArgs e)
        {
            string metin = TxtMetin.Text.Trim();

            if (string.IsNullOrEmpty(metin) || metin.Length < 50)
            {
                MessageBox.Show("Lütfen analiz için en az 50 karakterlik anlamlı bir metin girin!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TxtDurum.Text = "Yapay zeka modeli metni analiz ediyor...";
            BtnAnaliz.IsEnabled = false;

            try
            {
                // JSON Verisini Hazırla
                var jsonPayload = new JObject(new JProperty("metin", metin)).ToString();
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // FastAPI'ye İstek At
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject data = JObject.Parse(jsonResponse);

                    // Sonuçları Arayüze Yansıt
                    TxtAnaTahmin.Text = data["en_guclu_tahmin"]?.ToString();

                    // Alternatif Adayları Listele
                    LstAlternatifler.Items.Clear();
                    var adaylar = data["diger_adaylar"];
                    if (adaylar != null)
                    {
                        foreach (var aday in adaylar)
                        {
                            LstAlternatifler.Items.Add($"Yazar ID: {aday["yazar_id"]}");
                        }
                    }

                    // Üslup İstatistikleri
                    var profil = data["uslup_profili"];
                    if (profil != null)
                    {
                        TxtCumleUzunlugu.Text = $"• Ortalama Cümle Uzunluğu: {profil["ort_cumle_uzunlugu"]} kelime";
                        TxtKelimeZenginligi.Text = $"• Kelime Zenginliği: {profil["kelime_zenginligi"]}";
                        TxtNoktalama.Text = $"• Noktalama Yoğunluğu: {profil["noktalama_yogunlugu"]}";
                    }

                    TxtDurum.Text = "Analiz başarıyla tamamlandı.";
                }
                else
                {
                    string hataDetayi = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Sunucu Hatası: {hataDetayi}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtDurum.Text = "Analiz sırasında hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Python API sunucusuna bağlanılamadı!\n\nLütfen arka planda 'api.py' dosyasının çalıştığından emin olun.\n\nHata: {ex.Message}", "Bağlantı Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtDurum.Text = "Bağlantı hatası!";
            }
            finally
            {
                BtnAnaliz.IsEnabled = true;
            }
        }
    }
}