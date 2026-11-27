using System.Globalization;
using UnityEngine;

namespace HUD.Treino{
/// <summary>
/// Formatação de números para os textos do HUD. Não sabe nada de fase:
/// só transforma valores em texto ou em proporção para barras.
/// </summary>
public static class FormatoHud
{
    /// <summary>1 casa decimal com vírgula ("84,9"), sem depender do idioma do sistema (WebGL).</summary>
    public static string Decimal1(float valor)
    {
        return valor.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',');
    }

    /// <summary>Número inteiro arredondado ("80").</summary>
    public static string Inteiro(float valor)
    {
        return Mathf.RoundToInt(valor).ToString();
    }

    /// <summary>"/ 360"</summary>
    public static string Total(int total)
    {
        return "/ " + total;
    }

    /// <summary>Proporção 0..1 para preencher uma barra.</summary>
    public static float Proporcao(int atual, int total)
    {
        return total > 0 ? Mathf.Clamp01((float)atual / total) : 0f;
    }
}
}