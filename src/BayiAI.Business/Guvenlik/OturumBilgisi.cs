// Kullanici sınıfı Entities'te, KullaniciRolu ve Yetki Enums'ta duruyor.
using BayiAI.Core.Entities;
using BayiAI.Core.Enums;

namespace BayiAI.Business.Guvenlik;

// Programı ŞU AN kullanan kişinin bilgilerini tutar.
// Program boyunca tek bir tane olacak (buna "singleton" denir).
// "Bu kaydı kim yaptı?", "Bu kişi bu ekranı açabilir mi?" sorularının cevabı buradan gelir.
public class OturumBilgisi
{
    // Giriş yapan kullanıcının Id'si. int? : Kimse giriş yapmadıysa null.
    // private set: Sadece bu sınıfın metotları değiştirebilir (kapsülleme).
    public int? KullaniciId { get; private set; }

    // Ekranın üstünde "Hoş geldin, Ömer Faruk" yazdırmak için.
    public string? AdSoyad { get; private set; }

    // Giriş yapan kişinin rolü.
    public KullaniciRolu? Rol { get; private set; }

    // Ne zaman giriş yaptı.
    public DateTime? GirisZamani { get; private set; }

    // Kullanıcı Id'si doluysa biri giriş yapmış demektir.
    // Bu bir "hesaplanan özellik": değer saklamaz, her sorulduğunda hesaplanır.
    public bool GirisYapildiMi => KullaniciId.HasValue;

    // Giriş başarılı olunca çağrılır: kullanıcının bilgilerini oturuma kopyalar.
    public void Baslat(Kullanici kullanici, DateTime zaman)
    {
        KullaniciId = kullanici.Id;
        AdSoyad = kullanici.AdSoyad;
        Rol = kullanici.Rol;
        GirisZamani = zaman;
    }

    // Çıkış yapılınca çağrılır: her şeyi temizler.
    public void Kapat()
    {
        KullaniciId = null;
        AdSoyad = null;
        Rol = null;
        GirisZamani = null;
    }

    // Şu anki kullanıcının bu işleme yetkisi var mı?
    // Rol boşsa (giriş yoksa) false. Doluysa YetkiMatrisi'ne sorar.
    // Rol.Value: Nullable bir değişkenin içindeki asıl değeri alır.
    public bool YetkiVarMi(Yetki yetki) => Rol.HasValue && YetkiMatrisi.YetkiliMi(Rol.Value, yetki);
}
