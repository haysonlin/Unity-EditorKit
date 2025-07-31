using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.MainWindow
{
    interface IPublicFeature
    {
        void ShowAlert(GUIContent notification, double fadeoutWait = 2);
    }

    record PanelData(string Name, ISubPanel Panel);

    public class MainWindow : EditorWindow, IHasCustomMenu, IPublicFeature
    {
        [MenuItem(MenuPath.EditorKitMenuPath + " %&k", false, Priority.MainPanel)]
        static void OpenWindow()
        {
            var window = GetWindow<MainWindow>(false, MenuPath.EditorKitDisplayName);
            window.minSize = new(300, 300);
        }

        PinnedPanel pinnedPanel = new();
        SearchPanel searchPanel = new();
        ComponentManager compMgr = new();

        PanelData[] panelTable;

        string[] panelsNameCache;
        int currentPanelIdx = 0;

        void OnEnable()
        {
            compMgr.Setup(ComponentRegistry.Infos);

            pinnedPanel.Init(this, compMgr.Comps);
            pinnedPanel.OnRequestPinComp += OnPinnedPanelRequestPinComp;
            pinnedPanel.OnRequestUnpinComp += OnPinnedPanelRequestUnpinComp;
            pinnedPanel.OnRequestPopupComp += OnPinnedPanelRequestPopupComp;

            searchPanel.Init(this, compMgr);
            searchPanel.OnRequestPinComp += OnSearchPanelRequestPinComp;
            searchPanel.OnRequestOpenComp += OnSearchPanelRequestOpenComp;

            if (panelTable == null)
            {
                var pinnedPanelData = new PanelData("Pinned", pinnedPanel);
                var searchPanelData = new PanelData("Browse", searchPanel);
                panelTable = new PanelData[] { pinnedPanelData, searchPanelData };
                panelsNameCache = panelTable.Select(el => el.Name).ToArray();
            }

            foreach (var item in panelTable)
            {
                item.Panel.OnEnable();
            }
        }

        void OnDisable()
        {
            foreach (var item in panelTable)
            {
                item.Panel.OnDisable();
            }
        }

        void OnGUI()
        {
            if (StyleSheet.Instance.IsSetup == false)
            {
                StyleSheet.Instance.Setup();
            }

            GUILayout.Space(3);
            using (new EditorGUILayout.VerticalScope())
            {
                currentPanelIdx = GUILayout.Toolbar(currentPanelIdx, panelsNameCache);
                panelTable[currentPanelIdx].Panel.OnUpdateGUI(position);
            }
        }

        void OnPinnedPanelRequestPinComp(InstanceData data)
        {
            compMgr.PinComp(data);
            ComponentReadWriter.StashComps(compMgr.Comps.Select(el => el.type).ToList());
        }

        void OnPinnedPanelRequestUnpinComp(InstanceData data)
        {
            compMgr.UnpinComp(data);
            ComponentReadWriter.StashComps(compMgr.Comps.Select(el => el.type).ToList());
        }

        void OnPinnedPanelRequestPopupComp(InstanceData data)
        {
            compMgr.UnpinComp(data);
            ComponentContainerWindow.Create(data);
            ComponentReadWriter.StashComps(compMgr.Comps.Select(el => el.type).ToList());
        }

        void OnSearchPanelRequestPinComp(ComponentConfig el)
        {
            compMgr.PinComp(el);
            ComponentReadWriter.StashComps(compMgr.Comps.Select(el => el.type).ToList());
        }

        void OnSearchPanelRequestOpenComp(ComponentConfig el)
        {
            var instance = ComponentManager.InstanceComp(el.type);
            var compData = new InstanceData();
            compData.SetCompInfo(el);
            compData.SetInstanceTarget(instance);

            var mainWindow = GetWindow<MainWindow>();
            var windowPos = mainWindow.position;
            windowPos.position = new(windowPos.x + windowPos.width, windowPos.y);
            windowPos.width = 400;
            windowPos.height = 300;

            var window = ComponentContainerWindow.Create(compData);
            window.position = windowPos;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            panelTable[currentPanelIdx].Panel.AddItemsToMenu(menu);
            menu.AddItem(new GUIContent("🌎 Open repository in GitHub"), false, () => Application.OpenURL(Config.RepositoryUrl));
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Version: " + Config.Version), false, null);
        }

        void IPublicFeature.ShowAlert(GUIContent notification, double fadeoutWait)
        {
            ShowNotification(notification, fadeoutWait);
        }
    }
}