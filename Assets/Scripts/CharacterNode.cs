using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.Dialogue.VisualGraphView;

[CreateNodeMenu(menuName = "Character")]
public class CharacterNode : BaseNode
{
    public List<NPC> NPCs = new() { 
        new NPC("John"),
        new NPC("Jimmy"),
        new NPC("Emma"),
    };

    [Output] public string m_name = string.Empty;
    [field:Output] public string Papa { get; private set; }
}