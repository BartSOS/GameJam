using Pontos;
using System;

namespace Datas.Fase
{
[Serializable] public class Ponto : IData
{ 
    public string id; 
    public int ordem; 
    public float x;
    public float y;
    public float raio;
    public int consumido_tick;

    float IData.x { get => x; set => x = value; }
    float IData.y { get => y; set => y = value; }
    int IData.id { get => id.GetHashCode();}

    public bool ConsumidoAte(int frame)
    {
        return consumido_tick >= 0 && consumido_tick <= frame;
    }

    ///<summary>Fora, Dentro ou Consumido para o frame deste progresso.</summary>
    public EstadoCheckPoint EstadoNo(Progresso progresso)
    {
        if (progresso == null) return EstadoCheckPoint.Fora;
        if (ConsumidoAte(progresso.frame)) return EstadoCheckPoint.Consumido;
        if (progresso.EstaNoPonto(this.id)) return EstadoCheckPoint.Dentro;
        return EstadoCheckPoint.Fora;
    }
}
}