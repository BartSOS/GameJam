using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class ADependsProject : MonoBehaviour
{
    UniTask _pronto;
    bool _iniciado = false;
    bool _configurado = false;

    void Awake()
    {
        _ = Pronto;
        AoAcordar();
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

        AoCarregarProject(p);
        _configurado = true;
    }

    protected virtual void AoFalhar(){}
    protected virtual void AoAcordar(){}

    protected abstract void AoCarregarProject(Datas.Project project);

    protected T soDepoisDeCarregarProject<T>(T valor)
    {
        if (!_configurado)
        {
            Debug.LogError($"Tentativa de acessar {valor} antes do carregamento do Project.");
        }

        return valor;
    }
}