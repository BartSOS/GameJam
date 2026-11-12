using Cysharp.Threading.Tasks;
using Datas;
using UnityEngine;

public abstract class ADependsProject : MonoBehaviour
{
    UniTask _pronto;
    bool _iniciado = false;
    public bool Configurado {get; private set;}

    void Awake()
    {
        Configurado = false;
        AoAcordar();
        _ = Pronto;
    }

    public UniTask Pronto
    {
        get
        {
            if (!_iniciado)
            {
                _iniciado = true;
                _pronto = Configurar().Preserve();
            }
            return _pronto;
        }
    }

    async UniTask Configurar()
    {
        var p = await LoadJson.LoadProjectTask;
        if (p == null)
        {
            AoFalhar();
            return;
        }

        await AoCarregarProject(p);
        Configurado = true;
    }

    protected virtual void AoFalhar(){}
    protected virtual void AoAcordar(){}

    protected abstract UniTask AoCarregarProject(Project project);

    protected T soDepoisDeCarregarProject<T>(T valor)
    {
        if (!Configurado)
        {
            Debug.LogError($"Tentativa de acessar {valor} antes do carregamento do Project.");
        }

        return valor;
    }
}