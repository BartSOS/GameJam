namespace HUD.Treino{
/// <summary>
/// Todos os textos fixos das linhas do HUD do treino, num lugar só.
/// Para mudar um rótulo, mude aqui: as linhas e os objetivos leem daqui.
/// </summary>
public static class RotulosHud
{
    // Linha do frame do painel (HudLinhaBarraFrame)
    public const string Frame          = "Frame";

    // Linhas de valor (HudLinhaValor)
    public const string Velocidade     = "Velocidade";
    public const string Tiros          = "Tiros";
    public const string ProximaBombaEm = "Próxima Bomba em";
    public const string Bombas         = "Bombas";

    // Linhas com barra (HudLinhaBarra)
    public const string NoRaio         = "No Raio";
    public const string Energia        = "Energia";
    public const string Vida           = "Vida";

    // Linhas com pílulas (HudLinhaPilulas)
    public const string AlvosRestantes = "Alvos Restantes";
    public const string Pontos         = "Pontos";

    // Linha de distância (HudLinhaDistancia): "Distância a " + id do ponto ("Distância a B")
    public const string DistanciaA     = "Distância a ";
    public const string Dentro         = "Dentro";
    public const string Fora           = "Fora";
}
}
