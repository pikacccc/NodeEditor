using System;
using PKNodeSystem;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PKNodeSystem
{
    public class DialogueTreeEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        [SerializeField] private StyleSheet m_StyleSheet = default;

        NodeTreeViewer nodeTreeViewer;

        InspectorViewer inspectorViewer;

        private static DialogueTreeEditor m_instance;

        public NodeTree tree;

        public static DialogueTreeEditor Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = GetWindow<DialogueTreeEditor>();
                }

                return m_instance;
            }
        }

        [MenuItem("Window/DialogueTreeEditor")]
        public static void ShowWindow()
        {
            Instance.titleContent = new GUIContent("DialogueTreeEditor");
        }

        public static void OpenWindowByTree(NodeTree tree)
        {
            Instance.titleContent = new GUIContent("DialogueTreeEditor");
            Instance.tree = tree;
            Instance.nodeTreeViewer.PopulateView(tree);
        }

        private void GenerateMiniMap()
        {
            var minimap = new MiniMap { anchored = true };
            var cords = nodeTreeViewer.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            minimap.SetPosition(new Rect(0, cords.y, 200, 140));
            nodeTreeViewer.Add(minimap);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            m_VisualTreeAsset.CloneTree(root);

            root.styleSheets.Add(m_StyleSheet);

            // 将节点树视图添加到节点编辑器中
            nodeTreeViewer = root.Q<NodeTreeViewer>();
            // 将节属性面板视图添加到节点编辑器中
            inspectorViewer = root.Q<InspectorViewer>();

            nodeTreeViewer.OnNodeSelected += inspectorViewer.UpdateSelection;
            nodeTreeViewer.OnNodeDeSelected += inspectorViewer.UpdateDeSelection;
            GenerateMiniMap();
        }

        private void OnDestroy()
        {
            nodeTreeViewer.OnNodeSelected -= inspectorViewer.UpdateSelection;
            nodeTreeViewer.OnNodeDeSelected -= inspectorViewer.UpdateDeSelection;
            m_instance = null;
            tree = null;
        }

        [OnOpenAsset(1)]
        public static bool OpenDialogueTree(int instanceID, int line)
        {
            Type type = EditorUtility.InstanceIDToObject(instanceID).GetType();
            if (type == typeof(DialogueTree))
            {
                OpenWindowByTree(EditorUtility.InstanceIDToObject(instanceID) as NodeTree);
            }

            return true;
        }
    }
}