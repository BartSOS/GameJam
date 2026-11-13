using UnityEngine;

namespace HUD.Screen
{
public static class Cores
{
    public static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    public static readonly Color Fundo          = Hex("#0B1016");
    public static readonly Color Escurecimento  = Hex("#0B1016D6"); // 84%
    public static readonly Color Placa          = Hex("#0B1016B8"); // 72% (HUD)
    public static readonly Color Cartao         = Hex("#151C26E6"); // 90%
    public static readonly Color ContornoNeutro = Hex("#263241");
    public static readonly Color Texto          = Hex("#E6EDF3");
    public static readonly Color TextoApagado   = Hex("#7D8B99");
    public static readonly Color Vitoria        = Hex("#3DDC84");
    public static readonly Color Derrota        = Hex("#FF5252");
    public static readonly Color Empate         = Hex("#FF9F43");
    public static readonly Color Vida           = Hex("#FF3B3B");
    public static readonly Color VidaBaixa      = Hex("#FF5252");
    public static readonly Color Escudo         = Hex("#00E5FF");
    public static readonly Color FundoBarra     = Hex("#0000008C"); // preto 55%
    public static readonly Color Divisao        = Hex("#0B1016B3"); // 70%

    public static Color ComAlpha(Color c, float a) { c.a = a; return c; }
}
}