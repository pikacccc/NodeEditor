using System;
using System.Collections.Generic;
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
#endif
    }
}