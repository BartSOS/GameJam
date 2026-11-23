using UnityEngine;
using System.Collections.Generic;
using Tanques;
using WebCam;
using Datas;
using Datas.Fase;
using Tiros;
using Datas.Frames;
using Pooling;
using Artilharia;
using Cysharp.Threading.Tasks;
using HUD.Screen;
using HUD.Treino;
using Pontos;

public class ArenaVisualizer : ADependsProject
{
    [Header("Prefabs de Referência")]
    private PoolerManager poolerManager;
    private int indice;
    public int Indice{get => indice;}
    private bool gameOver;
    private Project project;
    public Tela tela;
    public HudTreinoPainel hudTreinoPainel;
    [SerializeField] private int velocidade = 4;
    private int contador = 0;

    private int folga = 0;

    protected override void AoAcordar()
    {
    }

    protected override async UniTask AoCarregarProject(Project project)
    {
        CameraDigital cameraDigital  = Camera.main.GetComponent<CameraDigital>();
        await cameraDigital.Pronto;
        this.project = project;
        StartFase(project.fase);
    }

    void Start()
    {
        poolerManager = PoolerManager.INSTANCE;
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
        hudTreinoPainel.Configurar(fase);
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

    private void FixedUpdateGame()
    {
        contador++;

        if (indice < project.frames.Count && !gameOver)
        {
            if (contador % velocidade == 0)
            {
                Frame frame = project.frames[indice];
                poolerManager.BeginTick(frame.tick);

                AtualizarTanques(frame.tanks);
                AtualizarProjeteis(frame.projectiles);
                AtualizarHazards(frame.hazards);
                AtualizarItems(frame.items);
                AtualizarCheckPoints(project.fase.pontos, frame.tick, project.fase.progresso[indice]);
                hudTreinoPainel.MostrarFrame(frame.tick);

                Debug.Log(frame.tick);
                
                indice++;
                poolerManager.UpdateTick();

                if (project.fase.progresso[indice].concluido)
                {
                    folga++;
                    if (folga == 4)
                        acabaJogo();
                }
            }
        }

        else 
        {
            acabaJogo();
        }
    }

    private void acabaJogo()
    {
        if (!gameOver && contador % velocidade == 0)
        {            
            gameOver = true;
            tela.MostraMsgFinal();
            hudTreinoPainel.DesligaHud();
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

    private void AtualizarCheckPoints(List<Ponto> pontos, int tick, Progresso progresso)
    {
        foreach (Ponto ponto in pontos)
        {
            atualizarPool<CheckPoint, Ponto>(ponto);
            CheckPoint checkPoint = poolerManager.GetVisual<CheckPoint>(ponto.id.GetHashCode());
            checkPoint.MostrarEstado(progresso);
        }
    }

    private void AtualizarProjeteis(List<Projectile> dadosProjetis)
    {
        AtualizarPoolAble<ManagerTiro, Projectile>(dadosProjetis);
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