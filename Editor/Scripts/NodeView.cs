using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace PKNodeSystem
{
    public class NodeView : Node
    {
        public Action<NodeView> OnNodeSelected;
        public Action<NodeView> OnNodeDeSelected;
        public NodeBase node;
        public Port input;
        public Port output;

        public NodeView(NodeBase node) : base("Assets/NodeEditor/Editor/UI/NodeView.uxml")
        {
            this.node = node;
            this.title = node.name;
            // 将guid作为Node类中的viewDataKey关联进行后续的视图层管理
            this.viewDataKey = node.guid;
            style.left = node.pos.x;
            style.top = node.pos.y;
            CreateInputPorts();
            CreateOutputPorts();
            this.RefreshExpandedState();
            this.RefreshPorts();
            this.showInMiniMap = true;
            this.elementTypeColor = Color.magenta;
            this.SetNodeClass();
            this.Q<TextField>("Desc").value = this.node.description;
            this.Q<TextField>("Desc").RegisterValueChangedCallback(s =>
            {
                Undo.RecordObject( this.node,"ChangeValue");
                this.node.description = s.newValue;
            });
        }

        public void SetNodeClass()
        {
            if (node is SingleNode)
            {
                this.AddToClassList("Single");
            }
            else if (node is CompositeNode)
            {
                this.AddToClassList("Branch");
            }
        }

        private void CreateInputPorts()
        {
            if (node is SingleNode)
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            else if (node is CompositeNode)
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            if (input != null)
            {
                // 将端口名设置为空
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                input.showInMiniMap = true;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            if (node is SingleNode)
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            else if (node is CompositeNode)
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                output.showInMiniMap = true;
                outputContainer.Add(output);
            }
        }

        // 设置节点在节点树视图中的位置
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(node, "NodeTree (SetPosition)");
            // 将视图中节点位置设置为最新位置newPos
            base.SetPosition(newPos);
            // 将最新位置记录到运行时节点树中持久化存储
            node.pos.x = newPos.xMin;
            node.pos.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public override void OnUnselected()
        {
            base.OnSelected();
            if (OnNodeDeSelected != null)
            {
                OnNodeDeSelected.Invoke(this);
            }
        }
    }
}