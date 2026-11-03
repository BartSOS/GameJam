using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks; // Obrigatório para o UniTask
using System.Collections.Generic;
public class LoadJson : MonoBehaviour
{
    public UniTask loadProjectTask {get; private set; } 
    public Project project;
    void Awake()
    {
        loadProjectTask = LoadProjectAsync();
    }

    public async UniTask LoadProjectAsync()
    {
        using (var req = UnityEngine.Networking.UnityWebRequest.Get(UrlDoReplay()))
        {
            await req.SendWebRequest();
    
            if (req.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || req.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Erro ao carregar o JSON: {req.error}");
                return;
            }

            try
            {
                CameraDigital cameraDigital = Camera.main.GetComponent<CameraDigital>();
                project = JsonUtility.FromJson<Project>(req.downloadHandler.text);
                project.NormalizeDates(cameraDigital.Altura, cameraDigital.Altura);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Erro ao desserializar o JSON: {ex.Message}");
            }   
        }

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
    }

    string UrlDoReplay()
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