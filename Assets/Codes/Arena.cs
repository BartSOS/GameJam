using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Tanques;
using WebCam;
using Datas;
using Datas.Fase;
using Tiros;
using Datas.Frames;
using Pooling;
using Cysharp.Threading.Tasks;
using HUD;

public class ArenaVisualizer : ADependsProject
{
    [Header("Prefabs de Referência")]
    private PoolerManager poolerManager;
    public GameObject CheckPoint;
    public GameObject CardTank;

    public Transform Painel;
    private int indice;
    private bool pode;
    private bool gameOver;
    private Project project;

    private Dictionary<string, CheckPoint> checkPointsNaCena = new Dictionary<string, CheckPoint>();
    private Dictionary<int, List<CheckPoint>> checkPointsNaCenaEliminacao = new Dictionary<int, List<CheckPoint>>();

    private Dictionary<int, CardTankUI> cardTanksAtivos = new Dictionary<int, CardTankUI>();
    private CameraDigital cameraDigital;
    private Jogador jogador;
    public Tela tela;


    protected override void AoAcordar()
    {
        cameraDigital = Camera.main.GetComponent<CameraDigital>();
    }

    protected override async UniTask AoCarregarProject(Project project)
    {
        await cameraDigital.Pronto;
        this.project = project;
        StartFase(project.fase);
    }

    void Start()
    {
        poolerManager = PoolerManager.INSTANCE;
        pode = false;
        indice = 0;
        gameOver = false;
    }

    void FixedUpdate()
    {    
        if (!Configurado) return;

        FixedUpdateGame();
    }

    private void StartFase(Fases fase)
    {
        if (fase == null)
        {
            Debug.LogError("Fase não encontrada no JSON.");
            return;
        }

        CriaJogador(fase.jogador);
        CriaAlvos(fase.alvos);  
        CriaPontos(fase.pontos);
    }

    private void CriaPontos(List<Ponto> pontos)
    {
        if (pontos == null || pontos.Count == 0)
        {
            Debug.LogError("Lista de Pontos não encontrada ou vazia no JSON.");
            return;
        }

        foreach (Ponto ponto in pontos)
        {
            CriaPonto(ponto);
        }
    }

    private void CriaPonto(Ponto ponto)
    {
        if (ponto == null)
        {
            Debug.LogError("Ponto não encontrado no JSON.");
            return;
        }

        Debug.LogError($"Criando Ponto: {ponto.id}, x: {ponto.x}, y: {ponto.y}, raio: {ponto.raio}, consumido_tick: {ponto.consumido_tick}");

        if (!checkPointsNaCena.ContainsKey(ponto.id))
        {
            GameObject novoPonto = Instantiate(CheckPoint, transform);
            novoPonto.name = $"CheckPoint_{ponto.id}";

            CheckPoint checkPointScript = novoPonto.GetComponent<CheckPoint>();
            checkPointScript.onEntry(ponto.raio, ponto.x, ponto.y, ponto.consumido_tick, ponto.ordem);

            checkPointsNaCena.Add(ponto.id, checkPointScript);

            if (!checkPointsNaCenaEliminacao.ContainsKey(ponto.consumido_tick))
            {
                checkPointsNaCenaEliminacao.Add(ponto.consumido_tick, new List<CheckPoint>());
            }
            checkPointsNaCenaEliminacao[ponto.consumido_tick].Add(checkPointScript);
        }
    }

    private void CriaJogador(Jogador jogadorFase)
    {
        if (jogadorFase == null)
        {
            Debug.LogError("Jogador não encontrado no JSON.");
            return;
        }

        
        Tank t = new Tank
        {
            id = jogadorFase.tank_id,
            x = jogadorFase.x,
            y = jogadorFase.y,
            angle = jogadorFase.angulo,
            hp = 100,
            energy = 100,
            shield = false,
            alive = true
        };

        List<Tank> a = new();
        a.Add(t);
        AtualizarPoolAble<Tanque, Tank>(a);
    }

    private void CriaAlvos(List<Alvo> alvos)
    {
        foreach (var alvo in alvos)
        {
            Tank t = new Tank
            {
                id = alvo.tank_id,
                x = alvo.x,
                y = alvo.y,
                angle = alvo.angulo,
                hp = alvo.hp,
                energy = 100,
                shield = false,
                alive = true
            };
            
            List<Tank> a = new();
            a.Add(t);
            AtualizarPoolAble<Tanque, Tank>(a);
        }
    }

    private void AtualizarCheckPoints(int tick)
    {
        if (checkPointsNaCenaEliminacao.ContainsKey(tick))
        {
            List<CheckPoint> checkPoints = checkPointsNaCenaEliminacao[tick];
            foreach (CheckPoint checkPoint in checkPoints)
            {
                if (checkPoint != null)
                {
                    checkPoint.gameObject.SetActive(false); // Desativa o CheckPoint quando consumido
                }
            }
        }
    }
    

    private void FixedUpdateGame()
    {
        if (indice < project.frames.Count)
        {
            if (pode)
            {
                AtualizarTanques(project.frames[indice].tanks);
                AtualizarProjeteis(project.frames[indice].projectiles);
                AtualizarHazards(project.frames[indice].hazards);
                AtualizarCardTanks(project.frames[indice].tanks);
                AtualizarItems(project.frames[indice].items);
                AtualizarCheckPoints(project.frames[indice].tick);

                Debug.Log(project.frames[indice].tick);
                
                indice++;
                pode = false;
                poolerManager.UpdateTick(project.frames[indice].tick);
            }

            else
            {
                pode = true;
            }
        }

        else 
        {
            if (!gameOver)
            {
                gameOver = true;
                // AudioArena audioController = GetComponent<AudioArena>();
                // var unicoItem = tanquesNaCena.FirstOrDefault(t => t.Value.gameObject.activeSelf);
                // audioController.tocaSomSaida(unicoItem.Value.name); 
                tela.MostraMsgFinal();
            }
        }
    }

   

    private void AtualizarHazards(List<Hazard> hazardDatas)
    {
        List<Hazard> lasers = hazardDatas.FindAll(h => h.type == "LASER");
        List<Hazard> artilharias = hazardDatas.FindAll(h => h.type == "ARTILLERY");

        AtualizarPoolAble<Laser, Hazard>(lasers);
        AtualizarPoolAble<Artillery, Hazard>(artilharias);
    }

    private void AtualizarTanques(List<Tank> dadosTanques)
    {
        foreach (Tank tank in dadosTanques)
        {
            if (!tank.alive)
                PoolerManager.INSTANCE.GetVisual<Tanque>(tank.id).gameObject.SetActive(false);
                
            atualizarPool<Tanque, Tank>(tank);
        }
    }

    private void AtualizarProjeteis(List<Projectile> dadosProjetis)
    {
        AtualizarPoolAble<ManagerTiro, Projectile>(dadosProjetis);
    }

    private void AtualizarCardTanks(List<Tank> dadosCardTanks)
    {
        if (dadosCardTanks == null) return;

        foreach (Tank t in dadosCardTanks)
        {
            CardTankUI cardTankUI;

            if (!cardTanksAtivos.ContainsKey(t.id))
            {
                GameObject novoCard = Instantiate(CardTank, Painel, false);
                novoCard.name = $"CardTank_ID_{t.id}";
    
                cardTankUI = novoCard.GetComponent<CardTankUI>();

                if (t == null || t.id == 0)
                {
                    Debug.LogError("Tank ou Tank ID é nulo.");
                    continue;
                }
                if (!LoadJson.project.nomesDosTanques.ContainsKey(t.id)) Debug.LogError($"Nome do tanque com ID {t.id} não encontrado.");

                cardTankUI.onEntry(t.id, LoadJson.project.nomesDosTanques[t.id], this); // Exemplo de nome, você pode ajustar conforme necessário


                cardTanksAtivos.Add(t.id, cardTankUI);
            }

            cardTankUI = cardTanksAtivos[t.id];
            cardTankUI.updateValues(t.hp, t.energy, t.shield);
        }
    }

    private void AtualizarItems(List<Item> dadosItems)
    {
        List<Item> dadosMedKits = dadosItems.FindAll(item => item.type == "HEAL");
        List<Item> dadosLightning = dadosItems.FindAll(item => item.type == "ENERGY");

        AtualizarPoolAble<MedKit, Item>(dadosMedKits);
        AtualizarPoolAble<Lightning, Item>(dadosLightning);
    }

    private void AtualizarPoolAble<TVisual, TData>(List<TData> dados) where TData : IData where TVisual : APoolAbleMonobehaviour<TData>
    {
        foreach (TData data in dados)
        {
            PoolerManager.INSTANCE.Morph<TVisual, TData>(data.id, data);
        }
    }

    private void atualizarPool<TVisual, TData>(TData data) where TData : IData where TVisual : APoolAbleMonobehaviour<TData>
    {
        PoolerManager.INSTANCE.Morph<TVisual, TData>(data.id, data);
    }
    // ========================================================================
    // MOTOR GENÉRICO DE OBJECT POOL
    // ========================================================================
    private void SincronizarPool<TData, TVisual>(
        List<TData> dadosJson,                   // Lista do JSON (ex: dadosProjetis ou dadosHazards)
        Dictionary<int, TVisual> dicionarioAtivos, // Dicionário de quem está na tela
        Queue<TVisual> pool,                     // Fila de reciclagem
        GameObject prefabBase,                   // Qual prefab instanciar se a fila estiver vazia
        Func<TData, int> extrairId,              // Como ler o ID (ex: p => p.id)
        Action<TData, TVisual> aoNascer,         // O que o script deve fazer no primeiro frame
        Action<TData, TVisual> aoAtualizar,      // O que o script deve fazer a cada frame (movimento)
        Func<TVisual, IEnumerator> rotinaMorte   // A Coroutine de explosão/desligamento
    ) where TVisual : MonoBehaviour              // Garante que TVisual é um script da Unity
    {
        if (dadosJson == null) return;

        HashSet<int> idsNoFrameAtual = new HashSet<int>();

        // 1. POSICIONA OU REUTILIZA
        foreach (TData itemData in dadosJson)
        {
            int id = extrairId(itemData);
            idsNoFrameAtual.Add(id);

            if (!dicionarioAtivos.ContainsKey(id))
            {
                TVisual scriptVisual;

                if (pool.Count > 0)
                {
                    scriptVisual = pool.Dequeue();
                    scriptVisual.gameObject.SetActive(true);
                }
                else
                {
                    GameObject novoObj = Instantiate(prefabBase, transform);
                    novoObj.name = $"{prefabBase.name}_{id}";
                    scriptVisual = novoObj.GetComponent<TVisual>();
                }

                // Executa a regra específica que você passou para o nascimento
                aoNascer?.Invoke(itemData, scriptVisual);

                dicionarioAtivos.Add(id, scriptVisual);
            }

            // Executa a regra específica de atualização
            aoAtualizar?.Invoke(itemData, dicionarioAtivos[id]);
        }

        // 2. LIMPEZA E RECICLAGEM
        List<int> idsParaRemover = new List<int>();
        foreach (var kvp in dicionarioAtivos)
        {
            if (!idsNoFrameAtual.Contains(kvp.Key))
                idsParaRemover.Add(kvp.Key);
        }

        foreach (int idRemovido in idsParaRemover)
        {
            TVisual scriptRemovido = dicionarioAtivos[idRemovido];
            dicionarioAtivos.Remove(idRemovido);

            // Usa a função auxiliar abaixo para esperar a animação acabar e guardar na fila
            StartCoroutine(RotinaReciclagemGenerica(scriptRemovido, pool, rotinaMorte));
        }
    }

    private IEnumerator RotinaReciclagemGenerica<TVisual>(
        TVisual script, 
        Queue<TVisual> pool, 
        Func<TVisual, IEnumerator> rotinaMorte
    ) where TVisual : MonoBehaviour
    {
        IEnumerator rotina = rotinaMorte?.Invoke(script);

        if (rotina != null)
            yield return StartCoroutine(rotina);
        
        // Só devolve para a caixa de munição quando a fumaça acabar
        pool.Enqueue(script);
    }

    public Tanque getTankById(int id)
    {
        Tanque t = poolerManager.GetVisual<Tanque>(id);

        if (t == null)
        {
            Debug.LogError($"Tanque deu merdacom ID {id} não encontrado.");
        }
        return t;
    }
}