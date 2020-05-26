using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.GlobalSetting
{
    public class SettingUIDebug : SettingUISection
    {
        public Toggle debugMode;
        public Toggle pvP;
        public Toggle playerAutoSwitch;
        public override string Name => nameof(SettingUIDebug);

        public bool IsDebugMode { get => Game.Configuration.IsDebugMode; set => Game.Configuration.IsDebugMode = value; }
        public bool PvP { get => Game.Configuration.PvP; set => Game.Configuration.PvP = value; }
        public bool PlayerAutoSwitch { get => Game.Configuration.PlayerAutoSwitch; set => Game.Configuration.PlayerAutoSwitch = value; }

        public void Start()
        {
            debugMode.isOn = IsDebugMode;
            pvP.isOn = PvP;
            playerAutoSwitch.isOn = PlayerAutoSwitch;
        }

    }
}
