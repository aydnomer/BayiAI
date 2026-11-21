namespace BayiAI.Core.Enums;

public enum KullaniciRolu : byte { Admin = 1, Satis = 2, Depo = 3, Muhasebe = 4 }

public enum SiparisDurumu : byte { Beklemede = 1, Onaylandi = 2, SevkEdildi = 3, TeslimEdildi = 4, Iptal = 5 }

public enum StokHareketTipi : byte { Giris = 1, Cikis = 2, Sayim = 3 }

public enum KasaBankaTipi : byte { Kasa = 1, Banka = 2 }

public enum OdemeTipi : byte { Nakit = 1, Havale = 2, Cek = 3, KrediKarti = 4 }

public enum CariIslemTipi : byte { Fatura = 1, Tahsilat = 2, Iade = 3, Duzeltme = 4 }

public enum AiIslemTipi : byte { SiparisTahmini = 1, RiskYorumu = 2, DogalDilSorgu = 3 }
