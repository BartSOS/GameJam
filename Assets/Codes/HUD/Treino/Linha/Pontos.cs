using System.Collections.Generic;
using Datas.Fase;

namespace HUD.Treino.Linha{
/// <summary>
/// Linha "PONTOS ........ k / 4" com as pílulas P1..P4 (fase 03).
/// Feito = pontos[].consumido_tick já passou; Atual = o primeiro ainda não feito; o resto Pendente.
/// </summary>
public class HudLinhaPontos : HudLinhaPilulas
{
    private Fases fase;
    private List<string> ids = new List<string>();

    public void Configurar(Fases novaFase)
    {
        fase = novaFase;
        DefinirRotulo(RotulosHud.Pontos);
        ids = fase.PontosEmOrdem();
        MontarPilulas(ids);
    }

    public void Mostrar(Progresso p)
    {
        if (fase == null) return;
        int feitos = fase.PontosConsumidosAte(p.frame);
        MostrarValor(feitos);
        for (int i = 0; i < ids.Count; i++)
        {
            var estado = i < feitos ? EstadoPilula.Feito
                       : i == feitos ? EstadoPilula.Atual
                       : EstadoPilula.Pendente;
            MostrarPilula(i, estado);
        }
    }
}
}