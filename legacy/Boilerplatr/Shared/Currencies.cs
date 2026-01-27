using System.Collections.Frozen;

namespace Boilerplatr.Shared;

public readonly record struct Currency(string Code, string Name, string Symbol);

public static class CurrencyCodes
{
    public const string USD = nameof(USD);
    public const string EUR = nameof(EUR);
    public const string JPY = nameof(JPY);
    public const string GBP = nameof(GBP);
    public const string AUD = nameof(AUD);
    public const string CAD = nameof(CAD);
    public const string CHF = nameof(CHF);
    public const string CNY = nameof(CNY);
    public const string SEK = nameof(SEK);
    public const string NZD = nameof(NZD);
    public const string MXN = nameof(MXN);
    public const string SGD = nameof(SGD);
    public const string HKD = nameof(HKD);
    public const string NOK = nameof(NOK);
    public const string KRW = nameof(KRW);
    public const string TRY = nameof(TRY);
    public const string INR = nameof(INR);
    public const string BRL = nameof(BRL);
    public const string RUB = nameof(RUB);
    public const string ZAR = nameof(ZAR);
    public const string None = "";
}

public static class Currencies
{
    public static Currency USD = new(CurrencyCodes.USD, "United States Dollar", "$");
    public static Currency EUR = new(CurrencyCodes.EUR, "Euro", "€");
    public static Currency JPY = new(CurrencyCodes.JPY, "Japanese Yen", "¥");
    public static Currency GBP = new(CurrencyCodes.GBP, "British Pound", "£");
    public static Currency AUD = new(CurrencyCodes.AUD, "Australian Dollar", "A$");
    public static Currency CAD = new(CurrencyCodes.CAD, "Canadian Dollar", "C$");
    public static Currency CHF = new(CurrencyCodes.CHF, "Swiss Franc", "Fr");
    public static Currency CNY = new(CurrencyCodes.CNY, "Chinese Yuan", "¥");
    public static Currency SEK = new(CurrencyCodes.SEK, "Swedish Krona", "kr");
    public static Currency NZD = new(CurrencyCodes.NZD, "New Zealand Dollar", "$");
    public static Currency MXN = new(CurrencyCodes.MXN, "Mexican Peso", "$");
    public static Currency SGD = new(CurrencyCodes.SGD, "Singapore Dollar", "S$");
    public static Currency HKD = new(CurrencyCodes.HKD, "Hong Kong Dollar", "$");
    public static Currency NOK = new(CurrencyCodes.NOK, "Norwegian Krone", "kr");
    public static Currency KRW = new(CurrencyCodes.KRW, "South Korean Won", "₩");
    public static Currency TRY = new(CurrencyCodes.TRY, "Turkish Lira", "₺");
    public static Currency INR = new(CurrencyCodes.INR, "Indian Rupee", "₹");
    public static Currency BRL = new(CurrencyCodes.BRL, "Brazilian Real", "R$");
    public static Currency RUB = new(CurrencyCodes.RUB, "Russian Ruble", "₽");
    public static Currency ZAR = new(CurrencyCodes.ZAR, "South African Rand", "R");

    public static FrozenSet<Currency> All = [USD, EUR, JPY, GBP, AUD, CAD, CHF, CNY, SEK, NZD, MXN, SGD, HKD, NOK, KRW, TRY, INR, BRL, RUB, ZAR];
    public static FrozenSet<string> Codes = All.Select(topic => topic.Code).ToFrozenSet();
    public static Currency Default = USD;

    public static Currency Get(string code)
    {
        return code switch
        {
            CurrencyCodes.USD => USD,
            CurrencyCodes.EUR => EUR,
            CurrencyCodes.JPY => JPY,
            CurrencyCodes.GBP => GBP,
            CurrencyCodes.AUD => AUD,
            CurrencyCodes.CAD => CAD,
            CurrencyCodes.CHF => CHF,
            CurrencyCodes.CNY => CNY,
            CurrencyCodes.SEK => SEK,
            CurrencyCodes.NZD => NZD,
            CurrencyCodes.MXN => MXN,
            CurrencyCodes.SGD => SGD,
            CurrencyCodes.HKD => HKD,
            CurrencyCodes.NOK => NOK,
            CurrencyCodes.KRW => KRW,
            CurrencyCodes.TRY => TRY,
            CurrencyCodes.INR => INR,
            CurrencyCodes.BRL => BRL,
            CurrencyCodes.RUB => RUB,
            CurrencyCodes.ZAR => ZAR,
            _ => throw new NotImplementedException($"Currency code '{code}' is not supported.")
        };
    }

    public static bool TryGet(string? code, out Currency? currency)
    {
        currency = null;

        if (!string.IsNullOrWhiteSpace(code))
        {
            try
            {
                currency = Get(code);
            }
            catch { }
        }

        return currency is not null;
    }
}
