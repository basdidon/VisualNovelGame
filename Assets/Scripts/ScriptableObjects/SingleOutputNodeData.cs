using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SingleOutputNodeData : GVNodeData
{
    [field:SerializeField] public GVNodeData OutputNodeData { get; set; }
}
