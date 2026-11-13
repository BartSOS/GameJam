using UnityEngine;
using System;

namespace Datas
{
[Serializable] public class Bot
{
    public int id;
    public string nome;
    public string lang;
    public int posicao;
    public bool vivo;
    public int kills;
    public float hp;
    public bool vencedor; 
    public int tick_eliminacao;
    public int abatido_por;
    public string causa_eliminacao;
    public float dano_causado;
}
}

// id": 1,
// "nome": "base_bfs",
// "lang": "Python 3",
// "posicao": 1,
// "vivo": true,
// "kills": 2,
// "hp": 100.0,
// "vencedor": true,
// "tick_eliminacao": null,
// "abatido_por": null,
// "causa_eliminacao": null,
// "dano_causado": 150.0