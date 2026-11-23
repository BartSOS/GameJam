using TMPro;
using UnityEngine;

namespace HUD.Treino.Linha{
/// <summary>
/// Linha "RÓTULO ........ valor complemento" (VELOCIDADE 5,0 / frame; TIROS 4; BOMBAS 2 / 7).
/// O rótulo fica fixo no prefab; o código só escreve o valor e, se precisar, o complemento.
/// </summary>
public class HudLinhaValor : MonoBehaviour
{
    [Tooltip("Valor grande (\"5,0\").")]
    public TMP_Text textoValor;
    [Tooltip("Complemento em cinza (\"/ frame\", \"/ 7\"). Opcional; se o texto for fixo, não precisa ligar.")]
    public TMP_Text textoComplemento;

    public void Mostrar(string valor)
    {
        if (textoValor) textoValor.text = valor;
    }

    public void DefinirComplemento(string complemento)
    {
        if (textoComplemento) textoComplemento.text = complemento;
    }

    /// <summary>Mostra ou esconde o complemento (ex.: "frames" some quando o valor é "—").</summary>
    public void MostrarComplemento(bool visivel)
    {
        if (textoComplemento) textoComplemento.gameObject.SetActive(visivel);
    }
}
}