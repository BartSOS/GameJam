using System;

namespace Datas.Fase
{
[Serializable]
public class Progresso
{
    public int frame;          // tick do frame
    public string ponto;       // id do ponto da vez ("B"); "" quando não há ponto ou o tanque morreu
    public float distancia;    // do centro do tanque ao centro do ponto; -1 quando ponto == ""
    public bool dentro;        // distancia <= raio do ponto
    public int seguidos;       // frames seguidos dentro do raio (pode passar de precisa)
    public int precisa;        // frames seguidos exigidos (20 na fase 02); 0 se não exige
    public float velocidade;   // unidades por frame
    public bool concluido;     // fase cumprida neste frame ou antes

    public bool TemPonto => !string.IsNullOrEmpty(ponto);
}
}
