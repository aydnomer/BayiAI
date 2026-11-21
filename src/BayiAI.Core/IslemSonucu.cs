namespace BayiAI.Core;

/// <summary>
/// Servislerin arayüze döndürdüğü generic sonuç tipi.
/// Başarılıysa Veri dolu, değilse Mesaj kullanıcıya gösterilecek açıklamadır.
/// </summary>
public class IslemSonucu<T>
{
    public bool Basarili { get; }
    public T? Veri { get; }
    public string Mesaj { get; }

    private IslemSonucu(bool basarili, T? veri, string mesaj)
    {
        Basarili = basarili;
        Veri = veri;
        Mesaj = mesaj;
    }

    public static IslemSonucu<T> Basari(T veri, string mesaj = "İşlem başarılı.") => new(true, veri, mesaj);
    public static IslemSonucu<T> Hata(string mesaj) => new(false, default, mesaj);
}
