# Metin Yazarlığı ve Üslup Analiz Sistemi

Girilen bir Türkçe metnin dilsel özelliklerini ve üslup profilini analiz ederek
en olası yazarı tahmin eden bir makine öğrenmesi projesi.

## Problem

Türkçe çok türlü (multi-genre) bir metin koleksiyonundan yola çıkarak, daha
Üslup imzasından yola çıkarak önce görülmemiş bir metin parçasının hangi yazara ait olduğunu tahmin etmek

## Veri İşleme

- Ham veri, ZIP içinde XML formatında dağıtılmış makalelerden oluşuyor.
  Dosyalar diske çıkarılmadan doğrudan ZIP içinden okunuyor.
- Her XML'den yazar adı, tür (genre) ve makale metni çıkarılıyor.
- Metinlerden beş sayısal üslup özelliği hesaplanıyor: ortalama cümle
  uzunluğu, virgül/ünlem/soru işareti oranı ve kelime zenginliği.
- İşlenmiş veri tek bir CSV dosyasında toplanıyor.

## Yöntem

- **Karakter n-gram (2-5)** ve **kelime n-gram (1-2)** özellikleri birlikte
  kullanıldı. Karakter düzeyindeki özellikler, yazarın konudan bağımsız
  üslup imzasını (ek kullanımı, noktalama alışkanlığı, cümle yapısı)
  yakalamada belirleyici oldu.
- Bunlara metinden çıkarılan beş sayısal üslup özelliği eklendi.
- Sınıflandırıcı olarak **LinearSVC** kullanıldı; seyrek ve yüksek boyutlu
  metin verisinde Random Forest gibi ağaç tabanlı yöntemlere göre belirgin
  şekilde daha iyi sonuç verdi.
- Toplam ~80.000 metinlik veri setinden en çok metne sahip 50 yazar seçildi
  ve her yazardan eşit sayıda örnek alınarak veri dengelendi.
- Eğitim/test ayrımı, özellik çıkarımından (TF-IDF fit) önce yapıldı; bu
  sayede test verisi eğitime sızmadı.

## Sonuçlar

| Metrik | Değer |
|---|---|
| Yazar sayısı | 50 |
| Doğruluk (top-1) | %69.85 |
| Doğruluk (top-3) | %82.77 |
| Rastgele tahmin (referans) | %2 |

Model, rastgele tahminin yaklaşık 35 katı başarı gösteriyor. Top-3 doğruluğun
top-1'e göre belirgin şekilde yüksek olması, modelin yanlış tahmin ettiği
durumlarda bile doğru yazarı genelde en olası adaylar arasında tuttuğunu
gösteriyor.

Ayrıca modelin en çok karıştırdığı yazar çiftleri ("üslup ikizleri") ayrı
olarak analiz edildi; bu çiftler genellikle birbirine yakın yazım
alışkanlıklarına sahip yazarlardan oluşuyor.

## Kullanılan Teknolojiler

- Python
- scikit-learn (TF-IDF, LinearSVC)
- pandas / numpy
- Streamlit (demo arayüzü)

## Sınırlılıklar

- Sonuçlar 50 yazarla sınırlı bir veri setine dayanıyor; yazar sayısı
  arttıkça doğruluğun düşmesi beklenir.
- Kısa metinlerde (~50 kelimeden az) tahmin güvenilirliği azalır.
- Bu bir adli bilişim veya kimlik doğrulama aracı değildir; bir öğrenme
  ve portföy projesidir.
