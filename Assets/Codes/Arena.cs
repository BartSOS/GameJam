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
    public int Indice { get => indice; }
    private bool gameOver;
    private Project project;
    public Tela tela;
    public HudTreinoPainel hudTreinoPainel;
    [SerializeField] private int velocidade = 4;
    private int contador = 0;
    private int folga = 0;

    protected override void AoAcordar()
    {
        poolerManager = PoolerManager.INSTANCE;
    }

    protected override async UniTask AoCarregarProject(Project project)
    {
        CameraDigital cameraDigital = Camera.main.GetComponent<CameraDigital>();
        await cameraDigital.Pronto;
        this.project = project;

        if (poolerManager == null) poolerManager = PoolerManager.INSTANCE;

        if (project.EhTreino)
            StartFase(project.fase);
        else if (hudTreinoPainel)
            hudTreinoPainel.DesligaHud();
    }

    void Start()
    {
        if (poolerManager == null) poolerManager = PoolerManager.INSTANCE;
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
        CriaJogador(fase.jogador);
        CriaAlvos(fase.alvos);
        if (hudTreinoPainel) hudTreinoPainel.Configurar(fase, project.frames);
    }

    private void CriaJogador(Jogador jogadorFase)
    {
        if (jogadorFase == null)
        {
            Debug.LogError("Jogador não encontrado no JSON.");
            return;
        }

        atualizarPool<Tanque, Tank>(new Tank
        {
            id = jogadorFase.tank_id,
            x = jogadorFase.x,
            y = jogadorFase.y,
            angle = jogadorFase.angulo,
            hp = 100,
            energy = 100,
            shield = false,
            alive = true
        });
    }

    private void CriaAlvos(List<Alvo> alvos)
    {
        if (alvos == null) return;

        foreach (var alvo in alvos)
        {
            atualizarPool<Tanque, Tank>(new Tank
            {
                id = alvo.tank_id,
                x = alvo.x,
                y = alvo.y,
                angle = alvo.angulo,
                hp = alvo.hp,
                energy = 100,
                shield = false,
                alive = true
            });
        }
    }

    private void FixedUpdateGame()
    {
        contador++;

        if (gameOver) return;

        if (project.frames == null || indice >= project.frames.Count)
        {
            acabaJogo();
            return;
        }

        if (contador % velocidade != 0) return;

        Frame frame = project.frames[indice];
        Progresso progresso = ProgressoNoIndice(indice);

        poolerManager.BeginTick(frame.tick);

        AtualizarTanques(frame.tanks);
        AtualizarProjeteis(frame.projectiles);
        AtualizarHazards(frame.hazards);
        AtualizarItems(frame.items);

        if (project.EhTreino)
        {
            AtualizarCheckPoints(project.fase.pontos, progresso);
            if (hudTreinoPainel) hudTreinoPainel.MostrarFrame(indice);
        }

        poolerManager.UpdateTick();
        indice++;

        if (progresso != null && progresso.concluido)
        {
            folga++;
            if (folga >= 20) acabaJogo();
        }
    }

    private Progresso ProgressoNoIndice(int i)
    {
        if (!project.EhTreino) return null;
        var lista = project.fase.progresso;
        if (lista == null || i < 0 || i >= lista.Count) return null;
        return lista[i];
    }

    private void acabaJogo()
    {
        if (gameOver) return;
        gameOver = true;
        if (tela) tela.MostraMsgFinal();
        if (hudTreinoPainel) hudTreinoPainel.DesligaHud();
    }

    private void AtualizarTanques(List<Tank> dadosTanques)
    {
        if (dadosTanques == null) return;

        foreach (Tank tank in dadosTanques)
        {
            if (!tank.alive)
            {
                Tanque visual = poolerManager.GetVisual<Tanque>(tank.id);
                if (visual) visual.gameObject.SetActive(false);
                continue;
            }

            atualizarPool<Tanque, Tank>(tank);
        }
    }

    private void AtualizarCheckPoints(List<Ponto> pontos, Progresso progresso)
    {
        if (pontos == null) return;

        foreach (Ponto ponto in pontos)
        {
            atualizarPool<CheckPoint, Ponto>(ponto);
            CheckPoint checkPoint = poolerManager.GetVisual<CheckPoint>(ponto.id.GetHashCode());
            if (checkPoint && progresso != null) checkPoint.MostrarEstado(progresso);
        }
    }

    private void AtualizarProjeteis(List<Projectile> dadosProjetis)
    {
        AtualizarPoolAble<ManagerTiro, Projectile>(dadosProjetis);
    }

    private void AtualizarHazards(List<Hazard> hazardDatas)
    {
        if (hazardDatas == null) return;
        AtualizarPoolAble<Laser, Hazard>(hazardDatas.FindAll(h => h.type == "LASER"));
        AtualizarPoolAble<Artillery, Hazard>(hazardDatas.FindAll(h => h.type == "ARTILLERY"));
    }

    private void AtualizarItems(List<Item> dadosItems)
    {
        if (dadosItems == null) return;
        AtualizarPoolAble<MedKit, Item>(dadosItems.FindAll(item => item.type == "HEAL"));
        AtualizarPoolAble<Lightning, Item>(dadosItems.FindAll(item => item.type == "ENERGY"));
    }

    private void AtualizarPoolAble<TVisual, TData>(List<TData> dados) where TData : IData where TVisual : APoolAbleMonobehaviour<TData>
    {
        if (dados == null) return;
        foreach (TData data in dados)
            poolerManager.Morph<TVisual, TData>(data.id, data);
    }

    private void atualizarPool<TVisual, TData>(TData data) where TData : IData where TVisual : APoolAbleMonobehaviour<TData>
    {
        poolerManager.Morph<TVisual, TData>(data.id, data);
    }

    public Tanque getTankById(int id)
    {
        Tanque t = poolerManager.GetVisual<Tanque>(id);
        if (t == null) Debug.LogError($"Tanque com ID {id} não encontrado.");
        return t;
    }
}