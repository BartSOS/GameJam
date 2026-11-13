using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace Datas.Fase{
[Serializable] public class Fases {
    public string id; 
    public int seed; 
    public int max_ticks;
    public Jogador jogador;
    public List<Ponto> pontos;
    public List<Alvo> alvos;
    public List<Progresso> progresso = new();
    public List<Artilharia> artilharia;
    public string titulo;
    public int limite_frame = -1;    // prazo da fase; -1 = sem prazo

    public bool laser; public bool itens; public bool zona;

    public bool TemProgresso => !string.IsNullOrEmpty(id) && progresso != null && progresso.Count > 0;

    /// <summary>"02" a partir de "02_parar".</summary>
    public bool TemPonto => pontos != null && pontos.Count > 0;
}
}