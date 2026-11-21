namespace BayiAI.Core.Interfaces;

/// <summary>
/// Yapay zeka sağlayıcısından bağımsız sözleşme.
/// Gemini, Claude, OpenAI veya yerel Ollama: hepsi bu arayüzü uygular,
/// sağlayıcı değiştirmek Program.cs'te tek satırı değiştirmektir.
/// </summary>
public interface IYapayZekaServisi
{
    Task<string> SorAsync(string sistemTalimati, string kullaniciMesaji, CancellationToken iptal = default);
}
