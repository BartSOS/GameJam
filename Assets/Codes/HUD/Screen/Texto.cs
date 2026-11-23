using UnityEngine;

namespace HUD.Screen
{
public static class Texto
{
    public static readonly string Vitoria = "VITORIA";
    public static readonly string Derrota = "DERROTA";
    public static readonly string Empate = "EMPATE";
    public static readonly string Neutro = "FIM";

    public static string Causa(string c) => c switch
    {
        "TIRO" => "por um tiro", "ARTILHARIA" => "pela artilharia", "LASER" => "pelo laser orbital",
        "ZONA" => "pela zona de radiação", "TIMEOUT" => "porque o bot não respondeu a tempo",
        "DESCONEXAO" => "porque o programa do bot encerrou", _ => ""
    };
}
}


// "VITÓRIA",
// "DERROTA",
// "EMPATE",
// "FIM DE PARTIDA",