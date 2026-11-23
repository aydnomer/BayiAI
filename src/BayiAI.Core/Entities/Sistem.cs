using BayiAI.Core.Enums;

namespace BayiAI.Core.Entities;

// Kim, ne zaman, hangi kayıtta ne yaptı? (Değişmedi, aynen duruyor.)
public class IslemLog : BaseEntity
{
    public int? KullaniciId { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;
    public string TabloAdi { get; set; } = "";
    public int KayitId { get; set; }
    public string Islem { get; set; } = "";   // Ekle / Guncelle / Sil
    public string? EskiDeger { get; set; }
    public string? YeniDeger { get; set; }
}

// Yapay zekaya yapılan istekler. (Değişmedi.)
public class AiIslem : BaseEntity
{
    public int? KullaniciId { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;
    public AiIslemTipi IslemTipi { get; set; }
    public string Girdi { get; set; } = "";
    public string? Cikti { get; set; }
    public int? SureMs { get; set; }
}

// YENİ: Firmanın kendi bilgileri ve genel ayarlar. Tabloda sadece 1 satır olur (Id = 1).
// Fatura, irsaliye ve raporların üst kısmındaki firma bilgisi buradan gelir.
public class FirmaAyar : BaseEntity
{
    public string Unvan { get; set; } = "";        // Firma adı (zorunlu)
    public string? VergiNo { get; set; }           // ? → boş bırakılabilir
    public string? VergiDairesi { get; set; }
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }

    // byte[]: Bayt dizisi. Logo resmini doğrudan veritabanında saklarız (SQL'de varbinary).
    public byte[]? Logo { get; set; }

    // Yeni ürün eklenirken otomatik gelecek KDV oranı. decimal: Para ve oran için doğru tip.
    public decimal VarsayilanKdvOrani { get; set; } = 20;

    // Yeni bayi eklenirken otomatik gelecek vade günü.
    public int VarsayilanVadeGun { get; set; } = 30;

    // Stok kritik seviyenin altına inince uyarı gösterilsin mi?
    public bool KritikStokUyarisi { get; set; } = true;
}

// YENİ: Belge numarası üretici. Her belge türü için bir satır (SIP, FTR, IRS, THS, BAY).
// Numara biçimi: Önek + Yıl + Sıra no → FTR2026000001
public class NumaraSerisi : BaseEntity
{
    public string Kod { get; set; } = "";     // Belge türü: SIP, FTR, IRS, THS, BAY
    public string Onek { get; set; } = "";    // Numaranın başına gelen harfler
    public int Yil { get; set; }              // Hangi yılın serisi

    // En son verilen numara. private set: Sadece SonrakiNumara metodu değiştirebilir,
    // böylece kimse yanlışlıkla numarayı geri alamaz (kapsülleme).
    public int SonNumara { get; private set; }

    // Sıra numarası kaç basamak olsun? 6 → 000001
    public byte Uzunluk { get; set; } = 6;

    // Sıradaki numarayı üretir ve döndürür.
    public string SonrakiNumara(DateTime tarih)
    {
        // Yeni yıla girildiyse sayaç sıfırdan başlar (2027'nin ilk faturası FTR2027000001 olur).
        if (tarih.Year != Yil)
        {
            Yil = tarih.Year;
            SonNumara = 0;
        }

        // Numarayı 1 artır.
        SonNumara++;

        // PadLeft(6, '0'): Sayıyı soldan sıfırla 6 haneye tamamlar. 1 → "000001"
        return $"{Onek}{Yil}{SonNumara.ToString().PadLeft(Uzunluk, '0')}";
    }
}