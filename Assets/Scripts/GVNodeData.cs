using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GVNodeData : ScriptableObject
{
    public BaseNode BaseNode { get; set; }
    public Vector2 GraphPosition { get; set; }             // position on graphview
    public List<GVNodeData> OutputNodeData { get; set; }
}
