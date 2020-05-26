﻿using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI.GlobalSetting
{
    public class SettingUI : MonoBehaviour
    {
        public List<SettingUISection> sections;
    }

    public abstract class SettingUISection : MonoBehaviour, INameable
    {
        public abstract string Name { get; }
    }
}
