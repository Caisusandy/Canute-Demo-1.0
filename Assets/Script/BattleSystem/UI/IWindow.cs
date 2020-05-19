using UnityEngine;

namespace Canute.BattleSystem
{
    public interface IWindow : IOpenable
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        bool enabled { get; set; }

        void Close();
    }
}