using System;
using System.Collections.Generic;

namespace Datas.Fase{
[Serializable] public class Fases {
    public string id; 
    public int seed; 
    public int max_ticks;
    public Jogador jogador;
    public List<Ponto> pontos;
    public List<Alvo> alvos;
    public List<Artilharia> artilharia;
    public bool laser; public bool itens; public bool zona;
}
}