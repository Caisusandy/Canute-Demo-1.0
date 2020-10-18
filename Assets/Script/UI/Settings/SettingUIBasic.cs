using Canute.LanguageSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.GlobalSetting
{

    public class SettingUIBasic : SettingUISection
    {
        public Slider playCardDelay;
        public Slider volume;
        public Toggle showStory;

        [Obsolete]
        public override string Name => nameof(SettingUIBasic);
        public float Volume { get => Game.Configuration.Volume; set => Game.Configuration.Volume = value; }
        public float PlayCardDelay { get => Game.Configuration.PlayCardDelay; set => Game.Configuration.PlayCardDelay = value; }
        public bool ShowStory { get => Game.Configuration.ShowStory; set => Game.Configuration.ShowStory = value; }

        [Obsolete]
        public void Start()
        {
            playCardDelay.value = PlayCardDelay;
            volume.value = Volume;
            showStory.isOn = ShowStory;
        }
    }
}
