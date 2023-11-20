using System.Collections;
using System.Collections.Generic;
using PKNodeSystem;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/DialogueTree", fileName = "DialogueTree")]
public class DialogueTree : NodeTree
{
    public override void OnTreeStart()
    {
        base.OnTreeStart();
        rootNode = nodes[0];
        runningNode = rootNode;
        runningNode.state = NodeState.Running;
    }
}