namespace Hayson.EditorKit
{
    public class Priority
    {
        public const int MainPanel = 21;
    }

    public class MenuPath
    {
        public const string Root = "Tools";
        public const string EditorKitDisplayName = "✦ EditorKit";
        public const string EditorKitMenuPath = Root + "/" + EditorKitDisplayName;
    }

    public class Config
    {
        public const string Version = "1.0.0";
        public const string RepositoryUrl = "https://github.com/haysonlin/Unity-EditorKit/";

        // public const string AssetsPath = "Assets/EditorKit";
        public const string AssetsPath = "Packages/EditorKit";

        public const string UserSettingsDirectory = "UserSettings";
        public const string EditorKitDirectory = "EditorKit";
    }
}
