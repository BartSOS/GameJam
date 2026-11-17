# GameJam — Visualizador da Arena (Unity)

Projeto Unity 2D que reproduz as partidas da **Arena de Tanques** da PET Game Jam 2026 a partir do replay `match_frames.json`. A Unity não simula nada: física, regras e classificação ficam na engine C++20 do [Wrapper-Orchestrator](https://github.com/BartSOS/Wrapper-Orchestrator). Este projeto só lê os frames gravados e anima tanques, tiros, perigos e itens, mais o HUD de fim de partida e o das fases de treino.

```text
códigos das equipes ──▶ Wrapper-Orchestrator (engine C++20) ──▶ match_frames.json ──▶ BOCA grava em StreamingAssets/ ──▶ GameJam (Unity) reproduz
```

O contrato do replay (campos de `config`, `bots`, `frames` e do bloco `fase`) está descrito no README do Wrapper-Orchestrator, seções 1.1 e 1.2.

---

## 1. Carregamento do replay (`Assets/Codes/Datas/LoadJson.cs`)

- Começa antes da primeira cena (`RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`) e baixa o JSON com `UnityWebRequest`.
- **Origem:** se a URL da página tiver `replay=<caminho>` (build Web), o caminho é resolvido em relação à URL. Caso contrário, lê `StreamingAssets/match_frames.json`.
- Desserializa com `JsonUtility` para a classe `Project`. Replay vazio ou sem `frames` gera erro e nada é reproduzido.
- **Normalização:** converte as coordenadas da engine (`config.arena_width` × `config.arena_height`, origem no canto) para o mundo da Unity (quadrado com a altura visível da câmera ortográfica, origem no centro). Vale para tanques, projéteis, perigos, itens e, no modo fase, jogador, alvos, pontos e artilharia programada.
- Os componentes que dependem do replay herdam de `ADependsProject` e aguardam a carga com UniTask antes de se configurar.

Campos lidos além do contrato C1: `tanques[]` (`cor` e `rotulo` de cada slot), `dono` (a equipe dona do replay), `bots[].abatido_por`, `causa_eliminacao`, `dano_causado` e `fase.progresso` (um item por frame, com distância ao ponto, frames seguidos no raio e `concluido`).

## 2. Reprodução (`Assets/Codes/Arena.cs`)

O `ArenaVisualizer` avança **um frame do replay a cada `velocidade` passos de `FixedUpdate`** (padrão 4 × 0,02 s = 0,08 s por frame). Em cada frame:

| Dado do replay | Visual | Comportamento |
| :--- | :--- | :--- |
| `frames[].tanks` | `Tanque` (`PreFabs/Tank.prefab`) | posição e ângulo interpolados, torre independente, barras de vida e energia, escudo, pisca em vermelho ao perder vida, some quando `alive` é `false` |
| `frames[].projectiles` | `ManagerTiro` (`PreFabs/Tiro.prefab`) | orientado por `vx`/`vy`, animação de disparo e de explosão |
| `hazards` com `type = LASER` | `Laser` (`PreFabs/Laser.prefab`) | feixe que gira com `angle`; troca a animação de aviso pela de queima quando o `timer` zera |
| `hazards` com `type = ARTILLERY` | `Artillery` (`PreFabs/Artillery.prefab`) | círculo de aviso cresce até o raio conforme o `timer`; explode ao sair do replay |
| `items` `HEAL` / `ENERGY` | `MedKit` / `Lightning` | aparecem e somem conforme o frame |
| `fase.pontos` | `CheckPoint` | marcador com o raio e a ordem do ponto, até o tick em que foi consumido |

**Pooling (`Assets/Codes/Pooling/`):** o `PoolerManager` mantém um pool por tipo de visual, indexado pelo `id` do dado. `Morph` cria o objeto na primeira aparição e o atualiza nas seguintes; objetos que não aparecem no tick corrente são devolvidos ao pool depois da animação de saída (`onExit`), como a explosão da artilharia.

Ao fim dos frames, a tela final é exibida.

## 3. Tela final (`Assets/Codes/HUD/Screen/`)

- Com `dono`: **VITORIA**, **DERROTA** ou **EMPATE** (mais de um bot em `posicao` 1), com subtítulo que diz quem abateu a equipe e em que frame, ou a causa (`TIRO`, `ARTILHARIA`, `LASER`, `ZONA`, `TIMEOUT`, `DESCONEXAO`).
- Cards da equipe: posição, vida final, dano causado e abates.
- Classificação de todos os bots ordenada por `posicao`, com a cor do slot vinda de `tanques[].cor` e a linha da equipe destacada.
- Sem `dono`: título **FIM DE PARTIDA** e o nome do vencedor.

## 4. HUD das fases de treino (`Assets/Codes/HUD/Treino/`)

Usado nos replays do **modo fase** (aba Treino do BOCA), que trazem o bloco `fase`. Sem `fase.progresso` (partida de torneio), o painel fica escondido.

- **Parte fixa (`HudTreinoPainel`):** etiqueta `FASE NN` (do `fase.id`), título, frame atual, prazo (`limite_frame`) e barra que fica laranja perto do prazo, além das faixas de fase concluída, prazo esgotado e tanque destruído.
- **Parte de baixo:** um prefab por fase, escolhido pelo número da fase. Há scripts para as seis fases: 01 `Chegar`, 02 `FicarNoPonto`, 03 `Percurso`, 04 `AcertarAlvos`, 05 `MiraAndando` e 06 `Sobreviver`.
- **Estado atual:** o HUD está em desenvolvimento. Na cena `War.unity`, as fases 01 e 02 já estão ligadas aos prefabs (`Andar` e `ObjetivoFicarNoPonto`); a 03 tem prefab (`Percorrer`); as fases 04 a 06 têm apenas o script.

---

## 5. Como rodar

**Requisitos:** Unity **6000.5.5f1** (`ProjectSettings/ProjectVersion.txt`), com o módulo de build da plataforma desejada.

1. Abra a pasta do repositório pelo Unity Hub (a primeira importação gera `Library/`, que é ignorada pelo git).
2. Abra a cena `Assets/Scenes/War.unity` (a única da build).
3. Coloque o replay em `Assets/StreamingAssets/match_frames.json` e aperte Play.

Replays de exemplo em `Assets/StreamingAssets/`:

| Arquivo | Conteúdo |
| :--- | :--- |
| `match_frames.json` | fase de treino `02_parar` (400 frames) |
| `replay_torneio_6bots.json` | partida de torneio com 6 bots (450 frames) |
| `replay_treino_04_mira.json` | fase de treino `04_mira_parado` (300 frames) |

Para ver outro exemplo, copie o arquivo por cima do `match_frames.json`. Para gerar um replay novo, rode o `script.sh` do Wrapper-Orchestrator e use o `match_frames.json` gerado na raiz dele.

**Builds:** os perfis estão em `Assets/Settings/Build Profiles/` (Linux, Windows e Web, com variantes Desktop/Mobile e Development/Release).

- **Linux / Windows:** o replay é lido de `StreamingAssets/match_frames.json`, dentro da pasta de dados da build. Na competição, a build é Linux e o BOCA grava o replay da partida nesse caminho a cada download do visualizador (detalhes na seção 7 do README do Wrapper-Orchestrator).
- **Web:** passe o replay pela URL da página com `?replay=<caminho do JSON>` (relativo à página). Sem o parâmetro, vale o `StreamingAssets/match_frames.json` da build.

**Dependências** (já no repositório ou no `Packages/manifest.json`): Universal Render Pipeline 2D, Input System, TextMesh Pro, [DOTween](https://dotween.demigiant.com/) (`Assets/Plugins/Demigiant/`) e [UniTask](https://github.com/Cysharp/UniTask) (`Assets/Plugins/UniTask/`).

## 6. Estrutura

```text
GameJam/
├── Assets/
│   ├── Codes/
│   │   ├── Arena.cs             # ArenaVisualizer: avança os frames e distribui os dados
│   │   ├── Datas/               # Modelo do replay (Project, Bot, Frames/, Fases/) e LoadJson
│   │   ├── Pooling/             # PoolerManager, Pooler e as bases dos objetos reaproveitados
│   │   ├── Tank/                # Tanque, barras, escudo e contorno de seleção
│   │   ├── Tiro/                # Projéteis
│   │   ├── Camera/              # Escala engine → mundo e zoom com painel lateral
│   │   ├── HUD/Screen/          # Tela final e classificação
│   │   ├── HUD/Treino/          # Painel das fases de treino e um prefab de objetivo por fase
│   │   └── Laser.cs, Artillery.cs, MedKit.cs, Lightning.cs, CheckPoint.cs
│   ├── PreFabs/                 # Tank, Tiro, Laser, Artillery, MedKit, Lightning, CheckPoint, HUD
│   ├── Scenes/War.unity         # Cena única
│   ├── StreamingAssets/         # match_frames.json lido em tempo de execução + exemplos
│   ├── Animations/, Audios/, Fonts/, Sprites/
│   ├── Plugins/                 # DOTween e UniTask
│   └── Settings/Build Profiles/ # Linux, Windows e Web
├── Packages/                    # manifest.json
└── ProjectSettings/             # ProjectVersion.txt: 6000.5.5f1
```

## 7. Repositórios relacionados

- [Wrapper-Orchestrator](https://github.com/BartSOS/Wrapper-Orchestrator): engine C++20 e orquestrador que compilam e executam os bots e geram o `match_frames.json`.
- [Starter-Kit-Game-Jam](https://github.com/BartSOS/Starter-Kit-Game-Jam): kit das equipes, com o bot inicial nas quatro linguagens e o verificador.
