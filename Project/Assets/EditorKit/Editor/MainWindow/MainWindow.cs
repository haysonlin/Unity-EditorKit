using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Hayson.EditorKit.Component;

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

        [SerializeField] PinnedPanel pinnedPanel = new();
        [SerializeField] SearchPanel searchPanel = new();
        [SerializeField] ComponentStore compMgr = new();

        PanelData[] panelTable;

        string[] panelsNameCache;
        int currentPanelIdx = 0;

        void OnEnable()
        {
            compMgr.Setup(ComponentRegistry.Infos);

            pinnedPanel.Init(compMgr.Comps, PinPresetComps);
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
                panelTable[currentPanelIdx].Panel.OnGUI(position);
            }
        }

        void OnPinnedPanelRequestPinComp(ComponentData data)
        {
            compMgr.PinComp(data);
            ComponentRecordStorage.StashComps(compMgr.Comps.Select(el => el.type));
        }

        void OnPinnedPanelRequestUnpinComp(ComponentData data)
        {
            compMgr.UnpinComp(data);
            ComponentRecordStorage.StashComps(compMgr.Comps.Select(el => el.type));
        }

        void OnPinnedPanelRequestPopupComp(ComponentData data)
        {
            compMgr.UnpinComp(data);
            ComponentContainerWindow.Create(data);
            ComponentRecordStorage.StashComps(compMgr.Comps.Select(el => el.type));
        }

        void OnSearchPanelRequestPinComp(ComponentConfig el)
        {
            compMgr.PinComp(el);
            ComponentRecordStorage.StashComps(compMgr.Comps.Select(el => el.type));
        }

        void OnSearchPanelRequestOpenComp(ComponentConfig el)
        {
            var instanceObj = ComponentInstanceCreator.Create(el.type);
            var compData = new ComponentData();
            compData.SetCompInfo(el);
            compData.SetInstanceObj(instanceObj);
            compData.Component.OnEnable();

            var mainWindow = GetWindow<MainWindow>();
            var windowPos = mainWindow.position;
            windowPos.position = new(windowPos.x + windowPos.width, windowPos.y);
            windowPos.width = 400;
            windowPos.height = 300;

            var window = ComponentContainerWindow.Create(compData);
            window.position = windowPos;
        }

        void PinPresetComps()
        {
            var list = new List<ComponentConfig>();

            var spritePacker = compMgr.CompsConfig.FirstOrDefault(el => el.type == typeof(SpritePackerSwitchTool));
            if (spritePacker != null)
            {
                list.Add(spritePacker);
            }

            var timeScaleSwitcher = compMgr.CompsConfig.FirstOrDefault(el => el.type == typeof(TimeScaleSwitchTool));
            if (timeScaleSwitcher != null)
            {
                list.Add(timeScaleSwitcher);
            }

            if (list.Count == 0)
            {
                Debug.LogError("Failed to find preset components");
            }
            else
            {
                foreach (var item in list)
                {
                    compMgr.PinComp(item);
                }
                ComponentRecordStorage.StashComps(compMgr.Comps.Select(el => el.type));
            }
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