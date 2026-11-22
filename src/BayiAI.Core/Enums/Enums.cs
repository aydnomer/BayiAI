namespace BayiAI.Core.Enums;

public enum KullaniciRolu : byte { Admin = 1, Satis = 2, Depo = 3, Muhasebe = 4 }

public enum SiparisDurumu : byte { Beklemede = 1, Onaylandi = 2, SevkEdildi = 3, TeslimEdildi = 4, Iptal = 5 }

public enum StokHareketTipi : byte { Giris = 1, Cikis = 2, Sayim = 3 }

public enum KasaBankaTipi : byte { Kasa = 1, Banka = 2 }

public enum OdemeTipi : byte { Nakit = 1, Havale = 2, Cek = 3, KrediKarti = 4 }

public enum CariIslemTipi : byte { Fatura = 1, Tahsilat = 2, Iade = 3, Duzeltme = 4 }

public enum AiIslemTipi : byte { SiparisTahmini = 1, RiskYorumu = 2, DogalDilSorgu = 3 }

// Programdaki her işlemin bir "yetki" adı var. Roller bu yetkilerle eşleştirilecek.
// enum: Sabit seçeneklerden oluşan liste. byte: Veritabanında 1 baytlık sayı olarak saklanır.
// BayiGoruntule = 1 dedik, sonrakiler otomatik 2, 3, 4... diye numaralanır.
public enum Yetki : byte
{
    BayiGoruntule = 1,   // Bayi listesini görme
    BayiDuzenle,         // Bayi ekleme, düzenleme, silme
    UrunGoruntule,       // Ürün listesini görme
    UrunDuzenle,         // Ürün ekleme, düzenleme, fiyat değiştirme
    StokIslem,           // Stok girişi, çıkışı, sayım
    SiparisGir,          // Yeni sipariş oluşturma
    SiparisOnayla,       // Siparişi onaylama veya iptal etme
    Sevkiyat,            // İrsaliye kesme, malı yola çıkarma
    Fatura,              // Fatura kesme
    Tahsilat,            // Bayiden para alma (tahsilat girişi)
    CariGoruntule,       // Bayinin borç/alacak hesabını görme
    Raporlar,            // Raporlar ekranı
    KullaniciYonetimi,   // Kullanıcı ekleme, şifre sıfırlama
    Ayarlar,             // Firma bilgileri, numara serileri
    LogGoruntule,        // Kim ne yaptı kayıtlarını görme
    YapayZeka            // Yapay zeka özellikleri
}