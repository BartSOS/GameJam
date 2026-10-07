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
        string caminhoArquivo = Path.Combine(Application.streamingAssetsPath, "match_frames.json");

        if (File.Exists(caminhoArquivo))
        {
            string conteudoJson = await File.ReadAllTextAsync(caminhoArquivo);
            try
            {
                CameraDigital cameraDigital = Camera.main.GetComponent<CameraDigital>();
                project = JsonUtility.FromJson<Project>(conteudoJson);
                project.NormalizeDates(cameraDigital.Altura, cameraDigital.Altura);
            }

            catch (System.Exception ex)
            {
                Debug.LogError($"Erro ao desserializar o JSON: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Não encontrei o arquivo! Verifique se ele está em: {caminhoArquivo}");
        }
    }
}