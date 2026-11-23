using BayiAI.Business.Guvenlik;

namespace BayiAI.Tests;

// Her test sınıfı bir konuyu test eder. Her metot bir senaryo.
public class SifreHasherTestleri
{
    // [Fact]: "Bu metot bir test, parametresi yok, bir kez çalıştır" etiketi.
    [Fact]
    public void DogruSifre_Dogrulanir()
    {
        string hash = SifreHasher.Hashle("Bayi2026");
        // Assert.True: İçindeki ifade true değilse test KIRMIZI olur.
        Assert.True(SifreHasher.Dogrula("Bayi2026", hash));
    }

    [Fact]
    public void YanlisSifre_Reddedilir()
    {
        string hash = SifreHasher.Hashle("Bayi2026");
        // Büyük-küçük harf farkı bile yanlış sayılmalı.
        Assert.False(SifreHasher.Dogrula("bayi2026", hash));
    }

    [Fact]
    public void AyniSifre_HerSeferindeFarkliHashUretir()
    {
        // Salt sayesinde aynı şifrenin iki hash'i birbirinden farklı olmalı.
        Assert.NotEqual(SifreHasher.Hashle("Bayi2026"), SifreHasher.Hashle("Bayi2026"));
    }

    [Fact]
    public void BozukHash_HataVermedenReddedilir()
    {
        // Veritabanında bozuk bir değer varsa program çökmemeli, sadece false dönmeli.
        Assert.False(SifreHasher.Dogrula("Bayi2026", "ORNEK_HASH"));
    }

    // [Theory]: Aynı testi farklı verilerle birden çok kez çalıştır.
    // Her [InlineData] satırı ayrı bir test olarak sayılır (burada 4 test).
    [Theory]
    [InlineData("kisa1", false)]        // 8 karakterden kısa
    [InlineData("sadeceharf", false)]   // rakam yok
    [InlineData("12345678", false)]     // harf yok
    [InlineData("Bayi2026", true)]      // kurallara uygun
    public void SifreKurali(string sifre, bool gecerliMi)
    {
        // Assert.Equal(beklenen, gerçekleşen): İkisi eşit değilse test kırmızı.
        Assert.Equal(gecerliMi, SifreHasher.KuralHatasi(sifre) is null);
    }
}