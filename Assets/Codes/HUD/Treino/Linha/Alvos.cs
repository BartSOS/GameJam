using System.Collections.Generic;
using Datas.Frames;
using Datas.Fase;

namespace HUD.Treino.Linha
{
/// <summary>
/// Linha "ALVOS ........ restantes / total" com as pílulas A1, A2... (fases 04 e 05).
/// Vivo ou destruído vem de frames[].tanks[].alive do tanque de cada alvo (fase.alvos[].tank_id).
/// </summary>
public class HudLinhaAlvos : HudLinhaPilulas
{
    private List<string> ids = new List<string>();
    private List<int> tanques = new List<int>();

    public void Configurar(Fases fase)
    {
        DefinirRotulo(RotulosHud.AlvosRestantes);
        ids = fase.IdsDosAlvos();
        tanques = fase.TanquesDosAlvos();
        MontarPilulas(ids);
        MostrarValor(ids.Count);
        for (int i = 0; i < ids.Count; i++) MostrarPilula(i, EstadoPilula.Vivo);
    }

    public void Mostrar(Frame frame)
    {
        int restantes = 0;
        for (int i = 0; i < tanques.Count; i++)
        {
            bool vivo = FramesHud.TanqueVivo(frame, tanques[i]);
            if (vivo) restantes++;
            MostrarPilula(i, vivo ? EstadoPilula.Vivo : EstadoPilula.Destruido);
        }
        MostrarValor(restantes);
    }
}
}