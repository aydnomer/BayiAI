// MailAddress sınıfı e-posta adresini kontrol etmek için.
using System.Net.Mail;

namespace BayiAI.Business.Dogrulama;

// Form alanlarını kontrol eden kurallar. static: Nesne üretmeden Dogrulayici.VknGecerliMi(...) diye çağrılır.
public static class Dogrulayici
{
    // 10 haneli Vergi Kimlik Numarası (VKN) gerçek mi? Son hane, ilk 9 haneden hesaplanan
    // "kontrol basamağı"dır. Rastgele 10 rakam yazan biri bu kontrolden geçemez.
    public static bool VknGecerliMi(string? vkn)
    {
        // Boşsa, 10 hane değilse ya da içinde rakam olmayan karakter varsa geçersiz.
        // All(char.IsDigit): Tüm karakterler rakam mı?
        if (vkn is null || vkn.Length != 10 || !vkn.All(char.IsDigit)) return false;

        int toplam = 0;
        // İlk 9 haneyi tek tek dolaş (i = 0'dan 8'e kadar).
        for (int i = 0; i < 9; i++)
        {
            // Karakteri sayıya çevir: '7' - '0' = 7 (karakterlerin sıra numarası farkı).
            int rakam = vkn[i] - '0';

            // Gelir İdaresi'nin resmi algoritması:
            int tmp = (rakam + 9 - i) % 10;                 // % : bölümden kalan
            int deger = (tmp * (1 << (9 - i))) % 9;         // 1 << n : 2 üzeri n (bit kaydırma)
            if (tmp != 0 && deger == 0) deger = 9;
            toplam += deger;
        }

        // Hesaplanan kontrol basamağı ile 10. hane aynı mı?
        int kontrol = (10 - toplam % 10) % 10;
        return kontrol == vkn[9] - '0';
    }

    // 11 haneli T.C. Kimlik No kontrolü (şahıs firmaları vergi no yerine TCKN kullanır).
    public static bool TcknGecerliMi(string? tckn)
    {
        // 11 hane, sadece rakam ve ilk hane 0 olamaz.
        if (tckn is null || tckn.Length != 11 || !tckn.All(char.IsDigit) || tckn[0] == '0') return false;

        // Select: Her karakteri sayıya çevir. ToArray: Sonucu diziye koy. d[0] = 1. hane ...
        int[] d = tckn.Select(c => c - '0').ToArray();

        // Kural 1: (1,3,5,7,9. hanelerin toplamı × 7 − 2,4,6,8. hanelerin toplamı) mod 10 = 10. hane
        int tekler = d[0] + d[2] + d[4] + d[6] + d[8];
        int ciftler = d[1] + d[3] + d[5] + d[7];
        int onuncu = ((tekler * 7 - ciftler) % 10 + 10) % 10;   // +10: negatif çıkmasın diye

        // Kural 2: İlk 10 hanenin toplamı mod 10 = 11. hane
        int onbirinci = d.Take(10).Sum() % 10;

        return d[9] == onuncu && d[10] == onbirinci;
    }

    // 11 haneyse TCKN, değilse VKN olarak kontrol et.
    // no?.Length: no null ise hata vermez, null döner (null-conditional operatör).
    public static bool VergiNoGecerliMi(string? no) =>
        no?.Length == 11 ? TcknGecerliMi(no) : VknGecerliMi(no);

    // E-posta formatı doğru mu? Boşsa sorun yok (zorunlu alan değil).
    public static bool EpostaGecerliMi(string? eposta)
    {
        if (string.IsNullOrWhiteSpace(eposta)) return true;

        // TryCreate: .NET'in kendi e-posta ayrıştırıcısı. Başarırsa true döner.
        // Ayrıca nokta içermeli (ali@firma gibi eksik adresleri eler).
        return MailAddress.TryCreate(eposta, out var adres) && adres.Address == eposta.Trim()
               && eposta.Contains('.');
    }

    // Telefonu sadece rakamlarına göre kontrol eder: "(0532) 111 22 33" → 11 rakam → geçerli.
    public static bool TelefonGecerliMi(string? telefon)
    {
        if (string.IsNullOrWhiteSpace(telefon)) return true;
        int haneSayisi = telefon.Count(char.IsDigit);   // Kaç tane rakam var?
        return haneSayisi is 10 or 11;                    // 10 veya 11 ise geçerli (pattern matching)
    }
}