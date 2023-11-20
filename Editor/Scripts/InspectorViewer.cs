using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PKNodeSystem
{
    public class InspectorViewer : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorViewer, VisualElement.UxmlTraits>
        {
        }

        Editor editor;

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }

        internal void UpdateDeSelection(NodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
        }
    }
}