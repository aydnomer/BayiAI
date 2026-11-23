using BayiAI.Business.Guvenlik;
using BayiAI.Business.Servisler;
using BayiAI.Core.Entities;
using BayiAI.Core.Enums;

namespace BayiAI.Tests;

public class GirisServisiTestleri
{
    // Her test için yeni, temiz nesneler oluşturulur (xUnit her test için sınıfı baştan kurar).
    private readonly SahteRepository<Kullanici> _kullanicilar = new();
    private readonly SahteRepository<IslemLog> _loglar = new();
    private readonly OturumBilgisi _oturum = new();
    private readonly SabitSaat _saat = new(new DateTime(2026, 11, 22, 10, 0, 0));
    private readonly GirisServisi _servis;

    // Constructor: Her testten önce çalışır. Sahte veritabanına "ayse" kullanıcısını koyar.
    public GirisServisiTestleri()
    {
        var ayse = new Kullanici { AdSoyad = "Ayşe Satış", KullaniciAdi = "ayse", Rol = KullaniciRolu.Satis };
        ayse.SifreAta(SifreHasher.Hashle("Bayi2026"), sonrakiGiristeDegistirsin: false);
        _kullanicilar.Kayitlar.Add(ayse);

        // Gerçek servisleri sahte parçalarla kuruyoruz.
        var log = new IslemLogServisi(_loglar, _oturum, _saat);
        _servis = new GirisServisi(_kullanicilar, log, _oturum, _saat);
    }

    // async Task: Async metotları test ederken test de async olmalı.
    [Fact]
    public async Task DogruBilgilerle_GirisYapilir_VeOturumBaslar()
    {
        var sonuc = await _servis.GirisYapAsync("ayse", "Bayi2026");

        Assert.True(sonuc.Basarili);
        Assert.True(_oturum.GirisYapildiMi);
        Assert.Equal(KullaniciRolu.Satis, _oturum.Rol);
        Assert.Single(_loglar.Kayitlar);     // Single: Listede tam 1 kayıt var mı? (giriş loglandı)
    }

    [Fact]
    public async Task OlmayanKullanici_GenelHataMesajiAlir()
    {
        var sonuc = await _servis.GirisYapAsync("yok", "Bayi2026");

        Assert.False(sonuc.Basarili);
        Assert.Equal("Kullanıcı adı veya şifre hatalı.", sonuc.Mesaj);
    }

    [Fact]
    public async Task BesYanlisSifre_HesabiKilitler_DogruSifreBileKabulEdilmez()
    {
        for (int i = 0; i < Kullanici.MaksHataliGiris; i++)
            await _servis.GirisYapAsync("ayse", "yanlis123");

        var sonuc = await _servis.GirisYapAsync("ayse", "Bayi2026");   // Doğru şifre

        Assert.False(sonuc.Basarili);
        Assert.Contains("kilitli", sonuc.Mesaj);   // Contains: Mesajın içinde "kilitli" geçiyor mu?
        Assert.False(_oturum.GirisYapildiMi);
    }

    [Fact]
    public async Task KilitSuresiDolunca_GirisYapilabilir()
    {
        for (int i = 0; i < Kullanici.MaksHataliGiris; i++)
            await _servis.GirisYapAsync("ayse", "yanlis123");

        _saat.Ilerlet(TimeSpan.FromMinutes(16));   // Sahte saati 16 dakika ileri al
        var sonuc = await _servis.GirisYapAsync("ayse", "Bayi2026");

        Assert.True(sonuc.Basarili);
    }

    [Fact]
    public async Task PasifKullanici_GirisYapamaz()
    {
        _kullanicilar.Kayitlar[0].AktifMi = false;

        var sonuc = await _servis.GirisYapAsync("ayse", "Bayi2026");

        Assert.False(sonuc.Basarili);
        Assert.Contains("pasif", sonuc.Mesaj);
    }
}
