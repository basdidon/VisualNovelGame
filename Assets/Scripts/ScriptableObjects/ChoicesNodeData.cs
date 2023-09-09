using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoicesNodeData : MutiOutputNodeData, ITextNodeData
{
    public Charecters Speaker { get; set; }
    public string[] TextLine { get; set; }
}
