using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class ArenaVisualizer : MonoBehaviour
{
    [Header("Prefabs de Referência")]
    public GameObject Laser;
    public GameObject Tank;
    public GameObject Tiro;
    public GameObject Artillery;
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

    private Dictionary<int, MovementTank> tanquesNaCena = new Dictionary<int, MovementTank>();
    private Dictionary<int, ManagerTiro> projeteisNaCena = new Dictionary<int, ManagerTiro>();
    private Dictionary<int, Laser> laserNaCena= new Dictionary<int, Laser>();
    private Dictionary<int, Artillery> artNaCena= new Dictionary<int, Artillery>();
    private Dictionary<int, CardTankUI> cardTanksAtivos = new Dictionary<int, CardTankUI>();
    private Dictionary<int, MedKit> medkitNaCena= new Dictionary<int, MedKit>();
    private Dictionary<int, Lightning> lightningNaCena= new Dictionary<int, Lightning>();

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
        cameraDigital.configCamera(loadJson.project.config.arena_width, loadJson.project.config.arena_height);
        dadosCarregados = true; 
        project = loadJson.project;
    }

    void FixedUpdate()
    {    
        if (!dadosCarregados) return;
        FixedUpdateGame();
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

   

    private void AtualizarHazards(List<HazardData> hazardDatas)
    {
        List<HazardData> lasers = hazardDatas.FindAll(h => h.type == "LASER");
        List<HazardData> artilharias = hazardDatas.FindAll(h => h.type == "ARTILLERY");

        SincronizarPool<HazardData, Laser> (
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

        SincronizarPool<HazardData, Artillery> (
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

    private void AtualizarTanques(List<Tank> dadosTanques)
    {
        if (dadosTanques == null) return;


        foreach (Tank t in dadosTanques)
        {
            if (!tanquesNaCena.ContainsKey(t.id))
            {
                configurarCores(t.id);
                GameObject novoTanque = Instantiate(Tank, transform);
                novoTanque.name = $"Tank_{t.id}";
                
                // CORREÇÃO: O GetComponent deve ser chamado no objeto instanciado (novoTanque), 
                // e não no objeto atual que gerencia a arena.
                MovementTank tankAtual = novoTanque.GetComponent<MovementTank>();

                tanquesNaCena.Add(t.id, tankAtual);
            }

            MovementTank objTanque = tanquesNaCena[t.id];
            objTanque.updateValues(t.x, t.y, t.angle, t.hp, t.energy, t.shield);
            
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

    private void AtualizarProjetis(List<ProjectileData> dadosProjetis)
    {
        SincronizarPool<ProjectileData, ManagerTiro> (
            dadosProjetis,
            projeteisNaCena,
            poolDeProjeteis,
            Tiro,
            p => p.id,
            aoNascer : (p, tiroManager) =>
            {
                float angle = Mathf.Atan2(p.vy, p.vx) * Mathf.Rad2Deg;
                StartCoroutine(tiroManager.onEntry(angle, p.x, p.y));
            },
            aoAtualizar : (p, tiroManager) =>
            {
                tiroManager.setValues(p.x, p.y);
            },
            tiroManager => tiroManager.onExit()
        );
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

    public MovementTank getTankById(int id)
    {
        if (tanquesNaCena.ContainsKey(id))
        {
            return tanquesNaCena[id];
        }
        return null;
    }
}