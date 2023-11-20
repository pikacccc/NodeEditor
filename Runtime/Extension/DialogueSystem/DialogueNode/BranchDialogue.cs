using System.Collections.Generic;
using UnityEngine;

namespace PKNodeSystem
{
    [CreateAssetMenu(menuName = "Dialog/Node/BranchDialogue", fileName = "BranchDialogue")]
    public class BranchDialogue : CompositeNode, INodeMarker
    {
        [TextArea] public string dialogueContent;
        public int nextDialogueIndex = 0;

        public override NodeBase OnLogicUpdate()
        {
            // 判断进入哪个对话节点
            if (Input.GetKeyDown(KeyCode.A))
            {
                nextDialogueIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                nextDialogueIndex = 1;
            }

            // 判断进入下一节点条件成功时 需将节点状态改为非运行中 且 返回对应子节点
            if (Input.GetKeyDown(KeyCode.Space))
            {
                state = NodeState.Waiting;
                if (children.Count > nextDialogueIndex)
                {
                    children[nextDialogueIndex].state = NodeState.Running;
                    return children[nextDialogueIndex];
                }
            }

            return this;
        }

        public override void OnStart()
        {
            Debug.Log(dialogueContent);
        }

        // 结束时打印OnStop
        public override void OnStop()
        {
            Debug.Log("OnStop");
        }
    }
}