using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.Mode
{
    public static class PrototypeLoader
    {
        public static string Path => Application.persistentDataPath + "/Mode/";


        public static void Export(Prototype prototype)
        {
            string json = JsonUtility.ToJson(prototype);
            throw new NotImplementedException();
        }
    }
}
