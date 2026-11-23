using BayiAI.Business.Dogrulama;

namespace BayiAI.Tests;

public class DogrulayiciTestleri
{
    // => ile tek satırda test: Assert'in sonucu metodun gövdesi.
    [Theory]
    [InlineData("1234567890", true)]    // algoritmaya uyan gerçek formatta VKN
    [InlineData("9876543217", true)]
    [InlineData("1234567891", false)]   // son hane (kontrol basamağı) yanlış
    [InlineData("12345", false)]        // eksik hane
    [InlineData("12345abcde", false)]   // rakam olmayan karakter
    public void Vkn(string vkn, bool beklenen) => Assert.Equal(beklenen, Dogrulayici.VknGecerliMi(vkn));

    [Theory]
    [InlineData("10000000146", true)]   // algoritmaya uyan örnek TCKN
    [InlineData("12345678901", false)]
    [InlineData("01234567890", false)]  // 0 ile başlayamaz
    public void Tckn(string tckn, bool beklenen) => Assert.Equal(beklenen, Dogrulayici.TcknGecerliMi(tckn));

    [Theory]
    [InlineData("satis@yildizgida.com", true)]
    [InlineData("yanlis-adres", false)]
    [InlineData(null, true)]            // boş bırakılabilir, zorunlu alan değil
    public void Eposta(string? eposta, bool beklenen) => Assert.Equal(beklenen, Dogrulayici.EpostaGecerliMi(eposta));

    [Theory]
    [InlineData("0216 000 00 01", true)]  // boşluklar sayılmaz, 11 rakam
    [InlineData("123", false)]
    public void Telefon(string telefon, bool beklenen) => Assert.Equal(beklenen, Dogrulayici.TelefonGecerliMi(telefon));
}