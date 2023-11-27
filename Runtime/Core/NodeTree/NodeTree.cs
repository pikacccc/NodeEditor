using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PKNodeSystem
{
    public class NodeTree : ScriptableObject
    {
        public NodeBase rootNode;
        public NodeBase runningNode;

        public TreeState treeState = TreeState.Waiting;

        public List<NodeBase> nodes = new List<NodeBase>();

        public virtual void Update()
        {
            if (runningNode != null && treeState == TreeState.Running && runningNode.state == NodeState.Running)
            {
                runningNode = runningNode.OnUpdate();
            }
        }

        public virtual void OnTreeStart()
        {
            treeState = TreeState.Running;
        }

        public virtual void OnTreeStop()
        {
            treeState = TreeState.Waiting;
        }

#if UNITY_EDITOR
        public T CreateNode<T>(System.Type type) where T : NodeBase
        {
            T node = ScriptableObject.CreateInstance<T>();
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            AssetDatabase.SaveAssets();
            return node;
        }

        public NodeBase CreateNode(System.Type type)
        {
            NodeBase node = ScriptableObject.CreateInstance(type) as NodeBase;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            nodes.Add(node);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            AssetDatabase.SaveAssets();
            return node;
        }

        public T DeleteNode<T>(T node) where T : NodeBase
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
            return node;
        }

        public NodeBase DeleteNode(NodeBase node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void RemoveChild(NodeBase pNodeNode, NodeBase cNodeNode)
        {
            pNodeNode.children.Remove(cNodeNode);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(NodeBase pNodeNode, NodeBase cNodeNode)
        {
            pNodeNode.children.Add(cNodeNode);
            AssetDatabase.SaveAssets();
        }

        
        //自定义数据结构可以转换成节点编辑工具的数据结构，参考如下
        // [Button("loadFromStrategyTree")]
        // public void loadFromStrategyTree(StrategyTreeGen.StrategyTreeStore tree)
        // {
        //     this.nodes.Clear();
        //     foreach (var node in tree.Nodes)
        //     {
        //         BranchDialogue tmpNode = new BranchDialogue();
        //         tmpNode.guid = GUID.Generate().ToString();
        //         tmpNode.name = node.ID.ToString();
        //         tmpNode.pos = node.Pos;
        //         tmpNode.description = node.ID.ToString();
        //         nodes.Add(tmpNode);
        //         AssetDatabase.AddObjectToAsset(tmpNode, this);
        //     }
        //     
        //     foreach (var node in tree.Nodes)
        //     {
        //         BranchDialogue tmpNode = nodes.Find(s => s.name == node.ID.ToString()) as BranchDialogue;
        //         foreach (var childIndex in node.ChildIndex)
        //         {
        //             tmpNode.children.Add(nodes[childIndex]);
        //         }
        //     }
        //     AssetDatabase.SaveAssets();
        // }
        // [Button("loadFromWorld")]
        // public void loadFromWorld(WorldGen.WorldGenResult res)
        // {
        //     this.nodes.Clear();
        //     foreach (var node in res.Rooms)
        //     {
        //         BranchDialogue tmpNode = new BranchDialogue();
        //         tmpNode.guid = GUID.Generate().ToString();
        //         tmpNode.name = node.Id.ToString();
        //         tmpNode.pos = new Vector2(node.X*100,node.Y*100);
        //         tmpNode.description = node.Id.ToString();
        //         nodes.Add(tmpNode);
        //         AssetDatabase.AddObjectToAsset(tmpNode, this);
        //     }
        //     
        //     foreach (var node in res.Rooms)
        //     {
        //         BranchDialogue tmpNode = nodes.Find(s => s.name == node.Id.ToString()) as BranchDialogue;
        //         foreach (var childIndex in node.ChildrenId)
        //         {
        //             tmpNode.children.Add(nodes.Find(s=>s.name==childIndex.ToString()));
        //         }
        //     }
        //     AssetDatabase.SaveAssets();
        // }
#endif
    }
}