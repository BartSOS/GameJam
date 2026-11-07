using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Tanques;
using WebCam;
using Datas;
using Datas.Fase;
using Datas.Frames;
using Pooling;

public class ArenaVisualizer : MonoBehaviour
{
    [Header("Prefabs de Referência")]
    private PoolManager poolManager;
    public GameObject Laser;
    public GameObject Tank;
    public GameObject Tiro;
    public GameObject Artillery;
    public GameObject CheckPoint;
    public GameObject CardTank;

    public Transform Painel;
    public GameObject MedKit;
    public GameObject Lightning;
    private uint indice;
    private bool pode;
    private bool gameOver;
    private Project project;
    private SpriteRenderer spriteRendererTopo;
    private SpriteRenderer spriteRendererBase;
    [SerializeField] private Sprite [] spritesTopo;
    [SerializeField] private Sprite [] spritesBase;

    private Dictionary<string, CheckPoint> checkPointsNaCena = new Dictionary<string, CheckPoint>();
    private Dictionary<int, List<CheckPoint>> checkPointsNaCenaEliminacao = new Dictionary<int, List<CheckPoint>>();

    private Dictionary<int, Tanque> tanquesNaCena = new Dictionary<int, Tanque>();
    private Dictionary<int, ManagerTiro> projeteisNaCena = new Dictionary<int, ManagerTiro>();
    private Dictionary<int, Laser> laserNaCena= new Dictionary<int, Laser>();
    private Dictionary<int, Artillery> artNaCena= new Dictionary<int, Artillery>();
    private Dictionary<int, CardTankUI> cardTanksAtivos = new Dictionary<int, CardTankUI>();
    private Dictionary<int, MedKit> medkitNaCena= new Dictionary<int, MedKit>();
    private Dictionary<int, Lightning> lightningNaCena= new Dictionary<int, Lightning>();
    private Dictionary<int, string> nomesDosTanques = new Dictionary<int, string>();

    private Queue<ManagerTiro> poolDeProjeteis = new Queue<ManagerTiro>();
    private Queue<Laser> poolDeLasers = new Queue<Laser>();
    private Queue<Artillery> poolDeArt = new Queue<Artillery>();
    private Queue<MedKit> poolDeMedKits = new Queue<MedKit>();
    private Queue<Lightning> poolDeLightnings = new Queue<Lightning>();

    private LoadJson loadJson;
    private bool dadosCarregados;

    void Awake()
    {
        loadJson = GetComponent<LoadJson>();
        poolManager = PoolManager.INSTANCE;
    }

    async void Start()
    {
        dadosCarregados = false;
        pode = false;
        indice = 0;
        gameOver = false;
        
        Camera cam = Camera.main;
        CameraDigital cameraDigital = cam.GetComponent<CameraDigital>();

        SpriteRenderer [] spriteRenderers = Tank.GetComponentsInChildren<SpriteRenderer>();
        spriteRendererTopo = spriteRenderers[0];
        spriteRendererBase = spriteRenderers[1];


        await loadJson.loadProjectTask;
        // cameraDigital.configCamera(loadJson.project.config.arena_width, loadJson.project.config.arena_height);
        dadosCarregados = true; 
        project = loadJson.project;

        StartNamesDosTanques(project.name2Ids);
        StartFase(project.fase);
    }

    void FixedUpdate()
    {    
        if (!dadosCarregados) return;

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
        CriaTanque(t);
        AtualizaTanque(t);
    }

    private void CriaAlvos(List<Alvo> alvos)
    {
        if (alvos == null || alvos.Count == 0)
        {
            Debug.LogError("Lista de Alvos não encontrada ou vazia no JSON.");
            return;
        }

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
            CriaTanque(t);
            AtualizaTanque(t);
        }
    }

    private void AtualizarCheckPoints(int tick)
    {
        if (checkPointsNaCenaEliminacao == null || checkPointsNaCenaEliminacao.Count == 0)
        {
            Debug.LogError("Lista de CheckPoints não encontrada ou vazia.");
            return;
        }   

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
    

    private void StartNamesDosTanques(List<Name2Id> bots)
    {
        if (bots == null || bots.Count == 0)
        {
            Debug.LogError("Lista de Bots não encontrada ou vazia no JSON.");
            return;
        }

        foreach (var bot in bots)
        {
            nomesDosTanques[bot.id] = bot.nome;
        }
    }

    private void FixedUpdateGame()
    {
        if (indice < project.frames.Count)
        {
            if (pode)
            {
                AtualizarTanques(project.frames[(int)indice].tanks);
                AtualizarProjetis(project.frames[(int)indice].projectiles);
                AtualizarHazards(project.frames[(int)indice].hazards);
                AtualizarCardTanks(project.frames[(int)indice].tanks);
                AtualizarItems(project.frames[(int)indice].items);
                AtualizarCheckPoints(project.frames[(int)indice].tick);

                Debug.Log(project.frames[(int)indice].tick);
                indice++;
                pode = false;
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
                AudioController audioController = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioController>();
                var unicoItem = tanquesNaCena.FirstOrDefault(t => t.Value.gameObject.activeSelf);
                audioController.tocaSomSaida(unicoItem.Value.name); 
            }
        }
    }

    public void configurarCores(int idBot)
    {
        Sprite topo = spritesTopo[idBot % spritesTopo.Length];
        spriteRendererTopo.sprite = topo;

        Sprite bases = spritesBase[idBot % spritesBase.Length];
        spriteRendererBase.sprite = bases;
    }
   

    private void AtualizarHazards(List<Hazard> hazardDatas)
    {
        List<Hazard> lasers = hazardDatas.FindAll(h => h.type == "LASER");
        List<Hazard> artilharias = hazardDatas.FindAll(h => h.type == "ARTILLERY");

        SincronizarPool<Hazard, Laser> (
            dadosJson : lasers,
            dicionarioAtivos : laserNaCena,
            poolDeLasers,
            Laser,
            l => l.id,
            aoNascer : (l, laserManager) =>
            {
                laserManager.onEntry(l.angle);
            },
            aoAtualizar : (l, laserManager) =>
            {
                laserManager.updateValues(l.angle, l.duration, l.timer);
            },
            rotinaMorte : laserManager => laserManager.onExit()
        );

        SincronizarPool<Hazard, Artillery> (
            dadosJson: artilharias,
            dicionarioAtivos: artNaCena,
            poolDeArt,
            Artillery,
            a => a.id,
            aoNascer: (a, artManager) =>
            {
                artManager.onEntry(a.radius, a.x, a.y, a.timer, project.config.arena_width, project.width);
            },
            aoAtualizar: (a, artManager) =>
            {
                artManager.updateValues(a.timer);
            },
            rotinaMorte : artManager => artManager.onExit()
        );
    }

    private void CriaTanque(Tank t)
    {
        if (!tanquesNaCena.ContainsKey(t.id))
        {
            configurarCores(t.id);
            GameObject novoTanque = Instantiate(Tank, transform);
            novoTanque.name = nomesDosTanques.ContainsKey(t.id) ? nomesDosTanques[t.id] : $"Tank_{t.id}";
            
            Tanque tankAtual = novoTanque.GetComponent<Tanque>();
            tankAtual.onEntry(t.x, t.y, t.angle, t.turret, t.hp, t.energy, t.shield);

            tanquesNaCena.Add(t.id, tankAtual);
        }
    }

    private void AtualizaTanque(Tank t)
    {
        if (tanquesNaCena.ContainsKey(t.id))
        {
            Tanque objTanque = tanquesNaCena[t.id];
            objTanque.updateValues(t.x, t.y, t.angle, t.turret, t.hp, t.energy, t.shield);
            
            // Garante que o estado visual reflita a simulação
            if (!t.alive)
            {
                objTanque.gameObject.SetActive(false);
            }
            else if (!objTanque.gameObject.activeSelf)
            {
                objTanque.gameObject.SetActive(true); // Reativa caso o tanque "reviva" ou inicie inativo
            }
        }
    }

    private void AtualizarTanques(List<Tank> dadosTanques)
    {
        if (dadosTanques == null) return;


        foreach (Tank t in dadosTanques)
        {
            CriaTanque(t);

            AtualizaTanque(t);
        }
    }

    private void AtualizarProjetis(List<Projectile> dadosProjetis)
    {
        foreach (Projectile dado in dadosProjetis)
        {
            poolManager.Morph<ManagerTiro, Projectile>(dado.id, dado);
        }
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
                cardTankUI.onEntry(t.id, $"Tank_{t.id}", this); // Exemplo de nome, você pode ajustar conforme necessário
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

        SincronizarPool<Item, Lightning>(
            dadosJson: dadosLightning,
            dicionarioAtivos: lightningNaCena,
            pool: poolDeLightnings,
            prefabBase: Lightning,
            extrairId: l => l.id,
            aoNascer: (l, lightning) =>
            {
                lightning.onEntry(l.x, l.y);
            },
            aoAtualizar: (l, lightning) =>
            {
                // Se houver alguma atualização necessária a cada frame, faça aqui
                // Por exemplo, se o Lightning tiver algum tipo de animação ou efeito
            },
            rotinaMorte: lightning =>
            {
                lightning.gameObject.SetActive(false); // Desativa o Lightning antes de devolvê-lo à pool
                return null;
            }
        );

        SincronizarPool<Item, MedKit>(
            dadosJson: dadosMedKits,
            dicionarioAtivos: medkitNaCena,
            pool: poolDeMedKits,
            prefabBase: MedKit,
            extrairId: m => m.id,
            aoNascer: (m, medKit) =>
            {
                medKit.onEntry(m.x, m.y);
            },
            aoAtualizar: (m, medKit) =>
            {
                // Se houver alguma atualização necessária a cada frame, faça aqui
                // Por exemplo, se o MedKit tiver algum tipo de animação ou efeito
            },
            rotinaMorte: medKit =>
            {
                medKit.gameObject.SetActive(false); // Desativa o MedKit antes de devolvê-lo à pool
                return null;
            }
        );
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
        if (tanquesNaCena.ContainsKey(id))
        {
            return tanquesNaCena[id];
        }
        return null;
    }
}