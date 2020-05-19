using UnityEngine;

namespace Canute.Testing
{
    [CreateAssetMenu(fileName = "Commands", menuName = "Command Sheet")]
    public class CommandSheet : ScriptableObject
    {
        public CommandList commands;
    }
}