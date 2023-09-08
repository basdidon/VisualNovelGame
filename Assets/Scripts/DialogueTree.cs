using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTree: ScriptableObject
{
    [field:SerializeField] public DialogueNodeData StartDialogue { get; set; }
    [SerializeField] public List<DialogueNodeData> Dialogues = new();
}
