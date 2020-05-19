using System;

namespace Canute
{
    [Serializable]
    public class CanuteVersion
    {
        public int major;
        public int minor;
        public int build;
        public int revision;

        public static implicit operator Version(CanuteVersion version)
        {
            return new Version(version.major, version.minor, version.build, version.revision);
        }
        public static implicit operator string(CanuteVersion version)
        {
            return version.ToString();
        }
        public static implicit operator CanuteVersion(Version version)
        {
            return new CanuteVersion(version.Major, version.Minor, version.Build, version.Revision);
        }

        public CanuteVersion()
        {
        }
        public CanuteVersion(int major, int minor, int build, int revision)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
        }

        public override string ToString()
        {
            return major + "." + minor + "." + build + (GameData.BuildSetting.IsInDebugMode ? "." + revision : "");
        }
    }
}
