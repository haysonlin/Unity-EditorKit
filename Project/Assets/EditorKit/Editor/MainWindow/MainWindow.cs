using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.MainWindow
{
    record PanelData(string Name, ISubPanel Panel);

    public class MainWindow : EditorWindow, IHasCustomMenu
    {
        [MenuItem(MenuPath.EditorKitMenuPath + " %&k", false, Priority.MainPanel)]
        static void OpenWindow()
        {
            var window = GetWindow<MainWindow>(false, MenuPath.EditorKitDisplayName);
            window.minSize = new(300, 300);
        }

        [SerializeField] PinnedPanel pinnedPanel = new();
        [SerializeField] SearchPanel searchPanel = new();
        [SerializeField] ComponentManager compMgr = new();

        PanelData[] panelTable;

        // 工作階段數據
        string[] panelsNameCache;
        int currentPanelIdx = 0;

        void OnEnable()
        {
            compMgr.Setup(ComponentRegistry.Infos);

            pinnedPanel.Init(compMgr.CompsConfigs.ToList(), compMgr.Comps);
            pinnedPanel.OnRequestPinComp += OnPinnedPanelRequestPinComp;
            pinnedPanel.OnRequestUnpinComp += OnPinnedPanelRequestUnpinComp;
            pinnedPanel.OnRequestPopupComp += OnPinnedPanelRequestPopupComp;

            searchPanel.Init(compMgr);
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
            // menu.AddItem(new GUIContent("🔁 ReBuild components index"), false, searchPanel.RebuildIndex);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Version: " + Config.Version), false, null);
        }
    }
}