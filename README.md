# BayiAI

**Yapay Zeka Destekli Bayi Yönetim ve Ön Muhasebe Otomasyonu**

BayiAI, ürünlerini bayiler üzerinden satan üretici ve toptancı firmalar için bir masaüstü uygulamasıdır. Firma; bayilerini, bayilerden gelen siparişleri, stoğunu, faturalarını ve bayilerden olan alacağını tek bir programdan yönetir. Yapay zeka, bayinin bir sonraki siparişini tahmin eder, bayinin ödeme riskini yorumlar ve doğal dille yazılan soruları rapora çevirir.

> Nesne Tabanlı Programlama dersi dönem projesi.

## Özellikler

- **Bayi yönetimi:** Bölge, yetkili kişi, bayiye özel iskonto oranı ve risk (kredi) limiti
- **Sipariş akışı:** Beklemede → Onaylandı → Sevk edildi → Teslim edildi
- **Ürün ve stok:** Kategori ağacı, kritik stok uyarısı, her stok hareketinin kaydı
- **Ön muhasebe:** Fatura, tahsilat, kasa/banka, bayi cari hesabı (borç/alacak), vadesi geçmiş alacaklar
- **Risk kontrolü:** Risk limitini aşan bayinin siparişi sevk edilmez
- **Raporlar:** Fatura, cari ekstre, satış grafikleri (DevExpress XtraReports / ChartControl)
- **Yapay zeka:**
  - Sipariş tahmini (bayinin geçmiş siparişlerine göre)
  - Bayi risk yorumu (bakiye, gecikmiş faturalar, ödeme geçmişi)
  - Doğal dille sorgu ("bu ay en çok borcu olan 5 bayi")

## Kullanılan teknolojiler

| Alan | Teknoloji |
| --- | --- |
| Dil / IDE | C#, .NET 10 (LTS), Visual Studio 2026 |
| Arayüz | Windows Forms + DevExpress |
| Veritabanı | SQL Server LocalDB |
| Veri erişimi | Entity Framework Core |
| Yapay zeka | Gemini / Claude API (HttpClient) veya yerel Ollama |
| Sürüm kontrolü | Git + GitHub |

## Mimari

Katmanlı yapı, her katman ayrı bir proje:

```
BayiAI.slnx
├── src/BayiAI.Core       Entity, enum, interface, exception (hiçbir projeye bağlı değil)
├── src/BayiAI.Data       DbContext, repository, migration (Core'a bağlı)
├── src/BayiAI.Business   Servisler, doğrulama, iş kuralları, yapay zeka (Core'a bağlı)
└── src/BayiAI.UI         DevExpress WinForms arayüzü
```

Veritabanı 15 tablodan oluşur. Tablo yapısı ve ilişkiler için `database/01_tablolar.sql` dosyasına bakın.

## Kurulum ve çalıştırma

1. Depoyu klonlayın:
   ```
   git clone https://github.com/<kullanici-adi>/BayiAI.git
   ```
2. Visual Studio 2026'da `BayiAI.slnx` dosyasını açın.
3. DevExpress'in kurulu olduğundan emin olun (30 günlük deneme sürümü yeterlidir).
4. `src/BayiAI.UI/appsettings.example.json` dosyasını aynı klasöre `appsettings.json` adıyla kopyalayın ve kendi bağlantı dizenizi ve yapay zeka API anahtarınızı yazın.
5. Package Manager Console'da (Tools → NuGet Package Manager → Package Manager Console) veritabanını oluşturun:
   ```
   Add-Migration IlkKurulum -Project BayiAI.Data -StartupProject BayiAI.Data
   Update-Database -Project BayiAI.Data -StartupProject BayiAI.Data
   ```
6. İsterseniz `database/02_ornek_veri.sql` ile örnek veri yükleyin.
7. `BayiAI.UI` projesini başlangıç projesi yapıp çalıştırın.

> `appsettings.json` dosyası `.gitignore` içindedir. Bağlantı dizesi ve API anahtarı depoya yüklenmez.

## Lisans

MIT
