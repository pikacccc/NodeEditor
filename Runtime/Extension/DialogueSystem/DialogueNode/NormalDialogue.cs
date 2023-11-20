using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKNodeSystem
{
    [CreateAssetMenu(menuName = "Dialog/Node/NormalDialogue", fileName = "NormalDialogue")]
    public class NormalDialogue : SingleNode, INodeMarker
    {
        [TextArea] public string dialogueContent;

        public override NodeBase OnLogicUpdate()
        {
            //TODO 后续会改成按键触发的形式 使用InputSystem
            if (Input.GetKeyDown(KeyCode.Space))
            {
                state = NodeState.Waiting;
                if (children[0] != null)
                {
                    children[0].state = NodeState.Running;
                    return children[0];
                }
                else
                {
                    Debug.LogError("子节点为空请检查对话树");
                }
            }

            return this;
        }

        public override void OnStart()
        {
            Debug.Log(dialogueContent);
        }

        public override void OnStop()
        {
            Debug.Log("OnStop");
        }
    }
}