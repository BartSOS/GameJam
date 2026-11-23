using UnityEngine;

namespace HUD.Treino.Prefab
{
    /// <summary>
    /// Se o campo aponta para um prefab (asset no Project), cria uma cópia dele na cena
    /// e devolve a cópia. Se já é um objeto da cena, devolve o próprio objeto.
    /// </summary>
    public static class HudInstancia
    {
        public static T Garantir<T>(T campo, Transform pai) where T : Component
        {
            if (campo == null) return null;
            if (campo.gameObject.scene.IsValid()) return campo; // já está na cena

            T copia = Object.Instantiate(campo, pai, false);
            copia.name = campo.name;
            return copia;
        }
    }
}
