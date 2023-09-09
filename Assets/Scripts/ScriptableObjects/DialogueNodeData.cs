using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNodeData : SingleOutputNodeData, ITextNodeData
{
    public Charecters Speaker { get; set; }
    public string[] TextLine { get; set; }
}
