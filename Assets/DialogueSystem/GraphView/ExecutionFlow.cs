using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [System.Serializable]
    public class ExecutionFlow
    {
        public IExecutableNode NextNode { get;set; }
    }
}
