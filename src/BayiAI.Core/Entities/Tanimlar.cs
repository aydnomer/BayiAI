using BayiAI.Core.Enums;

namespace BayiAI.Core.Entities;

// Kullanici sınıfı BaseEntity'den miras alır (Id, OlusturmaTarihi gibi ortak alanlar oradan gelir).
public class Kullanici : BaseEntity
{
    // Üst üste en fazla 5 hatalı giriş hakkı var.
    public const int MaksHataliGiris = 5;

    // Hesap kilitlenince 15 dakika kilitli kalır.
    // static readonly: Tüm kullanıcılar için ortak, program çalışınca bir kez atanır, sonra değişmez.
    // TimeSpan: "süre" tutan tip.
    public static readonly TimeSpan KilitSuresi = TimeSpan.FromMinutes(15);

    // Kullanıcının adı soyadı. = "" ile başlangıçta boş metin verilir, null olmaz.
    public string AdSoyad { get; set; } = "";

    // Giriş ekranında yazılacak kullanıcı adı.
    public string KullaniciAdi { get; set; } = "";

    // Şifrenin kendisi değil, SifreHasher ile üretilen hash'i saklanır.
    public string SifreHash { get; set; } = "";

    // Kullanıcının rolü (Yönetici, Muhasebe, Satış vb.). Enum olarak tutulur.
    public KullaniciRolu Rol { get; set; }

    // false yapılırsa kullanıcı sisteme giremez. Varsayılan: aktif.
    public bool AktifMi { get; set; } = true;

    // private set: Bu değerleri dışarıdan kimse direkt değiştiremez,
    // sadece aşağıdaki metotlar değiştirebilir. Buna KAPSÜLLEME denir (OOP'nin temel ilkesi).
    public int HataliGirisSayisi { get; private set; }

    // DateTime? : Tarih olabilir ya da boş (null) olabilir. Boşsa hesap kilitli değil demek.
    public DateTime? KilitBitisTarihi { get; private set; }

    // En son başarılı giriş zamanı. Hiç giriş yapmadıysa boş.
    public DateTime? SonGirisTarihi { get; private set; }

    // true ise kullanıcı ilk girişte şifresini değiştirmek zorunda.
    public bool SifreDegistirmeli { get; set; }

    // Hesap şu an kilitli mi? Kilit tarihi varsa VE o tarih henüz gelmediyse kilitlidir.
    // => işareti: tek satırlık metot yazımı, "şunu döndür" demek.
    public bool KilitliMi(DateTime simdi) => KilitBitisTarihi.HasValue && KilitBitisTarihi.Value > simdi;

    // Hatalı şifre girilince çağrılır.
    public void HataliGirisKaydet(DateTime simdi)
    {
        // Hatalı deneme sayısını 1 artırır.
        HataliGirisSayisi++;

        // 5'e ulaştıysa...
        if (HataliGirisSayisi >= MaksHataliGiris)
        {
            // ...şimdiden 15 dakika sonrasına kadar hesabı kilitler.
            KilitBitisTarihi = simdi.Add(KilitSuresi);

            // Sayacı sıfırlar, kilit bitince kullanıcı yine 5 hak kazanır.
            HataliGirisSayisi = 0;
        }
    }

    // Doğru şifreyle giriş yapılınca çağrılır.
    public void BasariliGirisKaydet(DateTime simdi)
    {
        // Hatalı sayacı sıfırlanır.
        HataliGirisSayisi = 0;

        // Varsa kilit kaldırılır.
        KilitBitisTarihi = null;

        // Son giriş zamanı kaydedilir.
        SonGirisTarihi = simdi;
    }

    // Yeni şifre verilince çağrılır (kullanıcı değiştirdiğinde ya da yönetici sıfırladığında).
    public void SifreAta(string yeniHash, bool sonrakiGiristeDegistirsin)
    {
        // Yeni hash kaydedilir.
        SifreHash = yeniHash;

        // Yönetici sıfırladıysa true gelir, kullanıcı ilk girişte şifresini değiştirmek zorunda kalır.
        SifreDegistirmeli = sonrakiGiristeDegistirsin;

        // Yeni şifreyle temiz başlangıç: sayaç ve kilit sıfırlanır.
        HataliGirisSayisi = 0;
        KilitBitisTarihi = null;
    }
}

// Bölge sınıfı: Marmara, Ege gibi. Bir bölgede birden çok bayi olur (1-N ilişki).
public class Bolge : BaseEntity
{
    public string Ad { get; set; } = "";
    public List<Bayi> Bayiler { get; set; } = new();
}

public class Kategori : BaseEntity
{
    public string Ad { get; set; } = "";

    // Kendine bağlanan ilişki: "Gazlı İçecek" -> üst kategori "İçecek"
    public int? UstKategoriId { get; set; }
    public Kategori? UstKategori { get; set; }
    public List<Kategori> AltKategoriler { get; set; } = new();

    public List<Urun> Urunler { get; set; } = new();
}