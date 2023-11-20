using System.Collections;
using System.Collections.Generic;
using PKNodeSystem;
using UnityEngine;

namespace PKNodeSystem
{
    public class DialogueController : MonoBehaviour
    {
        public DialogueTree tree;


        void Update()
        {
            if (tree == null) return;

            if (Input.GetKeyDown(KeyCode.P))
            {
                tree.OnTreeStart();
            }

            if (tree.treeState == TreeState.Running)
            {
                tree.Update();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                tree.OnTreeStop();
            }
        }
    }
}