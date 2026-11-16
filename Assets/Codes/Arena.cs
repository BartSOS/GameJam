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
using HUD.Screen;
using HUD.Treino;
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

    // private Dictionary<int, CardTankUI> cardTanksAtivos = new Dictionary<int, CardTankUI>();
    private CameraDigital cameraDigital;
    public Tela tela;
    public HudTreinoPainel hudTreinoPainel;


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
        if (indice < project.frames.Count)
        {
            if (pode)
            {
                AtualizarTanques(project.frames[indice].tanks);
                AtualizarProjeteis(project.frames[indice].projectiles);
                AtualizarHazards(project.frames[indice].hazards);
                AtualizarItems(project.frames[indice].items);
                AtualizarCheckPoints(project.fase.pontos, project.frames[indice].tick);

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

    private void AtualizarCheckPoints(List<Ponto> pontos, int tick)
    {
        foreach (Ponto ponto in pontos)
        {
            if (ponto.consumido_tick < tick)
                atualizarPool<CheckPoint, Ponto>(ponto);
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