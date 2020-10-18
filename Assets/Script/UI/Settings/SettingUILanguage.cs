using System;
using UnityEngine.UI;

namespace Canute.UI.GlobalSetting
{
    [Obsolete]
    public class SettingUILanguage : SettingUISection
    {
        public Dropdown language;

        [Obsolete]
        public override string Name => nameof(SettingUIBasic);
        [Obsolete]
        public float PlayCardDelay { get => Game.Configuration.PlayCardDelay; set => Game.Configuration.PlayCardDelay = value; }
        [Obsolete]
        public bool ShowStory { get => Game.Configuration.ShowStory; set => Game.Configuration.ShowStory = value; }

        [Obsolete]
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

        [Obsolete]
        public void ChangeLang(int id)
        {
            Language language = (Language)id;
            Game.Configuration.Language = language.ToString();
            Languages.ForceLoadLang();
        }
    }
}
