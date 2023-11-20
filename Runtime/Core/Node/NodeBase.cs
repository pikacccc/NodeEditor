using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PKNodeSystem
{
    public abstract class NodeBase : ScriptableObject
    {
        [FormerlySerializedAs("State")] public NodeState state = NodeState.Waiting;

        public bool started = false;
        public List<NodeBase> children = new List<NodeBase>();
        
        [TextArea] public string description;

        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 pos;

        public NodeBase OnUpdate()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            NodeBase curNodeBase = OnLogicUpdate();

            if (state != NodeState.Running)
            {
                OnStop();
                started = false;
            }

            return curNodeBase;
        }

        public abstract NodeBase OnLogicUpdate();
        public abstract void OnStart();
        public abstract void OnStop();
    }
}