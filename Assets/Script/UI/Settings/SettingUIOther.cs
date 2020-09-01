using System.Diagnostics;
using UnityEngine;

namespace Canute.UI.GlobalSetting
{
    public class SettingUIOther : SettingUISection
    {
        public override string Name => nameof(SettingUIDebug);
        public void OpenPlayerDataFolder()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    OpenInMac();
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    OpenInWindows();
                    break;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    break;
                case RuntimePlatform.IPhonePlayer:
                    break;
                case RuntimePlatform.Android:
                    break;
                default:
                    break;
            }
        }

        private static void OpenInWindows()
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            string path = @"C:\Users\" + System.Environment.UserName + @"\AppData\LocalLow\Minerva Studio\Canute\Saves";
            psi.Arguments = "/e,/select," + path;

            System.Diagnostics.Process.Start(psi);
        }

        public static void OpenInMac()
        {
            bool openInsidesOfFolder = false;
            string path = "~/Library/Application Support/Minerva Studio/Canute/Saves";
            // try mac
            string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

            if (System.IO.Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }

            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }

            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open mac finder in windows
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }
    }
}
