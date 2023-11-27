using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PKNodeSystem
{
    public class NodeTreeViewer : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeTreeViewer, GraphView.UxmlTraits>
        {
        }

        NodeTree tree;
        public Action<NodeView> OnNodeSelected;
        public Action<NodeView> OnNodeDeSelected;

        public NodeTreeViewer()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            StyleSheet style =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeEditor/Editor/UI/NodeTreeViewerUss.uss");
            styleSheets.Add(style);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        // NodeTreeViewer视图中添加右键节点创建栏
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // 添加Node抽象类下的所有子类到右键创建栏中
            {
                var types = TypeCache.GetTypesDerivedFrom<INodeMarker>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"{type.Name}", (a) => CreateNode(type));
                }
            }
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        void CreateNode(System.Type type)
        {
            // 创建运行时节点树上的对应类型节点
            NodeBase node = tree.CreateNode(type);
            CreateNodeView(node);
        }

        void CreateNodeView(NodeBase node)
        {
            // 创建节点UI
            NodeView nodeView = new NodeView(node);
            // 节点创建成功后 让nodeView.OnNodeSelected与当前节点树上的OnNodeSelected关联 让该节点属性显示在InspectorViewer上
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.OnNodeDeSelected = OnNodeDeSelected;
            // 将对应节点UI添加到节点树视图上
            AddElement(nodeView);
        }

        // 只要节点树视图发生改变就会触发OnGraphViewChanged方法
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 对所有删除进行遍历记录 只要视图内有元素删除进行判断
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    // 找到节点树视图中删除的NodeView
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        // 并将该NodeView所关联的运行时节点删除
                        tree.DeleteNode(nodeView.node);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView PNode = edge.output.node as NodeView;
                        NodeView CNode = edge.input.node as NodeView;
                        tree.RemoveChild(PNode.node, CNode.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    NodeView PNode = edge.output.node as NodeView;
                    NodeView CNode = edge.input.node as NodeView;
                    tree.AddChild(PNode.node, CNode.node);
                }
            }

            return graphViewChange;
        }

        internal void PopulateView(NodeTree tree)
        {
            this.tree = tree;
            // 在节点树视图重新绘制之前需要取消视图变更方法OnGraphViewChanged的订阅
            // 以防止视图变更记录方法中的信息是上一个节点树的变更信息
            graphViewChanged -= OnGraphViewChanged;
            // 清除之前渲染的graphElements图层元素
            DeleteElements(graphElements);
            // 在清除节点树视图所有的元素之后重新订阅视图变更方法OnGraphViewChanged
            graphViewChanged += OnGraphViewChanged;
            RebuildGraph(tree);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endport => endport.direction != startPort.direction && endport.node != startPort.node).ToList();
        }

        public void RebuildGraph(NodeTree tree)
        {
            if (this.tree == null) return;
            foreach (var node in tree.nodes)
            {
                CreateNodeView(node);
            }

            foreach (var PNode in tree.nodes)
            {
                foreach (var CNode in PNode.children)
                {
                    NodeView pNodeView = FindNodeView(PNode);
                    NodeView cNodeView = FindNodeView(CNode);
                    Edge edge = pNodeView.output.ConnectTo(cNodeView.input);
                    AddElement(edge);
                }
            }
        }

        private NodeView FindNodeView(NodeBase node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }
    }
}