using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutiOutputNodeData : GVNodeData
{
    [field:SerializeField] public List<GVNodeData> OutputNodeData { get; set; }
}
