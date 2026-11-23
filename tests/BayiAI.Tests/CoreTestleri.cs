using BayiAI.Business.Guvenlik;
using BayiAI.Core.Entities;
using BayiAI.Core.Enums;

namespace BayiAI.Tests;

// Bir dosyada birden fazla test sınıfı olabilir.
public class NumaraSerisiTestleri
{
    [Fact]
    public void SiradakiNumara_DogruBicimdeUretilir()
    {
        // Hazırla
        var seri = new NumaraSerisi { Kod = "FTR", Onek = "FTR", Yil = 2026, Uzunluk = 6 };

        // Yap + Kontrol et: İlk numara 000001, ikincisi 000002 olmalı.
        Assert.Equal("FTR2026000001", seri.SonrakiNumara(new DateTime(2026, 11, 22)));
        Assert.Equal("FTR2026000002", seri.SonrakiNumara(new DateTime(2026, 11, 22)));
    }

    [Fact]
    public void YilDegisince_SayacSifirlanir()
    {
        var seri = new NumaraSerisi { Kod = "SIP", Onek = "SIP", Yil = 2026, Uzunluk = 6 };
        seri.SonrakiNumara(new DateTime(2026, 12, 31));   // 2026'nın son siparişi

        // 1 Ocak 2027'de sayaç baştan başlamalı.
        Assert.Equal("SIP2027000001", seri.SonrakiNumara(new DateTime(2027, 1, 1)));
    }
}

public class KullaniciKilitTestleri
{
    [Fact]
    public void BesHataliDeneme_HesabiKilitler()
    {
        var k = new Kullanici();
        var simdi = new DateTime(2026, 11, 22, 10, 0, 0);

        // 5 kez hatalı giriş.
        for (int i = 0; i < Kullanici.MaksHataliGiris; i++) k.HataliGirisKaydet(simdi);

        Assert.True(k.KilitliMi(simdi));                    // Şu an kilitli
        Assert.False(k.KilitliMi(simdi.AddMinutes(16)));    // 16 dakika sonra açılmış
    }

    [Fact]
    public void BasariliGiris_SayaciSifirlar()
    {
        var k = new Kullanici();
        var simdi = new DateTime(2026, 11, 22, 10, 0, 0);
        k.HataliGirisKaydet(simdi);
        k.HataliGirisKaydet(simdi);

        k.BasariliGirisKaydet(simdi);

        Assert.Equal(0, k.HataliGirisSayisi);
        Assert.Equal(simdi, k.SonGirisTarihi);
    }
}

public class YetkiMatrisiTestleri
{
    // Her satır: rol, yetki, beklenen sonuç. 7 test.
    [Theory]
    [InlineData(KullaniciRolu.Admin, Yetki.Ayarlar, true)]
    [InlineData(KullaniciRolu.Satis, Yetki.SiparisGir, true)]
    [InlineData(KullaniciRolu.Satis, Yetki.Fatura, false)]
    [InlineData(KullaniciRolu.Depo, Yetki.StokIslem, true)]
    [InlineData(KullaniciRolu.Depo, Yetki.Tahsilat, false)]
    [InlineData(KullaniciRolu.Muhasebe, Yetki.SiparisOnayla, true)]
    [InlineData(KullaniciRolu.Muhasebe, Yetki.KullaniciYonetimi, false)]
    public void RolYetkileri(KullaniciRolu rol, Yetki yetki, bool beklenen) =>
        Assert.Equal(beklenen, YetkiMatrisi.YetkiliMi(rol, yetki));
}