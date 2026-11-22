// Şifreleme (kriptografi) sınıflarını kullanabilmek için bu kütüphaneyi ekliyoruz.
using System.Security.Cryptography;

// Bu dosya BayiAI.Business projesinin Guvenlik klasöründe duruyor.
namespace BayiAI.Business.Guvenlik;

// static: Bu sınıftan nesne (new) üretmeyiz, metotlarını direkt SifreHasher.Hashle(...) diye çağırırız.
public static class SifreHasher
{
    // const: Değeri hiç değişmeyen sabit.
    // Salt = her şifreye eklenen rastgele 16 baytlık "tuz". Aynı şifreler farklı hash'e dönüşür.
    private const int SaltBoyutu = 16;

    // Sonuçta çıkacak hash'in uzunluğu: 32 bayt (256 bit).
    private const int HashBoyutu = 32;

    // Hash işlemi 100.000 kez tekrarlanır. Saldırganın deneme yapmasını çok yavaşlatır.
    // 100_000 yazımındaki alt çizgi sadece okumayı kolaylaştırır, 100000 ile aynıdır.
    private const int Tekrar = 100_000;

    // Kullanıcının açık şifresini alır, veritabanına kaydedilecek hash metnini döndürür.
    public static string Hashle(string sifre)
    {
        // 16 baytlık tamamen rastgele bir salt üretir.
        byte[] salt = RandomNumberGenerator.GetBytes(SaltBoyutu);

        // PBKDF2 algoritmasıyla şifre + salt'ı SHA256 kullanarak 100.000 kez işler, 32 baytlık hash çıkarır.
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(sifre, salt, Tekrar, HashAlgorithmName.SHA256, HashBoyutu);

        // Hepsini tek metinde birleştirir: "PBKDF2$100000$salt$hash"
        // Base64 = baytları yazıya çeviren yöntem. $ işareti parçaları ayırır.
        // Başındaki $ işareti (string interpolation) süslü parantez içindeki değişkenleri metne yerleştirir.
        return $"PBKDF2${Tekrar}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    // Girişte yazılan şifre ile veritabanındaki hash'i karşılaştırır. Doğruysa true döner.
    public static bool Dogrula(string sifre, string saklananHash)
    {
        // Kayıtlı metni $ işaretinden 4 parçaya böler: [0]=PBKDF2, [1]=tekrar, [2]=salt, [3]=hash
        string[] parcalar = saklananHash.Split('$');

        // 4 parça yoksa ya da ilk parça "PBKDF2" değilse kayıt bozuktur, giriş reddedilir.
        if (parcalar.Length != 4 || parcalar[0] != "PBKDF2") return false;

        // Metin halindeki tekrar sayısını sayıya (int) çevirir.
        int tekrar = int.Parse(parcalar[1]);

        // Base64 yazıları tekrar bayt dizisine çevrilir.
        byte[] salt = Convert.FromBase64String(parcalar[2]);
        byte[] beklenen = Convert.FromBase64String(parcalar[3]);

        // Girilen şifre, KAYITLI salt ve tekrar sayısıyla aynı şekilde hash'lenir.
        byte[] hesaplanan = Rfc2898DeriveBytes.Pbkdf2(sifre, salt, tekrar, HashAlgorithmName.SHA256, beklenen.Length);

        // İki hash karşılaştırılır. FixedTimeEquals her zaman aynı sürede çalışır,
        // böylece saldırgan süreye bakarak kaç baytın tuttuğunu tahmin edemez.
        return CryptographicOperations.FixedTimeEquals(hesaplanan, beklenen);
    }

    // Yeni şifre kurallara uyuyor mu? Uymuyorsa hata mesajı, uyuyorsa null döner.
    // string? : Soru işareti bu değerin null (boş) olabileceğini söyler.
    public static string? KuralHatasi(string? sifre)
    {
        // Şifre boşsa ya da 8 karakterden kısaysa hata.
        if (string.IsNullOrWhiteSpace(sifre) || sifre.Length < 8) return "Şifre en az 8 karakter olmalı.";

        // Any: "içinde en az bir tane var mı" diye bakar. Harf veya rakam yoksa hata.
        // ! işareti "değil" anlamına gelir.
        if (!sifre.Any(char.IsLetter) || !sifre.Any(char.IsDigit)) return "Şifre en az bir harf ve bir rakam içermeli.";

        // Hata yok.
        return null;
    }
}