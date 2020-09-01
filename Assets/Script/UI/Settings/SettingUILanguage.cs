using System;
using UnityEngine.UI;

namespace Canute.UI.GlobalSetting
{
    public class SettingUILanguage : SettingUISection
    {
        public Dropdown language;

        public override string Name => nameof(SettingUIBasic);
        public float PlayCardDelay { get => Game.Configuration.PlayCardDelay; set => Game.Configuration.PlayCardDelay = value; }
        public bool ShowStory { get => Game.Configuration.ShowStory; set => Game.Configuration.ShowStory = value; }

        public void Start()
        {
            try
            {
                language.value = (int)Enum.Parse(typeof(Language), Game.Configuration.Language);
            }
            catch
            {
                language.value = 0;
            }
        }

        public void ChangeLang(int id)
        {
            Language language = (Language)id;
            Game.Configuration.Language = language.ToString();
            Languages.ForceLoadLang();
        }
    }
}
