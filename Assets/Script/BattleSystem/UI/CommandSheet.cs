using System.Collections.Generic;
using UnityEngine;

namespace Canute.Testing
{
    [CreateAssetMenu(fileName = "Commands", menuName = "Other/Command Sheet")]
    public class CommandSheet : ScriptableObject
    {
        public CommandList commands;

        [ContextMenu("Reorganize")]
        public void Reorganize()
        {
            List<string> names = new List<string>();
            CommandList commandInfos = new CommandList();
            foreach (var item in commands)
            {
                names.Add(item.Name);
            }

            names.Sort();

            foreach (var item in names)
            {
                commandInfos.Add(commands.Get(item));
            }
            commands = commandInfos;
        }
    }
}