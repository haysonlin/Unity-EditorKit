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
        public const string Version = "1.0.4";
        public const string RepositoryUrl = "https://github.com/haysonlin/Unity-EditorKit/";

        // 1️⃣ Project Path
        // public const string AssetsPath = "Assets/EditorKit";
        // 2️⃣ Package Path
        public const string AssetsPath = "Packages/com.haysonlin.editorkit";

        public const string UserSettingsDirectory = "UserSettings";
        public const string EditorKitDirectory = "EditorKit";
    }
}
