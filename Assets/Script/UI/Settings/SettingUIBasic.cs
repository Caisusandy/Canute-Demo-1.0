using Canute.LanguageSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.GlobalSetting
{

    public class SettingUIBasic : SettingUISection
    {
        public Slider playCardDelay;
        public Toggle showStory;

        public override string Name => nameof(SettingUIBasic);
        public float PlayCardDelay { get => Game.Configuration.PlayCardDelay; set => Game.Configuration.PlayCardDelay = value; }
        public bool ShowStory { get => Game.Configuration.ShowStory; set => Game.Configuration.ShowStory = value; }

        public void Start()
        {
            playCardDelay.value = PlayCardDelay;
            showStory.isOn = ShowStory;
        }
    }
}
