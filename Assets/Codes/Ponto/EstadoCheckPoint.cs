namespace Pontos{
/// <summary>Estado do marcador de ponto no campo.</summary>
public enum EstadoCheckPoint
{
    Fora,      // tanque longe do ponto (ou o ponto não é o da vez)
    Dentro,    // progresso.ponto é este ponto e progresso.dentro = true
    Consumido  // pontos[].consumido_tick já passou (tem prioridade sobre Dentro)
}}