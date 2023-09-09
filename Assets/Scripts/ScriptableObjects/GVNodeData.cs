using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GVNodeData : ScriptableObject
{
    [field:SerializeField] public string NodeType { get; set; }
    [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview
}
