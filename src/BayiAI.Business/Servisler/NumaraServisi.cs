using BayiAI.Core.Entities;
using BayiAI.Core.Exceptions;
using BayiAI.Core.Interfaces;

namespace BayiAI.Business.Servisler;

// Belge numarası verir. Sipariş, fatura vb. kaydederken "sıradaki numara ne?" diye buna soracağız.
public class NumaraServisi
{
    // Belge kodlarını sabit olarak tanımlıyoruz. Kodda "FTR" yazmak yerine NumaraServisi.Fatura yazarız,
    // böylece yazım hatası yapma ihtimali kalmaz.
    public const string Bayi = "BAY";
    public const string Siparis = "SIP";
    public const string Irsaliye = "IRS";
    public const string Fatura = "FTR";
    public const string Tahsilat = "THS";

    private readonly IRepository<NumaraSerisi> _seriler;
    private readonly TimeProvider _saat;

    public NumaraServisi(IRepository<NumaraSerisi> seriler, TimeProvider? saat = null)
    {
        _seriler = seriler;
        _saat = saat ?? TimeProvider.System;
    }

    // Örnek kullanım: string no = await numaraServisi.SonrakiAsync(NumaraServisi.Fatura);  → "FTR2026000001"
    public async Task<string> SonrakiAsync(string kod)
    {
        // Seriyi bul. ?? throw: Bulunamazsa (null ise) hata fırlat.
        var seri = await _seriler.BulAsync(x => x.Kod == kod)
                   ?? throw new IsKuraliException($"'{kod}' için numara serisi tanımlı değil.");

        // Numarayı üret (SonNumara 1 artar) ve veritabanına kaydet.
        string numara = seri.SonrakiNumara(_saat.GetLocalNow().DateTime);
        await _seriler.KaydetAsync();
        return numara;
    }
}