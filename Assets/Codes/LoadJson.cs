using UnityEngine;
using Cysharp.Threading.Tasks; // Obrigatório para o UniTask
using WebCam;
using Datas;
public static class LoadJson
{
    static bool _iniciada = false;
    static UniTask<Project> _task;
    public static UniTask<Project> LoadProjectTask
    {
        get
        {
            if (!_iniciada)
            {
                _iniciada = true;
                _task = LoadProjectAsync().Preserve();
            }

            return _task;
        }
    } 
    static Project project;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Iniciar()
    {
        _ = LoadProjectTask;
    }

    static async UniTask<Project> LoadProjectAsync()
    {
        var ct = Application.exitCancellationToken;

        using var req = UnityEngine.Networking.UnityWebRequest.Get(UrlDoReplay());
        try
        {
            await req.SendWebRequest().WithCancellation(ct);
        }
        catch (UnityWebRequestException ex)
        {
            Debug.LogError($"Erro ao carregar o JSON: {ex.Message}");
            return null;
        }

        CameraDigital cam = null;
        
        await UniTask.WaitUntil(() =>
        {
            if (Camera.main != null) cam = Camera.main.GetComponent<CameraDigital>();
            return cam != null && cam.Altura > 0;
        }, cancellationToken: ct);

        try
        {
            var p = JsonUtility.FromJson<Project>(req.downloadHandler.text);
            if (p == null || p.frames == null || p.frames.Count == 0)
            {
                Debug.LogError("Replay vazio ou sem frames.");
                return null;
            }

            p.NormalizeDates(cam.Altura, cam.Altura);
            project = p;                 // só publica depois de normalizar
            return p;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Erro ao ler o replay: {ex}");
            return null;
        }
    }

    static string UrlDoReplay()
    {
        string p = Application.absoluteURL;
        int i = p.IndexOf("replay=");
        if (i < 0)
        {
            string local = Application.streamingAssetsPath + "/match_frames.json";
            return local.Contains("://") ? local : "file://" + local;
        }
        
        string rel = System.Uri.UnescapeDataString(p.Substring(i + 7).Split('&')[0]);

        return new System.Uri(new System.Uri(p), rel).ToString();
    }
}

// Alternativo:
// string caminhoArquivo = Path.Combine(Application.streamingAssetsPath, "match_frames.json");

// if (File.Exists(caminhoArquivo))
// {
//     string conteudoJson = await File.ReadAllTextAsync(caminhoArquivo);
//     try
//     {
//         CameraDigital cameraDigital = Camera.main.GetComponent<CameraDigital>();
//         project = JsonUtility.FromJson<Project>(conteudoJson);
//         project.NormalizeDates(cameraDigital.Altura, cameraDigital.Altura);
//     }

//     catch (System.Exception ex)
//     {
//         Debug.LogError($"Erro ao desserializar o JSON: {ex.Message}");
//     }
// }
// else
// {
//     Debug.LogError($"Não encontrei o arquivo! Verifique se ele está em: {caminhoArquivo}");
// }