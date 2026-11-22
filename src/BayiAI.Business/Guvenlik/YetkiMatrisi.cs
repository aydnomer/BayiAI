// KullaniciRolu ve Yetki enum'larını kullanmak için Core projesindeki Enums klasörünü ekliyoruz.
using BayiAI.Core.Enums;

namespace BayiAI.Business.Guvenlik;

// Hangi rolün hangi işlemleri yapabileceğini TEK bir yerde tanımlar.
// Kural değişirse sadece bu dosyayı değiştiririz, ekranlara tek tek dokunmayız.
public static class YetkiMatrisi
{
    // Dictionary (sözlük): Anahtar → değer eşleşmesi tutar. Burada: Rol → o rolün yetkileri.
    // HashSet: Tekrarsız liste. "Bu yetki içinde var mı?" sorusunu çok hızlı cevaplar.
    // private static readonly: Sadece bu sınıf görür, tek kopya, sonradan yerine başka sözlük konamaz.
    private static readonly Dictionary<KullaniciRolu, HashSet<Yetki>> Matris = new()
    {
        // Admin'e liste vermiyoruz, çünkü aşağıda "Admin her şeyi yapar" diye ayrıca kontrol ediyoruz.
        [KullaniciRolu.Admin] = new(),

        // Satış personeli: bayilerle ilgilenir, sipariş girer, rapor görür.
        [KullaniciRolu.Satis] = new()
        {
            Yetki.BayiGoruntule, Yetki.BayiDuzenle, Yetki.UrunGoruntule,
            Yetki.SiparisGir, Yetki.CariGoruntule, Yetki.Raporlar, Yetki.YapayZeka
        },

        // Depo personeli: ürün ve stokla, sevkiyatla ilgilenir.
        [KullaniciRolu.Depo] = new()
        {
            Yetki.UrunGoruntule, Yetki.UrunDuzenle, Yetki.StokIslem, Yetki.Sevkiyat
        },

        // Muhasebe: sipariş onaylar, fatura keser, tahsilat alır.
        [KullaniciRolu.Muhasebe] = new()
        {
            Yetki.BayiGoruntule, Yetki.UrunGoruntule, Yetki.SiparisOnayla, Yetki.Fatura,
            Yetki.Tahsilat, Yetki.CariGoruntule, Yetki.Raporlar, Yetki.YapayZeka
        }
    };

    // "Bu rol bu işlemi yapabilir mi?" sorusunun cevabı. true = yapabilir.
    public static bool YetkiliMi(KullaniciRolu rol, Yetki yetki)
    {
        // Admin ise sormaya gerek yok, her şeye yetkili.
        if (rol == KullaniciRolu.Admin) return true;

        // TryGetValue: Sözlükte bu rol varsa yetkilerini "yetkiler" değişkenine koyar ve true döner.
        // out var: Metodun içinden dışarıya değer çıkarmanın yolu.
        // Contains: Listede bu yetki var mı?
        return Matris.TryGetValue(rol, out var yetkiler) && yetkiler.Contains(yetki);
    }

    // Bir rolün sahip olduğu tüm yetkileri döndürür. Menüleri açıp kapatırken kullanacağız.
    // IReadOnlyCollection: Sadece okunabilen liste, dışarıdan eleman eklenemez.
    public static IReadOnlyCollection<Yetki> RolYetkileri(KullaniciRolu rol)
    {
        // Admin: Yetki enum'ındaki tüm değerleri döndür.
        if (rol == KullaniciRolu.Admin) return Enum.GetValues<Yetki>();

        // Diğer roller: sözlükteki listesini döndür.
        if (Matris.TryGetValue(rol, out var yetkiler)) return yetkiler;

        // Tanımsız bir rol gelirse boş liste döndür (hiçbir yetki yok).
        return Array.Empty<Yetki>();
    }
}